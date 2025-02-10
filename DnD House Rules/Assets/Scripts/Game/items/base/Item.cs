using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System.Linq;

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
    
    public FixedList4096Bytes<FixedString32Bytes> bodyparts;

    public List<string> GetBodyparts(){

        List<string> parts = new List<string>();
        if(bodyparts.Length == 0)
            return parts;
        foreach(FixedString32Bytes byteName in bodyparts){

            parts.Add(byteName.ToString());
        }
        return parts;
    }

    public bool CapacityLogic(item item){

        if(type != Type.backpack || item.type == Type.backpack)
            return false;
        GameManager.Singleton.RequestJsonRpc(GameManager.Singleton.interpreter.GetUsername, "host", itemInventory.ToString());
        string output = File.ReadAllText(Application.persistentDataPath + "/" + GameManager.Singleton.interpreter.GetUsername + itemInventory.ToString());
        JsonItemInventory jsonItemInventory = JsonConvert.DeserializeObject<JsonItemInventory>(output);

        JsonItemInventory newjsonItemInventory = new JsonItemInventory();
        newjsonItemInventory.items = new GameManager.JsonItem[jsonItemInventory.items.Length + 1];
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
        string output = File.ReadAllText(Application.persistentDataPath + "/" + GameManager.Singleton.interpreter.GetUsername + itemInventory.ToString());
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
                isEquipped = false,
                bodyparts = jsonItemInventory.items[i].GetBodyparts()
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
        jsonItemInventory.items = new GameManager.JsonItem[items.Length];
        float sizeTally = 0;
        int calculatedWeight = 0;
        for(int i = 0; i < items.Length; i++){
            
            if(items[i].type == Type.backpack)
                return true;
            jsonItemInventory.items[i] = new GameManager.JsonItem(){

                name = items[i].name.ToString(),
                cost = items[i].cost,
                value = items[i].value,
                type = items[i].type,
                size = items[i].size,
                amount = items[i].amount,
                weight = items[i].weight,
                itemInventory = items[i].itemInventory.ToString(),
                id = items[i].id,
                equippable = items[i].equippable,
                bodyparts = items[i].GetBodyparts()
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
        string input = JsonConvert.SerializeObject(jsonItemInventory, Formatting.Indented);
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
        GameManager.Singleton.SaveDataRpc();
        return true;
    }
    public void AddInventory(item item, int siblingIndex = -1){

        if(type != Type.backpack || item.type == Type.backpack)
            return;
        GameManager.Singleton.RequestJsonRpc(GameManager.Singleton.interpreter.GetUsername, "host", itemInventory.ToString());
        string output = File.ReadAllText(Application.persistentDataPath + "/" + GameManager.Singleton.interpreter.GetUsername + itemInventory.ToString());
        JsonItemInventory jsonItemInventory = JsonConvert.DeserializeObject<JsonItemInventory>(output);

        JsonItemInventory newjsonItemInventory = new JsonItemInventory();
        newjsonItemInventory.items = new GameManager.JsonItem[jsonItemInventory.items.Length];
        float sizeTally = 0;
        int calculatedWeight = 0;
        for(int i = 0; i < jsonItemInventory.items.Length; i++){

            newjsonItemInventory.items[i] = new GameManager.JsonItem(){

                name = jsonItemInventory.items[i].name,
                cost = jsonItemInventory.items[i].cost,
                value = jsonItemInventory.items[i].value,
                type = jsonItemInventory.items[i].type,
                size = jsonItemInventory.items[i].size,
                amount = 1,
                weight = jsonItemInventory.items[i].weight,
                itemInventory = jsonItemInventory.items[i].itemInventory,
                id = jsonItemInventory.items[i].id,
                equippable = jsonItemInventory.items[i].equippable,
                bodyparts = jsonItemInventory.items[i].bodyparts
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
        if(siblingIndex == -1)
            siblingIndex = newjsonItemInventory.items.Length;
        newjsonItemInventory.items = InsertIntoArray(newjsonItemInventory.items,  new GameManager.JsonItem(){

            name = item.name.ToString(),
            cost = item.cost,
            value = item.value,
            type = item.type,
            size = item.size,
            amount = 1,
            weight = item.weight,
            itemInventory = item.itemInventory.ToString(),
            id = item.id,
            equippable = item.equippable,
            bodyparts = item.GetBodyparts()
        }, siblingIndex);
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
        string input = JsonConvert.SerializeObject(newjsonItemInventory, Formatting.Indented);
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
        GameManager.Singleton.SaveDataRpc();
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
        string output = File.ReadAllText(Application.persistentDataPath + "/" + GameManager.Singleton.interpreter.GetUsername + itemInventory.ToString());
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
        GameManager.JsonItem[] newItems = new GameManager.JsonItem[jsonItemInventory.items.Length - 1];
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
        string input = JsonConvert.SerializeObject(newJsonItemInventory, Formatting.Indented);
        string directory = $"/{GameManager.Singleton.interpreter.GetUsername} {name}{id} Inventory.json";
        GameManager.Singleton.SaveJsonRpc(directory, input);

        // Update the itemInventory field
        itemInventory = directory;
        GameManager.Singleton.SetItemStatsRpc(GameManager.Singleton.interpreter.GetUsername, name.ToString(), id, cost, value, amount, weight - weightToRemove);
        GameManager.Singleton.SaveDataRpc();
    }

    public void RemoveInventory(item[] itemsToRemove){

        foreach(item item in itemsToRemove){

            RemoveInventory(item);
        }
    }

    public static T[] InsertIntoArray<T>(T[] array, T item, int index)
    {
        // Ensure the index is within bounds
        if (index < 0) index = 0;
        if (index > array.Length) index = array.Length;

        // Create a new array with one additional slot
        T[] newArray = new T[array.Length + 1];

        // Copy elements up to the insertion index
        for (int i = 0; i < index; i++)
        {
            newArray[i] = array[i];
        }

        // Insert the new item at the specified index
        newArray[index] = item;

        // Copy the remaining elements after the insertion index
        for (int i = index; i < array.Length; i++)
        {
            newArray[i + 1] = array[i];
        }

        return newArray;
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
        
        int count = bodyparts.Length;
        serializer.SerializeValue(ref count);

        if (serializer.IsReader)
        {
            bodyparts.Clear();
            for (int i = 0; i < count; i++)
            {
                FixedString32Bytes part = default;
                serializer.SerializeValue(ref part);
                bodyparts.Add(part);
            }
        }
        else if (serializer.IsWriter)
        {
            for (int i = 0; i < count; i++)
            {
                var part = bodyparts[i];
                serializer.SerializeValue(ref part);
            }
        }
    }
}

[Serializable]
public class JsonItemInventory{

    public GameManager.JsonItem[] items;
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