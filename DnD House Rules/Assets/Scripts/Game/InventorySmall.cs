using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Unity.Netcode;
using UnityEngine;

public class InventorySmall : NetworkBehaviour
{
    public ItemDisplay thisItemDisplay;
    item[] inventory;
    public List<NetworkObject> itemDisplays = new List<NetworkObject>();
    [SerializeField] GameObject Panel;

    void Start()
    {
        thisItemDisplay = transform.parent.parent.GetComponent<ItemDisplay>();

        if (thisItemDisplay.type != Type.backpack || !IsOwner)
            return;

        GameManager.Singleton.RequestJsonRpc(GameManager.Singleton.interpreter.GetUsername, "host", $"/{GameManager.Singleton.interpreter.GetUsername} {thisItemDisplay.nameText.text}{thisItemDisplay.id} Inventory.json");
        LoadInventory();
    }

    void OnEnable()
    {
        thisItemDisplay = transform.parent.parent.GetComponent<ItemDisplay>();

        if (thisItemDisplay.type != Type.backpack)
            return;

        GameManager.Singleton.RequestJsonRpc(GameManager.Singleton.interpreter.GetUsername, "host", $"/{GameManager.Singleton.interpreter.GetUsername} {thisItemDisplay.nameText.text}{thisItemDisplay.id} Inventory.json");
        LoadInventory();
    }

    void OnDisable()
    {
        thisItemDisplay = transform.parent.parent.GetComponent<ItemDisplay>();

        if (thisItemDisplay.type != Type.backpack)
            return;

        GameManager.Singleton.RequestJsonRpc(GameManager.Singleton.interpreter.GetUsername, "host", $"/{GameManager.Singleton.interpreter.GetUsername} {thisItemDisplay.nameText.text}{thisItemDisplay.id} Inventory.json");
        LoadInventory(false);
    }

    public void LoadInventory(bool createDisplays = true)
    {
        string filePath = $"{Application.persistentDataPath}/{GameManager.Singleton.interpreter.GetUsername}/{GameManager.Singleton.interpreter.GetUsername} {thisItemDisplay.nameText.text}{thisItemDisplay.id} Inventory.json";
        JsonItemInventory jsonItemInventory = JsonConvert.DeserializeObject<JsonItemInventory>(File.ReadAllText(filePath));

        List<item> inventoryList = new List<item>();

        foreach (var jsonItem in jsonItemInventory.items)
        {
            int maxCapacity = jsonItem.size == Size.S ? 2 : (jsonItem.size == Size.T ? 4 : jsonItem.amount);

            bool added = false;

            // Check if the item already exists in the inventory
            for (int i = 0; i < inventoryList.Count; i++)
            {
                var existingItem = inventoryList[i];

                if (existingItem.name == jsonItem.name && existingItem.size == jsonItem.size && existingItem.type == jsonItem.type)
                {
                    if (existingItem.amount < maxCapacity)
                    {
                        existingItem.amount += 1;
                        existingItem.weight += jsonItem.weight; // Weight of a single item
                        inventoryList[i] = existingItem; // Update the item in the list
                        added = true;
                        break;
                    }
                }
            }

            // If the item wasn't added to an existing slot, create a new slot
            if (!added)
            {
                inventoryList.Add(new item
                {
                    name = jsonItem.name,
                    cost = jsonItem.cost,
                    value = jsonItem.value,
                    type = jsonItem.type,
                    size = jsonItem.size,
                    amount = 1, // Each JSON item starts with 1
                    weight = jsonItem.weight, // Weight of a single item
                    itemInventory = jsonItem.itemInventory,
                    id = jsonItem.id,
                    equippable = jsonItem.equippable,
                    isEquipped = false,
                    bodyparts = jsonItem.GetBodyparts(),
                    metadata = jsonItem.metadata
                });
            }
        }

        inventory = inventoryList.ToArray();

        // Clear previous item displays
        if (itemDisplays.Count > 0)
        {
            foreach (NetworkObject itemDis in itemDisplays)
            {
                Destroy(itemDis.gameObject);
            }
            itemDisplays.Clear();
        }

        // Create item displays for the updated inventory
        if(!createDisplays)
            return;
        
        foreach (item item in inventory)
        {
            NetworkObject itemDisplayBox = Instantiate(GameManager.Singleton.itemDisplayObjectSmall, Panel.transform);
            ItemDisplaySmall itemDisplay = itemDisplayBox.GetComponent<ItemDisplaySmall>();
            itemDisplay.nameText.text = item.name.ToString();
            itemDisplay.sizeText.text = $"{item.amount}{item.size}";
            itemDisplay.weightText.text = $"{item.weight} Lbs.";
            itemDisplay.type = item.type;
            itemDisplay.thisItem = item;
            itemDisplay.occupiedInventory = this;
            itemDisplays.Add(itemDisplayBox);
        }
    }
}
