using System;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
	public GameObject tooltip;
	public GameObject inventory;
	public GameObject characterSystem;
	public GameObject craftSystem;
	private Inventory craftSystemInventory;
	private CraftSystem activeCraftSystem;
	private Inventory mainInventory;
	private Inventory characterSystemInventory;
	private Tooltip activeTooltip;

	private InputManager inputManagerDatabase;
	private HardpointManager hpManager;

	//public GameObject HPMANACanvas;

	//private Text hpText;
	//private Text manaText;
	//private Image hpImage;
	//private Image manaImage;

	//private float maxHealth = 100;
	//private float maxMana = 100;
	//private float maxDamage = 0;
	//private float maxArmor = 0;

	//public float currentHealth = 60;
	//private float currentMana = 100;
	//private float currentDamage = 0;
	//private float currentArmor = 0;

	//private int normalSize = 3;

	private void Start()
	{
		hpManager = GetComponent<HardpointManager>();

		if (inputManagerDatabase == null)
			inputManagerDatabase = (InputManager)Resources.Load("InputManager");

		if (craftSystem != null)
			activeCraftSystem = craftSystem.GetComponent<CraftSystem>();

		if (tooltip != null)
			activeTooltip = tooltip.GetComponent<Tooltip>();

		if (inventory != null)
			mainInventory = inventory.GetComponent<Inventory>();
		if (characterSystem != null)
			characterSystemInventory = characterSystem.GetComponent<Inventory>();
		if (craftSystem != null)
			craftSystemInventory = craftSystem.GetComponent<Inventory>();
	}

	// Update is called once per frame
	private void Update()
	{
		if (Input.GetKeyDown(inputManagerDatabase.CharacterSystemKeyCode))
		{
			if (!characterSystem.activeSelf)
			{
				characterSystemInventory.openInventory();
			}
			else
			{
				if (activeTooltip != null)
					activeTooltip.deactivateTooltip();
				characterSystemInventory.closeInventory();
			}
		}

		if (Input.GetKeyDown(inputManagerDatabase.InventoryKeyCode))
		{
			if (!inventory.activeSelf)
			{
				mainInventory.openInventory();
			}
			else
			{
				if (activeTooltip != null)
					activeTooltip.deactivateTooltip();
				mainInventory.closeInventory();
			}
		}

		if (Input.GetKeyDown(inputManagerDatabase.CraftSystemKeyCode))
		{
			if (!craftSystem.activeSelf)
			{
				craftSystemInventory.openInventory();
			}
			else
			{
				if (activeCraftSystem != null)
					activeCraftSystem.backToInventory();
				if (activeTooltip != null)
					activeTooltip.deactivateTooltip();
				craftSystemInventory.closeInventory();
			}
		}
	}

	public void OnEnable()
	{
		//Inventory.ItemEquip += OnEquipBackpack;
		//Inventory.UnEquipItem += OnUnequipBackpack;

		Inventory.ItemEquip += OnEquipItem;
		//Inventory.ItemConsumed += OnConsumeItem;
		Inventory.UnEquipItem += OnUnequipItem;
	}

	public void OnDisable()
	{
		//Inventory.ItemEquip -= OnEquipBackpack;
		//Inventory.UnEquipItem -= OnUnequipBackpack;

		Inventory.ItemEquip -= OnEquipItem;
		//Inventory.ItemConsumed -= OnConsumeItem;
		Inventory.UnEquipItem -= OnUnequipItem;
	}

	private void OnEquipBackpack(Item item)
	{
		throw new NotImplementedException();
		//if (item.itemType == ItemType.Backpack)
		//{
		//	for (int i = 0; i < item.itemAttributes.Count; i++)
		//	{
		//		if (mainInventory == null)
		//			mainInventory = inventory.GetComponent<Inventory>();
		//		mainInventory.sortItems();
		//		if (item.itemAttributes[i].attributeName == "Slots")
		//			changeInventorySize(item.itemAttributes[i].attributeValue);
		//	}
		//}
	}

	private void OnUnequipBackpack(Item item)
	{
		throw new NotImplementedException();
		//if (item.itemType == ItemType.Backpack)
		//	changeInventorySize(normalSize);
	}

	//this should be handled in the inventory itself
	private void changeInventorySize(int size)
	{
		dropTheRestItems(size);

		//this shouldn't be necessary
		//if (mainInventory == null)
		//	mainInventory = inventory.GetComponent<Inventory>();

		mainInventory.width = 4;
		mainInventory.height = (int)Mathf.Ceil(size / 4f);
		mainInventory.updateSlotAmount();
		mainInventory.adjustInventorySize();
	}

	private void dropTheRestItems(int size)
	{
		if (size < mainInventory.ItemsInInventory.Count)
		{
			for (int i = size; i < mainInventory.ItemsInInventory.Count; i++)
			{
				GameObject dropItem = (GameObject)Instantiate(mainInventory.ItemsInInventory[i].itemModel);
				dropItem.AddComponent<PickUpItem>();
				dropItem.GetComponent<PickUpItem>().item = mainInventory.ItemsInInventory[i];
				dropItem.transform.localPosition = GameObject.FindGameObjectWithTag("Player").transform.localPosition;
			}
		}
	}

	public void OnConsumeItem(Item item)
	{
		throw new NotImplementedException();
		//for (int i = 0; i < item.itemAttributes.Count; i++)
		//{
		//	if (item.itemAttributes[i].attributeName == "Health")
		//	{
		//		if ((currentHealth + item.itemAttributes[i].attributeValue) > maxHealth)
		//			currentHealth = maxHealth;
		//		else
		//			currentHealth += item.itemAttributes[i].attributeValue;
		//	}
		//	if (item.itemAttributes[i].attributeName == "Mana")
		//	{
		//		if ((currentMana + item.itemAttributes[i].attributeValue) > maxMana)
		//			currentMana = maxMana;
		//		else
		//			currentMana += item.itemAttributes[i].attributeValue;
		//	}
		//	if (item.itemAttributes[i].attributeName == "Armor")
		//	{
		//		if ((currentArmor + item.itemAttributes[i].attributeValue) > maxArmor)
		//			currentArmor = maxArmor;
		//		else
		//			currentArmor += item.itemAttributes[i].attributeValue;
		//	}
		//	if (item.itemAttributes[i].attributeName == "Damage")
		//	{
		//		if ((currentDamage + item.itemAttributes[i].attributeValue) > maxDamage)
		//			currentDamage = maxDamage;
		//		else
		//			currentDamage += item.itemAttributes[i].attributeValue;
		//	}
		//}
	}

	public void OnEquipItem(PartSlot slot, Item item)
	{
		switch (item.itemType)
		{
			case ItemType.Mobility:
				hpManager.ChangeMobility(item.itemModel);
				break;

			case ItemType.Turret:
				hpManager.ChangeTurret(item.itemModel);
				break;

			case ItemType.Weapon:
				hpManager.ChangeWeapon(slot, item.itemModel);
				break;

			case ItemType.None:
			default:
				break;
		}

		//for (int i = 0; i < item.itemAttributes.Count; i++)
		//{
		//	if (item.itemAttributes[i].attributeName == "Health")
		//		maxHealth += item.itemAttributes[i].attributeValue;
		//	if (item.itemAttributes[i].attributeName == "Mana")
		//		maxMana += item.itemAttributes[i].attributeValue;
		//	if (item.itemAttributes[i].attributeName == "Armor")
		//		maxArmor += item.itemAttributes[i].attributeValue;
		//	if (item.itemAttributes[i].attributeName == "Damage")
		//		maxDamage += item.itemAttributes[i].attributeValue;
		//}
	}

	public void OnUnequipItem(PartSlot slot, Item item)
	{
		switch (item.itemType)
		{
			case ItemType.Weapon:
				hpManager.ChangeWeapon(slot, null);
				break;

			case ItemType.Mobility:
			case ItemType.Turret:
			case ItemType.None:
			default:
				break;
		}

		//for (int i = 0; i < item.itemAttributes.Count; i++)
		//{
		//	if (item.itemAttributes[i].attributeName == "Health")
		//		maxHealth -= item.itemAttributes[i].attributeValue;
		//	if (item.itemAttributes[i].attributeName == "Mana")
		//		maxMana -= item.itemAttributes[i].attributeValue;
		//	if (item.itemAttributes[i].attributeName == "Armor")
		//		maxArmor -= item.itemAttributes[i].attributeValue;
		//	if (item.itemAttributes[i].attributeName == "Damage")
		//		maxDamage -= item.itemAttributes[i].attributeValue;
		//}
	}
}