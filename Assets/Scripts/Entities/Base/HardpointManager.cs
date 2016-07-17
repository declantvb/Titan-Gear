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
	[SerializeField]
	private EquipmentSystem equip;

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
		equip = GetComponent<PlayerInventory>().characterSystem.GetComponent<EquipmentSystem>();

		ChangeMobility(MobilityParts[CurrentMobilityPart]);
		ChangeTurret(TurretParts[CurrentTurretPart]);
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

		// check for new hardpoints
		UpdateHardpoints(newTurret);
		equip.SetMainSlots(MobilitySlot, TurretSlot);
	}

	public void ChangeMobility(GameObject prefab)
	{
		MobilitySlot.ChangePart(prefab);
		equip.SetMainSlots(MobilitySlot, TurretSlot);
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

		//TODO fix
		if (equip == null)
		{
			equip = GetComponent<PlayerInventory>().characterSystem.GetComponent<EquipmentSystem>();
		}

		//update inventory
		equip.SetWeaponSlots(WeaponSlots);
	}
}