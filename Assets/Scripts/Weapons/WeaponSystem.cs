using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class WeaponSystem : MonoBehaviour
{
	private Transform player;

	[SerializeField]
	private List<Weapon> ActiveWeapons = new List<Weapon>();

	// Use this for initialization
	private void Start()
	{
		player = transform.root;
	}

	// Update is called once per frame
	private void FixedUpdate()
	{
		HandleLook();

		HandleFiring();
	}

	private void HandleFiring()
	{
		var playerVelocity = player.GetComponent<Rigidbody>().velocity;

		if (FireWeapon())
		{
			bool updateWeapons = false;
			foreach (var weapon in ActiveWeapons)
			{
				if (weapon != null)
				{
					weapon.Fire(playerVelocity);
				}
				else
				{
					updateWeapons = true;
				}
			}

			if (updateWeapons)
			{
				UpdateActiveWeapons();
			}
		}
	}

	public void UpdateActiveWeapons()
	{
		ActiveWeapons = GetComponentsInChildren<Weapon>().ToList();
	}

	public abstract void HandleLook();

	public abstract bool FireWeapon();

	public abstract bool SwitchWeapon();
}