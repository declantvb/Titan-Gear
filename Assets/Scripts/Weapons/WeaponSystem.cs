using UnityEngine;
using System.Collections;
using System.Linq;

public class WeaponSystem : MonoBehaviour
{
	private Transform player;
	private Transform _bulletStartPoint;

	[Header("Weapons")]
	[SerializeField]
	public WeaponDescriptor[] AvailableWeapons = new WeaponDescriptor[0];
	[SerializeField]
	private int CurrentWeaponIndex = 0;
	[SerializeField]
	public WeaponDescriptor CurrentWeapon { get { return AvailableWeapons.ElementAtOrDefault(CurrentWeaponIndex); } }
	[SerializeField]
	private bool SwitchingWeapon;
	[SerializeField]
	private float cooldown = 0f;

	// Use this for initialization
	void Start()
	{
		player = transform.root;
		_bulletStartPoint = transform.FindChild("bullet_start");
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		WeaponSwitching();

		HandleFiring();
	}

	private void WeaponSwitching()
	{
		if (Input.GetAxis("WeaponSwitch") > 0)
		{
			if (!SwitchingWeapon)
			{
				CurrentWeaponIndex = (CurrentWeaponIndex + 1) % AvailableWeapons.Length;
				SwitchingWeapon = true;
			}
		}
		else
		{
			SwitchingWeapon = false;
		}
	}

	private void HandleFiring()
	{
		var mousedown = Input.GetAxis("Fire1");

		if (mousedown > 0f && cooldown <= 0f)
		{
			GameObject newBullet = Instantiate(CurrentWeapon.ProjectilePrefab);
			newBullet.transform.position = _bulletStartPoint.position;
			newBullet.transform.rotation = _bulletStartPoint.rotation;

			switch (CurrentWeapon.Style)
			{
				case WeaponStyle.Projectile:
				case WeaponStyle.ProjectileStraight:
				case WeaponStyle.Pulse:
					newBullet.SetLayerRecursively(LayerMask.NameToLayer("Bullets"));
					newBullet.GetComponent<Rigidbody>().AddForce((_bulletStartPoint.forward * CurrentWeapon.InitialBulletVelocity) + player.GetComponent<Rigidbody>().velocity, ForceMode.VelocityChange);
					break;
				case WeaponStyle.Laser:
					newBullet.transform.SetParent(_bulletStartPoint.transform);
					var laser = newBullet.GetComponent<Laser>();
					laser.Duration = CurrentWeapon.Duration;
					break;
				case WeaponStyle.Hitscan:
				default:
					break;
			}

			cooldown = CurrentWeapon.Cooldown;
		}

		if (cooldown > 0)
		{
			cooldown -= Time.fixedDeltaTime;
		}
	}
}
