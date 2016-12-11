using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HardpointManager : MonoBehaviour
{
	public PartSlot ChassisSlot;
	public PartSlot MobilitySlot;
	public PartSlot TurretSlot;
	public List<PartSlot> WeaponSlots;

	public GameObject CurrentChassisPart;
	public GameObject CurrentMobilityPart;
	public GameObject CurrentTurretPart;
	public GameObject[] CurrentWeaponParts;

	[SerializeField]
	private WeaponSystem weaponSystem;

	[SerializeField]
	private EquipmentSystem equip;

	private bool isPlayer;

	public Action UpdateHud;
	private bool needGuiUpdate;
	private bool init = false;

	// Use this for initialization
	private void Start()
	{
		isPlayer = GetComponent<Player>() != null;

		if (isPlayer)
		{
			equip = GetComponent<PlayerInventory>().characterSystem.GetComponent<EquipmentSystem>();
			var equipInv = equip.GetComponent<Inventory>();

			equipInv.updateItemList();
			var items = equipInv.ItemsInInventory;

			CurrentChassisPart = items.FirstOrDefault(x => x.itemType == ItemType.Chassis).itemModel;
			CurrentMobilityPart = items.FirstOrDefault(x => x.itemType == ItemType.Mobility).itemModel;
			CurrentTurretPart = items.FirstOrDefault(x => x.itemType == ItemType.Turret).itemModel;

			CurrentWeaponParts = new GameObject[equip.slotsInTotal];
			for (int i = 3; i < equip.slotsInTotal; i++)
			{
				var item = equipInv.getItemInSlot(i);
				if (item != null)
				{
					CurrentWeaponParts[i-3] = item.itemModel; 
				}
			}
		}

		ChassisSlot = GetComponentInChildren<PartSlot>();

		ChangeChassis(CurrentChassisPart);
	}

	public void Update()
	{
		// odd that this seems needed
		if (!init)
		{
			ChangeMobility(CurrentMobilityPart);
			ChangeTurret(CurrentTurretPart);

			for (int i = 0; i < CurrentWeaponParts.Length; i++)
			{
				var weapon = CurrentWeaponParts[i];
				if (weapon != null && i < WeaponSlots.Count)
				{
					ChangeWeapon(WeaponSlots[i], weapon);
				}
			}

			init = true;
		}
	}

	private void ChangeChassis(GameObject prefab)
	{
		ChassisSlot.ChangePart(prefab);

		var newChassis = ChassisSlot.currentPartInstance;

		var partSlots = newChassis.GetComponentsInChildren<PartSlot>();

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
	}

	private void OnGUI()
	{
		if (needGuiUpdate)
		{
			if (UpdateHud != null) UpdateHud();
			needGuiUpdate = false;
		}
	}

	public void ChangeTurret(GameObject prefab)
	{
		TurretSlot.ChangePart(prefab);

		var newTurret = TurretSlot.currentPartInstance;
		weaponSystem = newTurret.GetComponentInChildren<WeaponSystem>();

		if (isPlayer)
		{
			// child camera
			var holder = newTurret.transform.GetComponentInChildren<CameraHolder>().transform;
			var mainCamera = Camera.main.transform;
			mainCamera.SetParent(holder);
			mainCamera.localPosition = Vector3.zero;
			mainCamera.localRotation = Quaternion.identity;

			// set weapon controller
			weaponSystem.gameObject.AddComponent<WeaponSystemPlayer>();

			// update inventory
			equip.SetMainSlots(ChassisSlot, MobilitySlot, TurretSlot);

			needGuiUpdate = true;
		}
		else
		{
			weaponSystem.gameObject.AddComponent<WeaponSystemAI>();
		}

		// check for new hardpoints
		UpdateHardpoints(newTurret);
	}

	public void ChangeMobility(GameObject prefab)
	{
		MobilitySlot.ChangePart(prefab);
		var wheeledVehicle = MobilitySlot.currentPartInstance.GetComponentInChildren<WheeledVehicle>();

		//TODO generically add player controller to mobility
		// static for now
		if (isPlayer)
		{
			wheeledVehicle.gameObject.AddComponent<WheeledVehiclePlayer>();

			equip.SetMainSlots(ChassisSlot, MobilitySlot, TurretSlot);

			needGuiUpdate = true;
		}
		else
		{
			wheeledVehicle.gameObject.AddComponent<WheeledVehicleAI>();
		}
	}

	public void ChangeWeapon(PartSlot slot, GameObject prefab)
	{
		slot.ChangePart(prefab);
		weaponSystem.UpdateActiveWeapons();

		needGuiUpdate = true;
	}

	private void UpdateHardpoints(GameObject turret)
	{
		var allHardpoints = turret.GetComponentsInChildren<PartSlot>();
		var weaponHardpoints = allHardpoints.Except(new PartSlot[] { MobilitySlot, TurretSlot });

		// diff and update
		WeaponSlots = weaponHardpoints.ToList();

		if (isPlayer)
		{
			//TODO fix
			if (equip == null)
			{
				equip = GetComponent<PlayerInventory>().characterSystem.GetComponent<EquipmentSystem>();
			}

			//update inventory
			equip.SetWeaponSlots(WeaponSlots);
		}
	}
}