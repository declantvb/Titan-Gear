using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HardpointManager : MonoBehaviour
{
	[Header("Prefabs")]
	public GameObject[] MobilityParts;
	public GameObject[] TurretParts;
	public GameObject[] WeaponParts;

	[Header("Current")]
	public PartSlot MobilitySlot;
	public PartSlot TurretSlot;
	public GameObject WeaponPrefab { get { return WeaponParts[CurrentWeaponPart]; } }
	public List<PartSlot> WeaponSlots;

	[SerializeField]
	private bool showGUI;

	private int CurrentWeaponPart = 0;
	private int CurrentMobilityPart = 0;
	private int CurrentTurretPart = 0;

	[SerializeField]
	private WeaponSystem weaponSystem;

	// Use this for initialization
	private void Start()
	{
		var partSlots = GetComponentsInChildren<PartSlot>();
		foreach (var slot in partSlots)
		{
			switch (slot.Kind)
			{
				case PartSlot.SlotType.Mobility:
					MobilitySlot = slot;
					break;

				case PartSlot.SlotType.Turret:
					TurretSlot = slot;
					break;

				default:
					throw new System.Exception("invalid SlotType");
			}
		}

		ChangeMobility(CurrentMobilityPart);
		ChangeTurret(CurrentTurretPart);
		weaponSystem = GetComponentInChildren<WeaponSystem>();
	}

	// Update is called once per frame
	private void Update()
	{
	}

	private void OnGUI()
	{
		if (showGUI)
		{
			GUI.BeginGroup(new Rect(10, 50, 150, 500));
			var changeTurret = GUI.Button(new Rect(0, 0, 120, 25), "Change Turret");
			var changeMobility = GUI.Button(new Rect(0, 35, 120, 25), "Change Mobility");
			var changeWeapon = GUI.Button(new Rect(0, 70, 120, 25), WeaponPrefab.GetComponent<Weapon>().DisplayName);

			GUI.BeginGroup(new Rect(0, 105, 150, 400));
			var i = 0;
			var weaponsChanged = false;
			foreach (var slot in WeaponSlots)
			{
				var buttonPressed = GUI.Button(new Rect(0, i * 35, 120, 25), "Change Weapon " + i);
				if (buttonPressed)
				{
					slot.ChangePart(WeaponPrefab);
					weaponsChanged = true;
				}
				i++;
			}

			GUI.EndGroup();
			GUI.EndGroup();

			if (changeMobility)
			{
				ChangeMobility((CurrentMobilityPart + 1) % MobilityParts.Length);
			}
			if (changeTurret)
			{
				ChangeTurret((CurrentTurretPart + 1) % TurretParts.Length);
				weaponsChanged = true;
			}
			if (changeWeapon)
			{
				CurrentWeaponPart = (CurrentWeaponPart + 1) % WeaponParts.Length;
			}

			if (weaponsChanged && weaponSystem != null)
			{
				weaponSystem.UpdateActiveWeapons();
			}
		}
	}

	private void ChangeTurret(int newIndex)
	{
		CurrentTurretPart = newIndex;
		TurretSlot.ChangePart(TurretParts[CurrentTurretPart]);

		var newTurret = TurretSlot.currentPartInstance;
		weaponSystem = newTurret.GetComponentInChildren<WeaponSystem>();

		// check for new hardpoints
		UpdateHardpoints(newTurret);
	}

	private void ChangeMobility(int newIndex)
	{
		CurrentMobilityPart = newIndex;
		MobilitySlot.ChangePart(MobilityParts[CurrentMobilityPart]);
	}

	private void UpdateHardpoints(GameObject turret)
	{
		var allHardpoints = turret.GetComponentsInChildren<PartSlot>();
		var weaponHardpoints = allHardpoints.Except(new PartSlot[] { MobilitySlot, TurretSlot });

		// diff and update
		WeaponSlots = weaponHardpoints.ToList();
	}
}