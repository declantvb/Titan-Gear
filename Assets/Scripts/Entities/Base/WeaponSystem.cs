using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
	private Transform player;

	[SerializeField]
	private List<Weapon> ActiveWeapons = new List<Weapon>();

	// Use this for initialization
	private void Start()
	{
		player = transform.root;
	}

	public void FireWeapon()
	{
		var playerVelocity = player.GetComponent<Rigidbody>().velocity;
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