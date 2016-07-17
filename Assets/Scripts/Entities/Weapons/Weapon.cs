using UnityEngine;

public class Weapon : MonoBehaviour
{
	public string DisplayName;
	[SerializeField]
	public WeaponDescriptor WeaponDescriptor;
	[SerializeField]
	private float cooldown = 0f;

	private Transform _bulletStartPoint;

	// Use this for initialization
	private void Start()
	{
		_bulletStartPoint = transform.FindChild("bullet_start");
	}

	// Update is called once per frame
	private void FixedUpdate()
	{
		if (cooldown > 0)
		{
			cooldown -= Time.fixedDeltaTime;
		}
	}

	public void Fire(Vector3 playerVelocity)
	{
		if (cooldown <= 0f)
		{
			GameObject newBullet = Instantiate(WeaponDescriptor.ProjectilePrefab);
			newBullet.transform.position = _bulletStartPoint.position;
			newBullet.transform.rotation = _bulletStartPoint.rotation;

			switch (WeaponDescriptor.Style)
			{
				case WeaponStyle.Projectile:
				case WeaponStyle.ProjectileStraight:
				case WeaponStyle.Pulse:
					newBullet.SetLayerRecursively(LayerMask.NameToLayer("Bullets"));
					newBullet.GetComponent<Rigidbody>().AddForce((_bulletStartPoint.forward * WeaponDescriptor.InitialBulletVelocity) + playerVelocity, ForceMode.VelocityChange);
					var projectile = newBullet.GetComponent<Projectile>();
					projectile.DamageModifier = WeaponDescriptor.Damage;
					projectile.BlastRadius = WeaponDescriptor.Radius;
					break;
				case WeaponStyle.Laser:
					newBullet.transform.SetParent(_bulletStartPoint.transform);
					var laser = newBullet.GetComponent<Laser>();
					laser.Duration = WeaponDescriptor.Duration;
					laser.DamagePerSecond = WeaponDescriptor.Damage;
					break;
				case WeaponStyle.Hitscan:
				default:
					break;
			}

			cooldown = WeaponDescriptor.Cooldown;
		}
	}
}