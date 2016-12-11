using UnityEngine;

public class Weapon : MonoBehaviour
{
	public string DisplayName;

	[SerializeField]
	public WeaponDescriptor WeaponDescriptor;

	[SerializeField]
	public float cooldown = 0f;

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

	public void Fire(Vector3 playerVelocity, Transform missileLock)
	{
		if (cooldown > 0f)
		{
			return;
		}

		var capacitor = GetComponent<Capacitor>();
		if (capacitor != null && capacitor.Stored >= WeaponDescriptor.PowerDrawPerShot)
		{
			capacitor.Stored -= WeaponDescriptor.PowerDrawPerShot;
		}
		else
		{
			return;
		}

		GameObject newBullet = Instantiate(WeaponDescriptor.ProjectilePrefab, _bulletStartPoint.position, _bulletStartPoint.rotation) as GameObject;

		switch (WeaponDescriptor.Style)
		{
			case WeaponStyle.Projectile:
				newBullet.SetLayerRecursively(LayerMask.NameToLayer("Bullets"));
				newBullet.GetComponent<Rigidbody>().AddForce((_bulletStartPoint.forward * WeaponDescriptor.InitialBulletVelocity) + playerVelocity, ForceMode.VelocityChange);
				var projectile = newBullet.GetComponent<Projectile>();
				projectile.DamageModifier = WeaponDescriptor.Damage;
				projectile.BlastRadius = WeaponDescriptor.Radius;
				break;

			case WeaponStyle.Missile:
				newBullet.SetLayerRecursively(LayerMask.NameToLayer("Bullets"));
				newBullet.GetComponent<Rigidbody>().AddForce((_bulletStartPoint.forward * WeaponDescriptor.InitialBulletVelocity) + playerVelocity, ForceMode.VelocityChange);
				var missile = newBullet.GetComponent<Missile>();
				missile.DamageModifier = WeaponDescriptor.Damage;
				missile.BlastRadius = WeaponDescriptor.Radius;
				missile.Target = missileLock;
				break;

			case WeaponStyle.Laser:
				newBullet.transform.SetParent(_bulletStartPoint.transform);
				var laser = newBullet.GetComponent<Laser>();
				laser.Duration = WeaponDescriptor.Duration;
				laser.DamagePerSecond = WeaponDescriptor.Damage;
				break;

			default:
				break;
		}

		cooldown = WeaponDescriptor.Cooldown;
	}
}