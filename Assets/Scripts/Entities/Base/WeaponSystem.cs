using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
	private Transform player;

	public List<Weapon> ActiveWeapons = new List<Weapon>();

	// Use this for initialization
	private void Start()
	{
		player = transform.root;
	}

	public void FireWeapon()
	{
		var rbVelocity = player.GetComponent<Rigidbody>().velocity;
		bool updateWeapons = false;
		foreach (var weapon in ActiveWeapons)
		{
			if (weapon != null)
			{
				weapon.Fire(rbVelocity);
			}
			else
			{
				updateWeapons = true;
			}
		}

		//TODO
		if (ActiveWeapons.Count == 0 || updateWeapons)
		{
			UpdateActiveWeapons();
		}
	}

	public void UpdateActiveWeapons()
	{
		ActiveWeapons = GetComponentsInChildren<Weapon>().ToList();
	}
}