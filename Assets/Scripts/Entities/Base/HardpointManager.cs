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
	public List<PartSlot> WeaponSlots;

	private int CurrentMobilityPart = 0;
	private int CurrentTurretPart = 0;

	[SerializeField]
	private WeaponSystem weaponSystem;

	[SerializeField]
	private EquipmentSystem equip;

	private bool isPlayer;

	// Use this for initialization
	private void Start()
	{
		isPlayer = GetComponent<Player>() != null;

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

		if (isPlayer)
		{
			equip = GetComponent<PlayerInventory>().characterSystem.GetComponent<EquipmentSystem>();
		}

		ChangeMobility(MobilityParts[CurrentMobilityPart]);
		ChangeTurret(TurretParts[CurrentTurretPart]);

		//debugging
		ChangeWeapon(WeaponSlots[0], WeaponParts[0]);
	}

	// Update is called once per frame
	private void Update()
	{
	}

	public void ChangeTurret(GameObject prefab)
	{
		TurretSlot.ChangePart(prefab);

		var newTurret = TurretSlot.currentPartInstance;
		weaponSystem = newTurret.GetComponentInChildren<WeaponSystem>();

		if (isPlayer)
		{
			// child camera
			var holder = TurretSlot.currentPartInstance.transform.GetComponentInChildren<CameraHolder>().transform;
			var mainCamera = Camera.main.transform;
			mainCamera.SetParent(holder);
			mainCamera.localPosition = Vector3.zero;
			mainCamera.localRotation = Quaternion.identity;

			// set weapon controller
			TurretSlot.GetComponentInChildren<WeaponSystem>().gameObject.AddComponent<WeaponSystemPlayer>();

			// update inventory
			equip.SetMainSlots(MobilitySlot, TurretSlot);
		}
		else
		{
			TurretSlot.GetComponentInChildren<WeaponSystem>().gameObject.AddComponent<WeaponSystemAI>();
		}

		// check for new hardpoints
		UpdateHardpoints(newTurret);
	}

	public void ChangeMobility(GameObject prefab)
	{
		MobilitySlot.ChangePart(prefab);
		//TODO generically add player controller to mobility
		// static for now
		if (isPlayer)
		{
			MobilitySlot.GetComponentInChildren<WheeledVehicle>().gameObject.AddComponent<WheeledVehiclePlayer>();

			equip.SetMainSlots(MobilitySlot, TurretSlot);
		}
		else
		{
			MobilitySlot.GetComponentInChildren<WheeledVehicle>().gameObject.AddComponent<WheeledVehicleAI>();
		}
	}

	public void ChangeWeapon(PartSlot slot, GameObject prefab)
	{
		slot.ChangePart(prefab);
		weaponSystem.UpdateActiveWeapons();
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