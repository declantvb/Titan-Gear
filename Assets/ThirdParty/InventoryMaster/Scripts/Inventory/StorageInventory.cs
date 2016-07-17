using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StorageInventory : MonoBehaviour
{

    [SerializeField]
    public GameObject inventory;

    [SerializeField]
    public List<Item> storageItems = new List<Item>();

    [SerializeField]
    private ItemDataBaseList itemDatabase;

    [SerializeField]
    public int distanceToOpenStorage;

    public float timeToOpenStorage;

    private InputManager inputManagerDatabase;

    public int itemAmount;

    Tooltip tooltip;
    Inventory inv;

    GameObject player;

    bool closeInv;

    bool showStorage;

    public void addItemToStorage(int id, int value)
    {
        Item item = itemDatabase.getItemByID(id);
        item.itemValue = value;
        storageItems.Add(item);
    }

    void Start()
    {
        if (inputManagerDatabase == null)
            inputManagerDatabase = (InputManager)Resources.Load("InputManager");

        player = GameObject.FindGameObjectWithTag("Player");
		inventory = GameObject.Find("LootMaster").GetComponent<LootMaster>().StorageInventory;
		inv = inventory.GetComponent<Inventory>();
        ItemDataBaseList inventoryItemList = (ItemDataBaseList)Resources.Load("ItemDatabase");

        int creatingItemsForChest = 0;

        int randomItemAmount = Random.Range(1, itemAmount);

        while (creatingItemsForChest < randomItemAmount)
        {
            int randomItemNumber = Random.Range(1, inventoryItemList.itemList.Count - 1);
            int raffle = Random.Range(1, 100);

            if (raffle >= inventoryItemList.itemList[randomItemNumber].rarity)
            {
                int randomValue = Random.Range(1, inventoryItemList.itemList[randomItemNumber].getCopy().maxStack);
                Item item = inventoryItemList.itemList[randomItemNumber].getCopy();
                item.itemValue = randomValue;
                storageItems.Add(item);
                creatingItemsForChest++;
            }
        }
        if (GameObject.FindGameObjectWithTag("Tooltip") != null)
            tooltip = GameObject.FindGameObjectWithTag("Tooltip").GetComponent<Tooltip>();

    }

    public void setImportantVariables()
    {
        if (itemDatabase == null)
            itemDatabase = (ItemDataBaseList)Resources.Load("ItemDatabase");
    }

    void Update()
    {
		if (tooltip == null)
		{
			if (GameObject.FindGameObjectWithTag("Tooltip") != null)
				tooltip = GameObject.FindGameObjectWithTag("Tooltip").GetComponent<Tooltip>();
		}

        float distance = Vector3.Distance(this.gameObject.transform.position, player.transform.position);

        if (distance <= distanceToOpenStorage && Input.GetKeyDown(inputManagerDatabase.StorageKeyCode))
        {
            showStorage = !showStorage;
            OpenInventory();
        }

        if (distance > distanceToOpenStorage && showStorage)
        {
            showStorage = false;
            if (inventory.activeSelf)
            {
                storageItems.Clear();
                setListofStorage();
                inventory.SetActive(false);
                inv.deleteAllItems();
            }
            tooltip.deactivateTooltip();
        }
    }

    void OpenInventory()
    {
        if (showStorage)
        {
            inv.ItemsInInventory.Clear();
            inventory.SetActive(true);
            addItemsToInventory();
        }
        else
        {
            storageItems.Clear();
            setListofStorage();
            inventory.SetActive(false);
            inv.deleteAllItems();
            tooltip.deactivateTooltip();
        }
    }

    void setListofStorage()
    {
        Inventory inv = inventory.GetComponent<Inventory>();
        storageItems = inv.getItemList();
    }

    void addItemsToInventory()
    {
        Inventory iV = inventory.GetComponent<Inventory>();
        for (int i = 0; i < storageItems.Count; i++)
        {
            iV.addItemToInventory(storageItems[i].itemID, storageItems[i].itemValue);
        }
        iV.stackableSettings();
    }






}
