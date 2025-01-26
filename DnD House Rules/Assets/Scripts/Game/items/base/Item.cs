using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

[Serializable]
public struct item:IEquatable<item>,INetworkSerializable{

    public FixedString32Bytes name;
    public int cost;
    public int value;
    public Type type;
    public Size size;
    public int amount;
    public int weight;
    public FixedString128Bytes itemInventory;
    public int id;

    public bool equippable;
    public bool isEquipped;

    public bool CapacityLogic(item item){

        if(type != Type.backpack || item.type == Type.backpack)
            return false;
        GameManager.Singleton.RequestJsonRpc(GameManager.Singleton.interpreter.GetUsername, "host", itemInventory.ToString());
        string output = File.ReadAllText(Application.persistentDataPath + itemInventory.ToString());
        JsonItemInventory jsonItemInventory = JsonConvert.DeserializeObject<JsonItemInventory>(output);

        JsonItemInventory newjsonItemInventory = new JsonItemInventory();
        newjsonItemInventory.items = new JsonItem[jsonItemInventory.items.Length + 1];
        float sizeTally = 0;
        for(int i = 0; i < jsonItemInventory.items.Length; i++){

            switch(jsonItemInventory.items[i].size){

                case Size.L:
                    // Debug.Log($"{jsonItemInventory.items[i].name} size is Large. Adding {(float)jsonItemInventory.items[i].amount} to size tally: {sizeTally}");
                    sizeTally += (float)jsonItemInventory.items[i].amount;
                    // Debug.Log($"size tally now equals: {sizeTally}");
                    break;
                case Size.S:
                    // Debug.Log($"{jsonItemInventory.items[i].name} size is Small. Adding {(float)jsonItemInventory.items[i].amount / 2} to size tally: {sizeTally}");
                    sizeTally += (float)jsonItemInventory.items[i].amount / 2;
                    // Debug.Log($"size tally now equals: {sizeTally}");
                    break;
                case Size.T:
                    // Debug.Log($"{jsonItemInventory.items[i].name} size is Tiny. Adding {(float)jsonItemInventory.items[i].amount / 4} to size tally: {sizeTally}");
                    sizeTally += (float)jsonItemInventory.items[i].amount / 4;
                    // Debug.Log($"size tally now equals: {sizeTally}");
                    break;
            }
        }
        switch(item.size){

            case Size.L:
                sizeTally += 1f * item.amount;
                break;
            case Size.S:
                sizeTally += 0.5f * item.amount;
                break;
            case Size.T:
                sizeTally += 0.25f * item.amount;
                break;
        }

        if(sizeTally <= value){

            return true;
        }
        else{

            return false;
        }
    }

    public item[] GetInventory(){

        if(type != Type.backpack)
            return null;
        GameManager.Singleton.RequestJsonRpc(GameManager.Singleton.interpreter.GetUsername, "host", itemInventory.ToString());
        string output = File.ReadAllText(Application.persistentDataPath + itemInventory.ToString());
        JsonItemInventory jsonItemInventory = JsonConvert.DeserializeObject<JsonItemInventory>(output);
        item[] items = new item[jsonItemInventory.items.Length];
        for(int i = 0; i < jsonItemInventory.items.Length; i++){

            items[i] = new item(){

                name = jsonItemInventory.items[i].name,
                cost = jsonItemInventory.items[i].cost,
                value = jsonItemInventory.items[i].value,
                type = jsonItemInventory.items[i].type,
                size = jsonItemInventory.items[i].size,
                amount = jsonItemInventory.items[i].amount,
                weight = jsonItemInventory.items[i].weight,
                itemInventory = jsonItemInventory.items[i].itemInventory,
                id = jsonItemInventory.items[i].id,
                equippable = jsonItemInventory.items[i].equippable,
                isEquipped = false
            };
            // Debug.Log(items[i].name.ToString() + " : " + jsonItemInventory.items[i].name);
        }
        return items;
    }
    public bool SetInventory(item[] items){

        if(type != Type.backpack)
            return true;
        GameManager.Singleton.RequestJsonRpc(GameManager.Singleton.interpreter.GetUsername, "host", itemInventory.ToString());
        JsonItemInventory jsonItemInventory = new JsonItemInventory();
        jsonItemInventory.items = new JsonItem[items.Length];
        float sizeTally = 0;
        int calculatedWeight = 0;
        for(int i = 0; i < items.Length; i++){
            
            if(items[i].type == Type.backpack)
                return true;
            jsonItemInventory.items[i] = new JsonItem(){

                name = items[i].name.ToString(),
                cost = items[i].cost,
                value = items[i].value,
                type = items[i].type,
                size = items[i].size,
                amount = items[i].amount,
                weight = items[i].weight,
                itemInventory = items[i].itemInventory.ToString(),
                id = items[i].id,
                equippable = items[i].equippable
            };
            switch(items[i].size){

                case Size.L:
                    sizeTally += (float)items[i].amount;
                    break;
                case Size.S:
                    sizeTally += (float)items[i].amount / 2;
                    break;
                case Size.T:
                    sizeTally += (float)items[i].amount / 4;
                    break;
            }
            calculatedWeight += jsonItemInventory.items[i].weight;
        }
        string input = JsonConvert.SerializeObject(jsonItemInventory);
        string directory = $"/{GameManager.Singleton.interpreter.GetUsername} {name}{id} Inventory.json";
        // Debug.Log($"{sizeTally} < {value} ?");
        if(sizeTally < value){

            // Debug.Log("true");
            // GameObject.Find(GameManager.Singleton.interpreter.GetUsername).GetComponent<User>().backpack.RefreshItemDisplayBoxRpc(GameManager.Singleton.interpreter.GetUsername);
            GameManager.Singleton.SaveJsonRpc(directory, input);
            itemInventory = directory;
            GameManager.Singleton.SetItemStatsRpc(GameManager.Singleton.interpreter.GetUsername, name.ToString(), id, cost, value, amount, calculatedWeight);
        }
        else{

            // Debug.Log("false");
        }
        GameManager.Singleton.SaveData();
        return true;
    }
    public void AddInventory(item item){

        if(type != Type.backpack || item.type == Type.backpack)
            return;
        GameManager.Singleton.RequestJsonRpc(GameManager.Singleton.interpreter.GetUsername, "host", itemInventory.ToString());
        string output = File.ReadAllText(Application.persistentDataPath + itemInventory.ToString());
        JsonItemInventory jsonItemInventory = JsonConvert.DeserializeObject<JsonItemInventory>(output);

        JsonItemInventory newjsonItemInventory = new JsonItemInventory();
        newjsonItemInventory.items = new JsonItem[jsonItemInventory.items.Length + 1];
        float sizeTally = 0;
        int calculatedWeight = 0;
        for(int i = 0; i < jsonItemInventory.items.Length; i++){

            newjsonItemInventory.items[i] = new JsonItem(){

                name = jsonItemInventory.items[i].name,
                cost = jsonItemInventory.items[i].cost,
                value = jsonItemInventory.items[i].value,
                type = jsonItemInventory.items[i].type,
                size = jsonItemInventory.items[i].size,
                amount = 1,
                weight = jsonItemInventory.items[i].weight,
                itemInventory = jsonItemInventory.items[i].itemInventory,
                id = jsonItemInventory.items[i].id,
                equippable = jsonItemInventory.items[i].equippable
            };
            switch(jsonItemInventory.items[i].size){

                case Size.L:
                    // Debug.Log($"{jsonItemInventory.items[i].name} size is Large. Adding {(float)jsonItemInventory.items[i].amount} to size tally: {sizeTally}");
                    sizeTally += (float)jsonItemInventory.items[i].amount;
                    // Debug.Log($"size tally now equals: {sizeTally}");
                    break;
                case Size.S:
                    // Debug.Log($"{jsonItemInventory.items[i].name} size is Small. Adding {(float)jsonItemInventory.items[i].amount / 2} to size tally: {sizeTally}");
                    sizeTally += (float)jsonItemInventory.items[i].amount / 2;
                    // Debug.Log($"size tally now equals: {sizeTally}");
                    break;
                case Size.T:
                    // Debug.Log($"{jsonItemInventory.items[i].name} size is Tiny. Adding {(float)jsonItemInventory.items[i].amount / 4} to size tally: {sizeTally}");
                    sizeTally += (float)jsonItemInventory.items[i].amount / 4;
                    // Debug.Log($"size tally now equals: {sizeTally}");
                    break;
            }
            calculatedWeight += jsonItemInventory.items[i].weight;
        }
        newjsonItemInventory.items[jsonItemInventory.items.Length] = new JsonItem(){

            name = item.name.ToString(),
            cost = item.cost,
            value = item.value,
            type = item.type,
            size = item.size,
            amount = 1,
            weight = item.weight,
            itemInventory = item.itemInventory.ToString(),
            id = item.id,
            equippable = item.equippable
        };
        switch(item.size){

            case Size.L:
                sizeTally += 1;
                break;
            case Size.S:
                sizeTally += 0.5f;
                break;
            case Size.T:
                sizeTally += 0.25f;
                break;
        }
        calculatedWeight += item.weight;
        // Debug.Log($"after item to add, size tally equals: {sizeTally}");
        string input = JsonConvert.SerializeObject(newjsonItemInventory);
        string directory = $"/{GameManager.Singleton.interpreter.GetUsername} {name}{id} Inventory.json";
        // Debug.Log($"{sizeTally} <= {value} ?");
        if(sizeTally <= value){

            // Debug.Log("true");
            GameManager.Singleton.SaveJsonRpc(directory, input);
            itemInventory = directory;
            GameManager.Singleton.SetItemStatsRpc(GameManager.Singleton.interpreter.GetUsername, name.ToString(), id, cost, value, amount, calculatedWeight);
        }
        else{

            Debug.Log("false");
        }
        GameManager.Singleton.SaveData();
    }
    
    public void AddInventory(item[] items){

        foreach(item item in items){

            AddInventory(item);
        }
    }

    public void RemoveInventory(item itemToRemove){

        if (type != Type.backpack || itemToRemove.type == Type.backpack)
            return;
        GameManager.Singleton.RequestJsonRpc(GameManager.Singleton.interpreter.GetUsername, "host", itemInventory.ToString());
        string output = File.ReadAllText(Application.persistentDataPath + itemInventory.ToString());
        JsonItemInventory jsonItemInventory = JsonConvert.DeserializeObject<JsonItemInventory>(output);

        // Find the first instance of the item to remove
        int indexToRemove = -1;
        int weightToRemove = 0;
        for (int i = 0; i < jsonItemInventory.items.Length; i++)
        {
            if (jsonItemInventory.items[i].name == itemToRemove.name.ToString())
            {
                indexToRemove = i;
                weightToRemove = jsonItemInventory.items[i].weight;
                break;
            }
        }

        if (indexToRemove == -1)
        {
            Debug.LogWarning($"Item {itemToRemove.name} not found in inventory!");
            return;
        }

        // Create a new array with one less item
        JsonItem[] newItems = new JsonItem[jsonItemInventory.items.Length - 1];
        int newIndex = 0;
        for (int i = 0; i < jsonItemInventory.items.Length; i++)
        {
            if (i != indexToRemove)
            {
                newItems[newIndex] = jsonItemInventory.items[i];
                newIndex++;
            }
        }

        // Update the inventory
        JsonItemInventory newJsonItemInventory = new JsonItemInventory { items = newItems };
        string input = JsonConvert.SerializeObject(newJsonItemInventory);
        string directory = $"/{GameManager.Singleton.interpreter.GetUsername} {name}{id} Inventory.json";
        GameManager.Singleton.SaveJsonRpc(directory, input);

        // Update the itemInventory field
        itemInventory = directory;
        GameManager.Singleton.SetItemStatsRpc(GameManager.Singleton.interpreter.GetUsername, name.ToString(), id, cost, value, amount, weight - weightToRemove);
        GameManager.Singleton.SaveData();
    }

    public void RemoveInventory(item[] itemsToRemove){

        foreach(item item in itemsToRemove){

            RemoveInventory(item);
        }
    }

    public bool Equals(item other)
    {
        return other.name == name && other.id == id;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref name);
        serializer.SerializeValue(ref cost);
        serializer.SerializeValue(ref value);
        serializer.SerializeValue(ref type);
        serializer.SerializeValue(ref size);
        serializer.SerializeValue(ref amount);
        serializer.SerializeValue(ref weight);
        serializer.SerializeValue(ref itemInventory);
        serializer.SerializeValue(ref id);
        serializer.SerializeValue(ref equippable);
        serializer.SerializeValue(ref isEquipped);
    }
}

[Serializable]
public class JsonItem{

    public string name;
    public int cost;
    public int value;
    public Type type;
    public Size size;
    public int amount;
    public int weight;
    public string itemInventory;
    public int id;
    public bool equippable;
}

[Serializable]
public class JsonItemInventory{

    public JsonItem[] items;
}

public enum Type{

    other = -1,
    food = 0,
    medical = 1,
    weapon = 2,
    lightArmor = 3,
    heavyArmor = 4,
    capacityMult = 5,
    capacityMultL = 6,
    capacityMultS = 7,
    capacityMultT = 8,
    backpack = 9
}
public enum Size{

    T = 0,
    S = 1,
    L = 2
}

[Serializable]
public struct itemShort:IEquatable<item>, INetworkSerializable
{

    public FixedString32Bytes name;
    public int id;

    public bool Equals(item other)
    {
        return other.name == name && other.id == id;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref name);
        serializer.SerializeValue(ref id);
    }
}