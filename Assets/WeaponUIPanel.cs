using System;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUIPanel : MonoBehaviour
{
	public Weapon Weapon;
	private Capacitor WeaponCapacitor;

	public GameObject Icon;
	public GameObject Cooldown;
	public GameObject Power;

	public void OnGUI()
	{
		Cooldown.GetComponent<Slider>().value = Weapon.cooldown / Weapon.WeaponDescriptor.Cooldown;
		Power.GetComponent<Slider>().value = WeaponCapacitor.Stored / WeaponCapacitor.Capacity;
	}

	public void Initialise()
	{
		WeaponCapacitor = Weapon.GetComponent<Capacitor>();
	}
}