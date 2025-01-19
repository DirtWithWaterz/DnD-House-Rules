using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Unity.Netcode;
using UnityEngine;

public class InventorySmall : NetworkBehaviour
{

    ItemDisplay thisItem;
    item[] inventory;
    List<NetworkObject> itemDisplays = new List<NetworkObject>();
    [SerializeField] GameObject Panel;

    // Start is called before the first frame update
    void Start(){

        thisItem = transform.parent.parent.GetComponent<ItemDisplay>();

        if(thisItem.type != Type.backpack)
            return;

        if(!IsOwner)
            return;

        JsonItemInventory jsonItemInventory = JsonConvert.DeserializeObject<JsonItemInventory>(
            File.ReadAllText($"{Application.persistentDataPath}/{GameManager.Singleton.interpreter.GetUsername} {thisItem.nameText.text}{thisItem.id} Inventory.json")
        );

        Dictionary<string, item> inventoryDict = new Dictionary<string, item>();

        foreach (var jsonItem in jsonItemInventory.items)
        {
            if (inventoryDict.TryGetValue(jsonItem.name, out var existingItem))
            {
                // Debug.Log($"{jsonItem.amount} + {existingItem.amount} = {jsonItem.amount + existingItem.amount} < 4 ?");
                // Update existing item if conditions allow
                if (jsonItem.size != Size.L &&
                    !(jsonItem.size == Size.S && jsonItem.amount + existingItem.amount > 2) &&
                    !(jsonItem.size == Size.T && jsonItem.amount + existingItem.amount > 4) &&
                    jsonItem.type != Type.backpack)
                {
                    inventoryDict[jsonItem.name] = new item{

                        name = existingItem.name,
                        cost = existingItem.cost,
                        value = existingItem.value,
                        type = existingItem.type,
                        size = existingItem.size,
                        amount = existingItem.amount + 1,
                        weight = existingItem.weight + (existingItem.weight / existingItem.amount),
                        itemInventory = jsonItem.itemInventory,
                        id = existingItem.id
                    };
                    // Debug.Log($"true. amount now equals {existingItem.amount}.");
                }
                else{
                    // Debug.Log("false.");
                }
            }
            else
            {
                // Add new item to the inventory
                inventoryDict[jsonItem.name] = new item
                {
                    name = jsonItem.name,
                    cost = jsonItem.cost,
                    value = jsonItem.value,
                    type = jsonItem.type,
                    size = jsonItem.size,
                    amount = jsonItem.amount,
                    weight = jsonItem.weight,
                    itemInventory = jsonItem.itemInventory,
                    id = jsonItem.id
                };
            }
        }

        inventory = new item[inventoryDict.Count];
        inventoryDict.Values.CopyTo(inventory, 0);

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
        foreach (item item in inventory)
        {
            NetworkObject itemDisplayBox = Instantiate(GameManager.Singleton.itemDisplayObjectSmall, Panel.transform);
            ItemDisplaySmall itemDisplay = itemDisplayBox.GetComponent<ItemDisplaySmall>();
            itemDisplay.nameText.text = item.name.ToString();
            itemDisplay.sizeText.text = $"{item.amount}{item.size}";
            itemDisplay.weightText.text = $"{item.weight} Lbs.";
            itemDisplay.type = item.type;
            itemDisplays.Add(itemDisplayBox);
        }
    }

    void OnEnable(){

        thisItem = transform.parent.parent.GetComponent<ItemDisplay>();

        if(thisItem.type != Type.backpack)
            return;

        JsonItemInventory jsonItemInventory = JsonConvert.DeserializeObject<JsonItemInventory>(
            File.ReadAllText($"{Application.persistentDataPath}/{GameManager.Singleton.interpreter.GetUsername} {thisItem.nameText.text}{thisItem.id} Inventory.json")
        );

        Dictionary<string, item> inventoryDict = new Dictionary<string, item>();

        foreach (var jsonItem in jsonItemInventory.items)
        {
            if (inventoryDict.TryGetValue(jsonItem.name, out var existingItem))
            {
                // Update existing item if conditions allow
                if (jsonItem.size != Size.L &&
                    !(jsonItem.size == Size.S && jsonItem.amount + existingItem.amount > 2) &&
                    !(jsonItem.size == Size.T && jsonItem.amount + existingItem.amount > 4) &&
                    jsonItem.type != Type.backpack)
                {
                    inventoryDict[jsonItem.name] = new item{

                        name = existingItem.name,
                        cost = existingItem.cost,
                        value = existingItem.value,
                        type = existingItem.type,
                        size = existingItem.size,
                        amount = existingItem.amount + 1,
                        weight = existingItem.weight + (existingItem.weight / existingItem.amount),
                        itemInventory = jsonItem.itemInventory,
                        id = existingItem.id
                    };
                }
            }
            else
            {
                // Add new item to the inventory
                inventoryDict[jsonItem.name] = new item
                {
                    name = jsonItem.name,
                    cost = jsonItem.cost,
                    value = jsonItem.value,
                    type = jsonItem.type,
                    size = jsonItem.size,
                    amount = jsonItem.amount,
                    weight = jsonItem.weight,
                    itemInventory = jsonItem.itemInventory,
                    id = jsonItem.id
                };
            }
        }

        inventory = new item[inventoryDict.Count];
        inventoryDict.Values.CopyTo(inventory, 0);

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
        foreach (item item in inventory)
        {
            NetworkObject itemDisplayBox = Instantiate(GameManager.Singleton.itemDisplayObjectSmall, Panel.transform);
            ItemDisplaySmall itemDisplay = itemDisplayBox.GetComponent<ItemDisplaySmall>();
            itemDisplay.nameText.text = item.name.ToString();
            itemDisplay.sizeText.text = $"{item.amount}{item.size}";
            itemDisplay.weightText.text = $"{item.weight} Lbs.";
            itemDisplay.type = item.type;
            itemDisplays.Add(itemDisplayBox);
        }
    }
}
