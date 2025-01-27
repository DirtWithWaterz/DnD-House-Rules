using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;


[Serializable]
public struct userData:IEquatable<userData>,INetworkSerializable{

    public ulong id;
    public FixedString32Bytes username;

    public NetworkObjectReference objRef;

    public int Str, Dex, Con, Int, Wis, Cha;

    public int Lvl;

    public int HP_NECK,
        HP_HEAD, HP_CHEST,
        HP_ARM_LEFT, HP_FOREARM_LEFT,
        HP_HAND_LEFT, HP_ARM_RIGHT,
        HP_FOREARM_RIGHT, HP_HAND_RIGHT,
        HP_TORSO, HP_PELVIS,
        HP_THIGH_LEFT, HP_CRUS_LEFT,
        HP_FOOT_LEFT, HP_THIGH_RIGHT,
        HP_CRUS_RIGHT, HP_FOOT_RIGHT;
    
    public int AC_NECK,
        AC_HEAD, AC_CHEST,
        AC_ARM_LEFT, AC_FOREARM_LEFT,
        AC_HAND_LEFT, AC_ARM_RIGHT,
        AC_FOREARM_RIGHT, AC_HAND_RIGHT,
        AC_TORSO, AC_PELVIS,
        AC_THIGH_LEFT, AC_CRUS_LEFT,
        AC_FOOT_LEFT, AC_THIGH_RIGHT,
        AC_CRUS_RIGHT, AC_FOOT_RIGHT;
    
    public FixedString32Bytes CONDITION_NECK,
        CONDITION_HEAD, CONDITION_CHEST,
        CONDITION_ARM_LEFT, CONDITION_FOREARM_LEFT,
        CONDITION_HAND_LEFT, CONDITION_ARM_RIGHT,
        CONDITION_FOREARM_RIGHT, CONDITION_HAND_RIGHT,
        CONDITION_TORSO, CONDITION_PELVIS,
        CONDITION_THIGH_LEFT, CONDITION_CRUS_LEFT,
        CONDITION_FOOT_LEFT, CONDITION_THIGH_RIGHT,
        CONDITION_CRUS_RIGHT, CONDITION_FOOT_RIGHT;

    // public FixedString4096Bytes

    public bool barbarian;
    public int baseSpeed;
    public bool initProf;

    public bool Equals(userData other)
    {
        return other.id == id;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref id);
        serializer.SerializeValue(ref username);
        serializer.SerializeValue(ref objRef);

        serializer.SerializeValue(ref Str);
        serializer.SerializeValue(ref Dex);
        serializer.SerializeValue(ref Con);
        serializer.SerializeValue(ref Int);
        serializer.SerializeValue(ref Wis);
        serializer.SerializeValue(ref Cha);

        serializer.SerializeValue(ref Lvl);

        serializer.SerializeValue(ref HP_NECK);
        serializer.SerializeValue(ref HP_HEAD);
        serializer.SerializeValue(ref HP_CHEST);
        serializer.SerializeValue(ref HP_ARM_LEFT);
        serializer.SerializeValue(ref HP_FOREARM_LEFT);
        serializer.SerializeValue(ref HP_HAND_LEFT);
        serializer.SerializeValue(ref HP_ARM_RIGHT);
        serializer.SerializeValue(ref HP_FOREARM_RIGHT);
        serializer.SerializeValue(ref HP_HAND_RIGHT);
        serializer.SerializeValue(ref HP_TORSO);
        serializer.SerializeValue(ref HP_PELVIS);
        serializer.SerializeValue(ref HP_THIGH_LEFT);
        serializer.SerializeValue(ref HP_CRUS_LEFT);
        serializer.SerializeValue(ref HP_FOOT_LEFT);
        serializer.SerializeValue(ref HP_THIGH_RIGHT);
        serializer.SerializeValue(ref HP_CRUS_RIGHT);
        serializer.SerializeValue(ref HP_FOOT_RIGHT);

        serializer.SerializeValue(ref AC_NECK);
        serializer.SerializeValue(ref AC_HEAD);
        serializer.SerializeValue(ref AC_CHEST);
        serializer.SerializeValue(ref AC_ARM_LEFT);
        serializer.SerializeValue(ref AC_FOREARM_LEFT);
        serializer.SerializeValue(ref AC_HAND_LEFT);
        serializer.SerializeValue(ref AC_ARM_RIGHT);
        serializer.SerializeValue(ref AC_FOREARM_RIGHT);
        serializer.SerializeValue(ref AC_HAND_RIGHT);
        serializer.SerializeValue(ref AC_TORSO);
        serializer.SerializeValue(ref AC_PELVIS);
        serializer.SerializeValue(ref AC_THIGH_LEFT);
        serializer.SerializeValue(ref AC_CRUS_LEFT);
        serializer.SerializeValue(ref AC_FOOT_LEFT);
        serializer.SerializeValue(ref AC_THIGH_RIGHT);
        serializer.SerializeValue(ref AC_CRUS_RIGHT);
        serializer.SerializeValue(ref AC_FOOT_RIGHT);

        serializer.SerializeValue(ref CONDITION_NECK);
        serializer.SerializeValue(ref CONDITION_HEAD);
        serializer.SerializeValue(ref CONDITION_CHEST);
        serializer.SerializeValue(ref CONDITION_ARM_LEFT);
        serializer.SerializeValue(ref CONDITION_FOREARM_LEFT);
        serializer.SerializeValue(ref CONDITION_HAND_LEFT);
        serializer.SerializeValue(ref CONDITION_ARM_RIGHT);
        serializer.SerializeValue(ref CONDITION_FOREARM_RIGHT);
        serializer.SerializeValue(ref CONDITION_HAND_RIGHT);
        serializer.SerializeValue(ref CONDITION_TORSO);
        serializer.SerializeValue(ref CONDITION_PELVIS);
        serializer.SerializeValue(ref CONDITION_THIGH_LEFT);
        serializer.SerializeValue(ref CONDITION_CRUS_LEFT);
        serializer.SerializeValue(ref CONDITION_FOOT_LEFT);
        serializer.SerializeValue(ref CONDITION_THIGH_RIGHT);
        serializer.SerializeValue(ref CONDITION_CRUS_RIGHT);
        serializer.SerializeValue(ref CONDITION_FOOT_RIGHT);

        serializer.SerializeValue(ref barbarian);
        serializer.SerializeValue(ref baseSpeed);
        serializer.SerializeValue(ref initProf);

    }
}

public class GameManager : NetworkBehaviour
{

    public static GameManager Singleton;

    void Awake(){

        Singleton = Singleton != null && Singleton != this ? null : this;
    }

    public GameObject terminal;

    public NetworkList<userData> userDatas;
    public NetworkList<item> items;

    public InqueCalendar inqueCalendar;
    public Interpreter interpreter;

    public TerminalManager terminalManager;

    public NetworkObject itemDisplayObject, itemDisplayObjectSmall, itemDisplayBoxMouse, armorSlotDisplayObject;

    public Dictionary<string, string> conditionsKeyValue = new Dictionary<string, string>();
    public Dictionary<string, string> conditionsValueKey = new Dictionary<string, string>();
    public NetworkVariable<int> itemIdTally = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);


    void Start(){
        
        userDatas = new NetworkList<userData>();
        items = new NetworkList<item>();
        userDatas.Initialize(this);

        userDatas.OnListChanged += UpdateNames;

        inqueCalendar = GetComponent<InqueCalendar>();

        interpreter = FindObjectOfType<Interpreter>().GetComponent<Interpreter>();

        terminalManager = terminal.transform.GetChild(0).GetComponent<TerminalManager>();
    }

    private void UpdateNames(NetworkListEvent<userData> changeEvent)
    {
        foreach(userData data in userDatas){

            if(data.objRef.TryGet(out NetworkObject netObj))
                netObj.name = data.username.ToString();
        }
    }

    [Rpc(SendTo.Server)]
    public void UpdateIdTallyRpc(){

        itemIdTally.Value++;
    }

    [Serializable]
    public class JsonUserData{

        public string id;
        public string username;

        public int Str, Dex, Con, Int, Wis, Cha;

        public int Lvl;

        public int HP_NECK,
            HP_HEAD, HP_CHEST,
            HP_ARM_LEFT, HP_FOREARM_LEFT,
            HP_HAND_LEFT, HP_ARM_RIGHT,
            HP_FOREARM_RIGHT, HP_HAND_RIGHT,
            HP_TORSO, HP_PELVIS,
            HP_THIGH_LEFT, HP_CRUS_LEFT,
            HP_FOOT_LEFT, HP_THIGH_RIGHT,
            HP_CRUS_RIGHT, HP_FOOT_RIGHT;
        
        public int AC_NECK,
            AC_HEAD, AC_CHEST,
            AC_ARM_LEFT, AC_FOREARM_LEFT,
            AC_HAND_LEFT, AC_ARM_RIGHT,
            AC_FOREARM_RIGHT, AC_HAND_RIGHT,
            AC_TORSO, AC_PELVIS,
            AC_THIGH_LEFT, AC_CRUS_LEFT,
            AC_FOOT_LEFT, AC_THIGH_RIGHT,
            AC_CRUS_RIGHT, AC_FOOT_RIGHT;
        
        public string CONDITION_NECK,
            CONDITION_HEAD, CONDITION_CHEST,
            CONDITION_ARM_LEFT, CONDITION_FOREARM_LEFT,
            CONDITION_HAND_LEFT, CONDITION_ARM_RIGHT,
            CONDITION_FOREARM_RIGHT, CONDITION_HAND_RIGHT,
            CONDITION_TORSO, CONDITION_PELVIS,
            CONDITION_THIGH_LEFT, CONDITION_CRUS_LEFT,
            CONDITION_FOOT_LEFT, CONDITION_THIGH_RIGHT,
            CONDITION_CRUS_RIGHT, CONDITION_FOOT_RIGHT;

        // public FixedString4096Bytes

        public bool barbarian;
        public int baseSpeed;
        public bool initProf;
    }

    [Serializable]
    public class JsonUserDatas{

        public JsonUserData[] jsonUserDatas;
    }

    [Serializable]
    public class JsonConditions{

        public Dictionary<string, string> conditionsKeyValue = new Dictionary<string, string>();
        public Dictionary<string, string> conditionsValueKey = new Dictionary<string, string>();
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
    public class JsonItems{

        public JsonItem[] items;
    }

    [Serializable]
    public class JsonInventory{

        public string username;
        public JsonItem[] items;
    }

    [Serializable]
    public class JsonInventories{

        public JsonInventory[] inventories;
    }
    
    [Serializable]
    public class JsonItemIdTally{

        public int idTally;
    }

    [Serializable]
    public class JsonItemSlot{

        public item item;
        public SlotModifierType slotModifierType;
        public string bodypart;
    }
    [Serializable]
    public class JsonBodyArray{

        public string username;
        public JsonItemSlot[] itemSlots;
    }
    [Serializable]
    public class JsonBodyArrays{

        public JsonBodyArray[] bodyArrays;
    }


    // void OnApplicationQuit(){

    //     SaveData();
    // }

    public void SaveData(){

        if(!IsHost)
            return;

        SaveIdTallyRpc();
        
        JsonUserDatas jsonUserDatas = JsonConvert.DeserializeObject<JsonUserDatas>(File.ReadAllText($"{Application.persistentDataPath}/userdatas.json"));

        for(int i = 0; i < userDatas.Count; i++){
            
            for(int j = 0; j < jsonUserDatas.jsonUserDatas.Length; j++){
                
                if(userDatas[i].username.ToString() == jsonUserDatas.jsonUserDatas[j].username){

                    jsonUserDatas.jsonUserDatas[j] = new JsonUserData(){

                        id = userDatas[i].id.ToString(),
                        username = userDatas[i].username.ToString(),

                        Str = userDatas[i].Str,
                        Dex = userDatas[i].Dex,
                        Con = userDatas[i].Con,
                        Int = userDatas[i].Int,
                        Wis = userDatas[i].Wis,
                        Cha = userDatas[i].Cha,

                        Lvl = userDatas[i].Lvl,
                        
                        HP_NECK = userDatas[i].HP_NECK,
                        HP_HEAD = userDatas[i].HP_HEAD,
                        HP_CHEST = userDatas[i].HP_CHEST,
                        HP_ARM_LEFT = userDatas[i].HP_ARM_LEFT,
                        HP_FOREARM_LEFT = userDatas[i].HP_FOREARM_LEFT,
                        HP_HAND_LEFT = userDatas[i].HP_HAND_LEFT,
                        HP_ARM_RIGHT = userDatas[i].HP_ARM_RIGHT,
                        HP_FOREARM_RIGHT = userDatas[i].HP_FOREARM_RIGHT,
                        HP_HAND_RIGHT = userDatas[i].HP_HAND_RIGHT,
                        HP_TORSO = userDatas[i].HP_TORSO,
                        HP_PELVIS = userDatas[i].HP_PELVIS,
                        HP_THIGH_LEFT = userDatas[i].HP_THIGH_LEFT,
                        HP_CRUS_LEFT = userDatas[i].HP_CRUS_LEFT,
                        HP_FOOT_LEFT = userDatas[i].HP_FOOT_LEFT,
                        HP_THIGH_RIGHT = userDatas[i].HP_THIGH_RIGHT,
                        HP_CRUS_RIGHT = userDatas[i].HP_CRUS_RIGHT,
                        HP_FOOT_RIGHT = userDatas[i].HP_FOOT_RIGHT,

                        AC_NECK = userDatas[i].AC_NECK,
                        AC_HEAD = userDatas[i].AC_HEAD,
                        AC_CHEST = userDatas[i].AC_CHEST,
                        AC_ARM_LEFT = userDatas[i].AC_ARM_LEFT,
                        AC_FOREARM_LEFT = userDatas[i].AC_FOREARM_LEFT,
                        AC_HAND_LEFT = userDatas[i].AC_HAND_LEFT,
                        AC_ARM_RIGHT = userDatas[i].AC_ARM_RIGHT,
                        AC_FOREARM_RIGHT = userDatas[i].AC_FOREARM_RIGHT,
                        AC_HAND_RIGHT = userDatas[i].AC_HAND_RIGHT,
                        AC_TORSO = userDatas[i].AC_TORSO,
                        AC_PELVIS = userDatas[i].AC_PELVIS,
                        AC_THIGH_LEFT = userDatas[i].AC_THIGH_LEFT,
                        AC_CRUS_LEFT = userDatas[i].AC_CRUS_LEFT,
                        AC_FOOT_LEFT = userDatas[i].AC_FOOT_LEFT,
                        AC_THIGH_RIGHT = userDatas[i].AC_THIGH_RIGHT,
                        AC_CRUS_RIGHT = userDatas[i].AC_CRUS_RIGHT,
                        AC_FOOT_RIGHT = userDatas[i].AC_FOOT_RIGHT,

                        CONDITION_NECK = userDatas[i].CONDITION_NECK.ToString(),
                        CONDITION_HEAD = userDatas[i].CONDITION_HEAD.ToString(),
                        CONDITION_CHEST = userDatas[i].CONDITION_CHEST.ToString(),
                        CONDITION_ARM_LEFT = userDatas[i].CONDITION_ARM_LEFT.ToString(),
                        CONDITION_FOREARM_LEFT = userDatas[i].CONDITION_FOREARM_LEFT.ToString(),
                        CONDITION_HAND_LEFT = userDatas[i].CONDITION_HAND_LEFT.ToString(),
                        CONDITION_ARM_RIGHT = userDatas[i].CONDITION_ARM_RIGHT.ToString(),
                        CONDITION_FOREARM_RIGHT = userDatas[i].CONDITION_FOREARM_RIGHT.ToString(),
                        CONDITION_HAND_RIGHT = userDatas[i].CONDITION_HAND_RIGHT.ToString(),
                        CONDITION_TORSO = userDatas[i].CONDITION_TORSO.ToString(),
                        CONDITION_PELVIS = userDatas[i].CONDITION_PELVIS.ToString(),
                        CONDITION_THIGH_LEFT = userDatas[i].CONDITION_THIGH_LEFT.ToString(),
                        CONDITION_CRUS_LEFT = userDatas[i].CONDITION_CRUS_LEFT.ToString(),
                        CONDITION_FOOT_LEFT = userDatas[i].CONDITION_FOOT_LEFT.ToString(),
                        CONDITION_THIGH_RIGHT = userDatas[i].CONDITION_THIGH_RIGHT.ToString(),
                        CONDITION_CRUS_RIGHT = userDatas[i].CONDITION_CRUS_RIGHT.ToString(),
                        CONDITION_FOOT_RIGHT = userDatas[i].CONDITION_FOOT_RIGHT.ToString(),

                        

                        barbarian = userDatas[i].barbarian,
                        baseSpeed = userDatas[i].baseSpeed,
                        initProf = userDatas[i].initProf

                    };
                }
                else if (j == jsonUserDatas.jsonUserDatas.Length - 1){ // Reached the end, user doesn't exist
                
                    Array.Resize(ref jsonUserDatas.jsonUserDatas, jsonUserDatas.jsonUserDatas.Length + 1);

                    jsonUserDatas.jsonUserDatas[^1] = new JsonUserData{

                        id = userDatas[i].id.ToString(),
                        username = userDatas[i].username.ToString(),

                        Str = userDatas[i].Str,
                        Dex = userDatas[i].Dex,
                        Con = userDatas[i].Con,
                        Int = userDatas[i].Int,
                        Wis = userDatas[i].Wis,
                        Cha = userDatas[i].Cha,

                        Lvl = userDatas[i].Lvl,

                        HP_NECK = userDatas[i].HP_NECK,
                        HP_HEAD = userDatas[i].HP_HEAD,
                        HP_CHEST = userDatas[i].HP_CHEST,
                        HP_ARM_LEFT = userDatas[i].HP_ARM_LEFT,
                        HP_FOREARM_LEFT = userDatas[i].HP_FOREARM_LEFT,
                        HP_HAND_LEFT = userDatas[i].HP_HAND_LEFT,
                        HP_ARM_RIGHT = userDatas[i].HP_ARM_RIGHT,
                        HP_FOREARM_RIGHT = userDatas[i].HP_FOREARM_RIGHT,
                        HP_HAND_RIGHT = userDatas[i].HP_HAND_RIGHT,
                        HP_TORSO = userDatas[i].HP_TORSO,
                        HP_PELVIS = userDatas[i].HP_PELVIS,
                        HP_THIGH_LEFT = userDatas[i].HP_THIGH_LEFT,
                        HP_CRUS_LEFT = userDatas[i].HP_CRUS_LEFT,
                        HP_FOOT_LEFT = userDatas[i].HP_FOOT_LEFT,
                        HP_THIGH_RIGHT = userDatas[i].HP_THIGH_RIGHT,
                        HP_CRUS_RIGHT = userDatas[i].HP_CRUS_RIGHT,
                        HP_FOOT_RIGHT = userDatas[i].HP_FOOT_RIGHT,

                        AC_NECK = userDatas[i].AC_NECK,
                        AC_HEAD = userDatas[i].AC_HEAD,
                        AC_CHEST = userDatas[i].AC_CHEST,
                        AC_ARM_LEFT = userDatas[i].AC_ARM_LEFT,
                        AC_FOREARM_LEFT = userDatas[i].AC_FOREARM_LEFT,
                        AC_HAND_LEFT = userDatas[i].AC_HAND_LEFT,
                        AC_ARM_RIGHT = userDatas[i].AC_ARM_RIGHT,
                        AC_FOREARM_RIGHT = userDatas[i].AC_FOREARM_RIGHT,
                        AC_HAND_RIGHT = userDatas[i].AC_HAND_RIGHT,
                        AC_TORSO = userDatas[i].AC_TORSO,
                        AC_PELVIS = userDatas[i].AC_PELVIS,
                        AC_THIGH_LEFT = userDatas[i].AC_THIGH_LEFT,
                        AC_CRUS_LEFT = userDatas[i].AC_CRUS_LEFT,
                        AC_FOOT_LEFT = userDatas[i].AC_FOOT_LEFT,
                        AC_THIGH_RIGHT = userDatas[i].AC_THIGH_RIGHT,
                        AC_CRUS_RIGHT = userDatas[i].AC_CRUS_RIGHT,
                        AC_FOOT_RIGHT = userDatas[i].AC_FOOT_RIGHT,

                        CONDITION_NECK = userDatas[i].CONDITION_NECK.ToString(),
                        CONDITION_HEAD = userDatas[i].CONDITION_HEAD.ToString(),
                        CONDITION_CHEST = userDatas[i].CONDITION_CHEST.ToString(),
                        CONDITION_ARM_LEFT = userDatas[i].CONDITION_ARM_LEFT.ToString(),
                        CONDITION_FOREARM_LEFT = userDatas[i].CONDITION_FOREARM_LEFT.ToString(),
                        CONDITION_HAND_LEFT = userDatas[i].CONDITION_HAND_LEFT.ToString(),
                        CONDITION_ARM_RIGHT = userDatas[i].CONDITION_ARM_RIGHT.ToString(),
                        CONDITION_FOREARM_RIGHT = userDatas[i].CONDITION_FOREARM_RIGHT.ToString(),
                        CONDITION_HAND_RIGHT = userDatas[i].CONDITION_HAND_RIGHT.ToString(),
                        CONDITION_TORSO = userDatas[i].CONDITION_TORSO.ToString(),
                        CONDITION_PELVIS = userDatas[i].CONDITION_PELVIS.ToString(),
                        CONDITION_THIGH_LEFT = userDatas[i].CONDITION_THIGH_LEFT.ToString(),
                        CONDITION_CRUS_LEFT = userDatas[i].CONDITION_CRUS_LEFT.ToString(),
                        CONDITION_FOOT_LEFT = userDatas[i].CONDITION_FOOT_LEFT.ToString(),
                        CONDITION_THIGH_RIGHT = userDatas[i].CONDITION_THIGH_RIGHT.ToString(),
                        CONDITION_CRUS_RIGHT = userDatas[i].CONDITION_CRUS_RIGHT.ToString(),
                        CONDITION_FOOT_RIGHT = userDatas[i].CONDITION_FOOT_RIGHT.ToString(),

                        barbarian = userDatas[i].barbarian,
                        baseSpeed = userDatas[i].baseSpeed,
                        initProf = userDatas[i].initProf
                    };
                }
            }
        }

        string output = JsonConvert.SerializeObject(jsonUserDatas);

        File.WriteAllText($"{Application.persistentDataPath}/userdatas.json", output);

        output = JsonConvert.SerializeObject(new JsonConditions(){

            conditionsKeyValue = this.conditionsKeyValue,
            conditionsValueKey = this.conditionsValueKey
        });
        File.WriteAllText($"{Application.persistentDataPath}/conditions.json", output);

        JsonItems jsonItems = new JsonItems();
        jsonItems.items = new JsonItem[items.Count];

        for(int i = 0; i < items.Count; i++){

            jsonItems.items[i] = new JsonItem(){

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
        }

        output = JsonConvert.SerializeObject(jsonItems);
        File.WriteAllText($"{Application.persistentDataPath}/items.json", output);

        JsonInventories jsonInventories = JsonConvert.DeserializeObject<JsonInventories>(File.ReadAllText($"{Application.persistentDataPath}/inventories.json"));

        for(int i = 0; i < userDatas.Count; i++){

            for(int k = 0; k < jsonInventories.inventories.Length; k++){

                if(userDatas[i].username.ToString() == jsonInventories.inventories[k].username){

                    jsonInventories.inventories[k] = new JsonInventory();
                    User userI = GameObject.Find(userDatas[i].username.ToString()).GetComponent<User>();
                    jsonInventories.inventories[k].username = userDatas[i].username.ToString();
                    jsonInventories.inventories[k].items = new JsonItem[userI.backpack.inventory.Count];
                    for(int j = 0; j < userI.backpack.inventory.Count; j++){

                        jsonInventories.inventories[k].items[j] = new JsonItem(){

                            name = userI.backpack.inventory[j].name.ToString(),
                            cost = userI.backpack.inventory[j].cost,
                            value = userI.backpack.inventory[j].value,
                            type = userI.backpack.inventory[j].type,
                            size = userI.backpack.inventory[j].size,
                            amount = userI.backpack.inventory[j].amount,
                            weight = userI.backpack.inventory[j].weight,
                            itemInventory = userI.backpack.inventory[j].itemInventory.ToString(),
                            id = userI.backpack.inventory[j].id,
                            equippable = userI.backpack.inventory[j].equippable
                        };
                    }
                    break;
                }
                else if (k == jsonInventories.inventories.Length - 1){ // Reached the end, inventory doesn't exist

                    Array.Resize(ref jsonInventories.inventories, jsonInventories.inventories.Length + 1);

                    User userI = GameObject.Find(userDatas[i].username.ToString())?.GetComponent<User>();

                    if (userI != null){

                        var inventoryList = userI.backpack.inventory; // NetworkList
                        var itemsArray = new JsonItem[inventoryList.Count]; // Create an array for JsonItems

                        for (int j = 0; j < inventoryList.Count; j++){ // Populate itemsArray

                            itemsArray[j] = new JsonItem{

                                name = inventoryList[j].name.ToString(),
                                cost = inventoryList[j].cost,
                                value = inventoryList[j].value,
                                type = inventoryList[j].type,
                                size = inventoryList[j].size,
                                amount = inventoryList[j].amount,
                                weight = inventoryList[j].weight,
                                itemInventory = inventoryList[j].itemInventory.ToString(),
                                id = inventoryList[j].id,
                                equippable = inventoryList[j].equippable
                            };
                        }

                        jsonInventories.inventories[^1] = new JsonInventory{

                            username = userDatas[i].username.ToString(),
                            items = itemsArray
                        };
                    }
                    else
                    {
                        // Handle case where userI is null (e.g., User not found in the scene)
                        jsonInventories.inventories[^1] = new JsonInventory
                        {
                            username = userDatas[i].username.ToString(),
                            items = Array.Empty<JsonItem>() // Empty inventory for this user
                        };
                    }
                }
            }
        }
        output = JsonConvert.SerializeObject(jsonInventories);
        File.WriteAllText($"{Application.persistentDataPath}/inventories.json", output);

        JsonBodyArrays jsonBodyArrays = JsonConvert.DeserializeObject<JsonBodyArrays>(File.ReadAllText($"{Application.persistentDataPath}/bodyarrays.json"));

        for(int i = 0; i < userDatas.Count; i++){

            for(int j = 0; j < jsonBodyArrays.bodyArrays.Length; j++){

                if(userDatas[i].username.ToString() == jsonBodyArrays.bodyArrays[j].username){

                    jsonBodyArrays.bodyArrays[j] = new JsonBodyArray();
                    User userI = GameObject.Find(userDatas[i].username.ToString()).GetComponent<User>();
                    userI.UpdateNetworkedSlotsRpc(userI.name);
                    jsonBodyArrays.bodyArrays[j].username = userDatas[i].username.ToString();
                    jsonBodyArrays.bodyArrays[j].itemSlots = new JsonItemSlot[userI.itemSlots.Count];
                    for(int k = 0; k < userI.itemSlots.Count; k++){

                        jsonBodyArrays.bodyArrays[j].itemSlots[k] = new JsonItemSlot{

                            item = userI.itemSlots[k].item,
                            slotModifierType = userI.itemSlots[k].slotModifierType,
                            bodypart = userI.itemSlots[k].bodypart.ToString()
                        };
                    }
                }
                else if (j == jsonBodyArrays.bodyArrays.Length - 1){ // Reached the end, body array doesn't exist

                    // Resize the array to add a new entry for this user
                    Array.Resize(ref jsonBodyArrays.bodyArrays, jsonBodyArrays.bodyArrays.Length + 1);

                    User userI = GameObject.Find(userDatas[i].username.ToString())?.GetComponent<User>();

                    if (userI != null)
                    {
                        userI.UpdateNetworkedSlotsRpc(userI.name);

                        var itemSlotsList = userI.itemSlots; // List of item slots
                        var itemSlotsArray = new JsonItemSlot[itemSlotsList.Count]; // Create an array for JsonItemSlots

                        for (int k = 0; k < itemSlotsList.Count; k++) // Populate itemSlotsArray
                        {
                            itemSlotsArray[k] = new JsonItemSlot
                            {
                                item = itemSlotsList[k].item,
                                slotModifierType = itemSlotsList[k].slotModifierType,
                                bodypart = itemSlotsList[k].bodypart.ToString()
                            };
                        }

                        // Add the new entry for the user
                        jsonBodyArrays.bodyArrays[^1] = new JsonBodyArray
                        {
                            username = userDatas[i].username.ToString(),
                            itemSlots = itemSlotsArray
                        };
                    }
                    else
                    {
                        // Handle case where userI is null (e.g., User not found in the scene)
                        jsonBodyArrays.bodyArrays[^1] = new JsonBodyArray
                        {
                            username = userDatas[i].username.ToString(),
                            itemSlots = Array.Empty<JsonItemSlot>() // Empty item slots for this user
                        };
                    }
                }
            }
        }
        
        output = JsonConvert.SerializeObject(jsonBodyArrays);
        File.WriteAllText($"{Application.persistentDataPath}/bodyarrays.json", output);

        // Debug.Log($"Writing to directory: {Application.persistentDataPath}");
    }
    bool InitialLoad = true;
    public IEnumerator LoadData(){

        if(!IsHost)
            yield break;

        LoadIdTallyRpc();
        // Debug.Log(itemIdTally.Value);

        string output = File.ReadAllText($"{Application.persistentDataPath}/userdatas.json");
        JsonConditions jsonConditions = JsonConvert.DeserializeObject<JsonConditions>(File.ReadAllText($"{Application.persistentDataPath}/conditions.json"));
        JsonUserDatas jsonUserDatas;

        if(output == "{}"){

            JsonUserDatas newJsonUserDatas = new JsonUserDatas();
            newJsonUserDatas.jsonUserDatas = new JsonUserData[userDatas.Count];

            for(int i = 0; i < userDatas.Count; i++){

                newJsonUserDatas.jsonUserDatas[i] = new JsonUserData(){
                    
                

                    id = userDatas[i].id.ToString(),
                    username = userDatas[i].username.ToString(),

                    Str = 0,
                    Dex = 0,
                    Con = 0,
                    Int = 0,
                    Wis = 0,
                    Cha = 0,

                    Lvl = 1,
                    
                    HP_NECK = 2,
                    HP_HEAD = 3,
                    HP_CHEST = 5,
                    HP_ARM_LEFT = 4,
                    HP_FOREARM_LEFT = 3,
                    HP_HAND_LEFT = 2,
                    HP_ARM_RIGHT = 4,
                    HP_FOREARM_RIGHT = 3,
                    HP_HAND_RIGHT = 2,
                    HP_TORSO = 5,
                    HP_PELVIS = 4,
                    HP_THIGH_LEFT = 4,
                    HP_CRUS_LEFT = 3,
                    HP_FOOT_LEFT = 2,
                    HP_THIGH_RIGHT = 4,
                    HP_CRUS_RIGHT = 3,
                    HP_FOOT_RIGHT = 2,

                    AC_NECK = 12,
                    AC_HEAD = 11,
                    AC_CHEST = 8,
                    AC_ARM_LEFT = 9,
                    AC_FOREARM_LEFT = 11,
                    AC_HAND_LEFT = 12,
                    AC_ARM_RIGHT = 9,
                    AC_FOREARM_RIGHT = 11,
                    AC_HAND_RIGHT = 12,
                    AC_TORSO = 8,
                    AC_PELVIS = 9,
                    AC_THIGH_LEFT = 9,
                    AC_CRUS_LEFT = 11,
                    AC_FOOT_LEFT = 12,
                    AC_THIGH_RIGHT = 9,
                    AC_CRUS_RIGHT = 11,
                    AC_FOOT_RIGHT = 12,

                    CONDITION_NECK = "normal",
                    CONDITION_HEAD = "normal",
                    CONDITION_CHEST = "normal",
                    CONDITION_ARM_LEFT = "normal",
                    CONDITION_FOREARM_LEFT = "normal",
                    CONDITION_HAND_LEFT = "normal",
                    CONDITION_ARM_RIGHT = "normal",
                    CONDITION_FOREARM_RIGHT = "normal",
                    CONDITION_HAND_RIGHT = "normal",
                    CONDITION_TORSO = "normal",
                    CONDITION_PELVIS = "normal",
                    CONDITION_THIGH_LEFT = "normal",
                    CONDITION_CRUS_LEFT = "normal",
                    CONDITION_FOOT_LEFT = "normal",
                    CONDITION_THIGH_RIGHT = "normal",
                    CONDITION_CRUS_RIGHT = "normal",
                    CONDITION_FOOT_RIGHT = "normal",

                    

                    barbarian = false,
                    baseSpeed = 44,
                    initProf = false

                };
            }

            output = JsonConvert.SerializeObject(newJsonUserDatas);

            File.WriteAllText($"{Application.persistentDataPath}/userdatas.json", output);
        }

        jsonUserDatas = JsonConvert.DeserializeObject<JsonUserDatas>(File.ReadAllText($"{Application.persistentDataPath}/userdatas.json"));

        conditionsKeyValue = jsonConditions.conditionsKeyValue;
        conditionsValueKey = jsonConditions.conditionsValueKey;

        if(jsonUserDatas.jsonUserDatas.Length < userDatas.Count){

            JsonUserDatas newJsonUserDatas = new JsonUserDatas();
            newJsonUserDatas.jsonUserDatas = new JsonUserData[userDatas.Count];

            for(int i = 0; i < userDatas.Count; i++){
                
                User userI = GameObject.Find(userDatas[i].username.ToString()).GetComponent<User>();
                try{

                    newJsonUserDatas.jsonUserDatas[i] = jsonUserDatas.jsonUserDatas[i];
                }
                catch (Exception){

                    newJsonUserDatas.jsonUserDatas[i] = new JsonUserData(){
                        
                    

                        id = userDatas[i].id.ToString(),
                        username = userDatas[i].username.ToString(),

                        Str = 0,
                        Dex = 0,
                        Con = 0,
                        Int = 0,
                        Wis = 0,
                        Cha = 0,

                        Lvl = 1,
                        
                        HP_NECK = 2,
                        HP_HEAD = 3,
                        HP_CHEST = 5,
                        HP_ARM_LEFT = 4,
                        HP_FOREARM_LEFT = 3,
                        HP_HAND_LEFT = 2,
                        HP_ARM_RIGHT = 4,
                        HP_FOREARM_RIGHT = 3,
                        HP_HAND_RIGHT = 2,
                        HP_TORSO = 5,
                        HP_PELVIS = 4,
                        HP_THIGH_LEFT = 4,
                        HP_CRUS_LEFT = 3,
                        HP_FOOT_LEFT = 2,
                        HP_THIGH_RIGHT = 4,
                        HP_CRUS_RIGHT = 3,
                        HP_FOOT_RIGHT = 2,

                        AC_NECK = 12,
                        AC_HEAD = 11,
                        AC_CHEST = 8,
                        AC_ARM_LEFT = 9,
                        AC_FOREARM_LEFT = 11,
                        AC_HAND_LEFT = 12,
                        AC_ARM_RIGHT = 9,
                        AC_FOREARM_RIGHT = 11,
                        AC_HAND_RIGHT = 12,
                        AC_TORSO = 8,
                        AC_PELVIS = 9,
                        AC_THIGH_LEFT = 9,
                        AC_CRUS_LEFT = 11,
                        AC_FOOT_LEFT = 12,
                        AC_THIGH_RIGHT = 9,
                        AC_CRUS_RIGHT = 11,
                        AC_FOOT_RIGHT = 12,

                        CONDITION_NECK = "normal",
                        CONDITION_HEAD = "normal",
                        CONDITION_CHEST = "normal",
                        CONDITION_ARM_LEFT = "normal",
                        CONDITION_FOREARM_LEFT = "normal",
                        CONDITION_HAND_LEFT = "normal",
                        CONDITION_ARM_RIGHT = "normal",
                        CONDITION_FOREARM_RIGHT = "normal",
                        CONDITION_HAND_RIGHT = "normal",
                        CONDITION_TORSO = "normal",
                        CONDITION_PELVIS = "normal",
                        CONDITION_THIGH_LEFT = "normal",
                        CONDITION_CRUS_LEFT = "normal",
                        CONDITION_FOOT_LEFT = "normal",
                        CONDITION_THIGH_RIGHT = "normal",
                        CONDITION_CRUS_RIGHT = "normal",
                        CONDITION_FOOT_RIGHT = "normal",

                        

                        barbarian = false,
                        baseSpeed = 44,
                        initProf = false

                    };
                }
            }

            output = JsonConvert.SerializeObject(newJsonUserDatas);

            File.WriteAllText($"{Application.persistentDataPath}/userdatas.json", output);
            jsonUserDatas = JsonConvert.DeserializeObject<JsonUserDatas>(File.ReadAllText($"{Application.persistentDataPath}/userdatas.json"));
        }

        for(int i = 0; i < userDatas.Count; i++){

            foreach(userData data in userDatas){

                if(data.username.ToString() == jsonUserDatas.jsonUserDatas[i].username){

                    userDatas[i] = new userData(){

                        id = userDatas[i].id,
                        username = userDatas[i].username,

                        objRef = userDatas[i].objRef,

                        Str = jsonUserDatas.jsonUserDatas[i].Str,
                        Dex = jsonUserDatas.jsonUserDatas[i].Dex,
                        Con = jsonUserDatas.jsonUserDatas[i].Con,
                        Int = jsonUserDatas.jsonUserDatas[i].Int,
                        Wis = jsonUserDatas.jsonUserDatas[i].Wis,
                        Cha = jsonUserDatas.jsonUserDatas[i].Cha,

                        Lvl = jsonUserDatas.jsonUserDatas[i].Lvl,
                        
                        HP_NECK = jsonUserDatas.jsonUserDatas[i].HP_NECK,
                        HP_HEAD = jsonUserDatas.jsonUserDatas[i].HP_HEAD,
                        HP_CHEST = jsonUserDatas.jsonUserDatas[i].HP_CHEST,
                        HP_ARM_LEFT = jsonUserDatas.jsonUserDatas[i].HP_ARM_LEFT,
                        HP_FOREARM_LEFT = jsonUserDatas.jsonUserDatas[i].HP_FOREARM_LEFT,
                        HP_HAND_LEFT = jsonUserDatas.jsonUserDatas[i].HP_HAND_LEFT,
                        HP_ARM_RIGHT = jsonUserDatas.jsonUserDatas[i].HP_ARM_RIGHT,
                        HP_FOREARM_RIGHT = jsonUserDatas.jsonUserDatas[i].HP_FOREARM_RIGHT,
                        HP_HAND_RIGHT = jsonUserDatas.jsonUserDatas[i].HP_HAND_RIGHT,
                        HP_TORSO = jsonUserDatas.jsonUserDatas[i].HP_TORSO,
                        HP_PELVIS = jsonUserDatas.jsonUserDatas[i].HP_PELVIS,
                        HP_THIGH_LEFT = jsonUserDatas.jsonUserDatas[i].HP_THIGH_LEFT,
                        HP_CRUS_LEFT = jsonUserDatas.jsonUserDatas[i].HP_CRUS_LEFT,
                        HP_FOOT_LEFT = jsonUserDatas.jsonUserDatas[i].HP_FOOT_LEFT,
                        HP_THIGH_RIGHT = jsonUserDatas.jsonUserDatas[i].HP_THIGH_RIGHT,
                        HP_CRUS_RIGHT = jsonUserDatas.jsonUserDatas[i].HP_CRUS_RIGHT,
                        HP_FOOT_RIGHT = jsonUserDatas.jsonUserDatas[i].HP_FOOT_RIGHT,

                        AC_NECK = jsonUserDatas.jsonUserDatas[i].AC_NECK,
                        AC_HEAD = jsonUserDatas.jsonUserDatas[i].AC_HEAD,
                        AC_CHEST = jsonUserDatas.jsonUserDatas[i].AC_CHEST,
                        AC_ARM_LEFT = jsonUserDatas.jsonUserDatas[i].AC_ARM_LEFT,
                        AC_FOREARM_LEFT = jsonUserDatas.jsonUserDatas[i].AC_FOREARM_LEFT,
                        AC_HAND_LEFT = jsonUserDatas.jsonUserDatas[i].AC_HAND_LEFT,
                        AC_ARM_RIGHT = jsonUserDatas.jsonUserDatas[i].AC_ARM_RIGHT,
                        AC_FOREARM_RIGHT = jsonUserDatas.jsonUserDatas[i].AC_FOREARM_RIGHT,
                        AC_HAND_RIGHT = jsonUserDatas.jsonUserDatas[i].AC_HAND_RIGHT,
                        AC_TORSO = jsonUserDatas.jsonUserDatas[i].AC_TORSO,
                        AC_PELVIS = jsonUserDatas.jsonUserDatas[i].AC_PELVIS,
                        AC_THIGH_LEFT = jsonUserDatas.jsonUserDatas[i].AC_THIGH_LEFT,
                        AC_CRUS_LEFT = jsonUserDatas.jsonUserDatas[i].AC_CRUS_LEFT,
                        AC_FOOT_LEFT = jsonUserDatas.jsonUserDatas[i].AC_FOOT_LEFT,
                        AC_THIGH_RIGHT = jsonUserDatas.jsonUserDatas[i].AC_THIGH_RIGHT,
                        AC_CRUS_RIGHT = jsonUserDatas.jsonUserDatas[i].AC_CRUS_RIGHT,
                        AC_FOOT_RIGHT = jsonUserDatas.jsonUserDatas[i].AC_FOOT_RIGHT,

                        CONDITION_NECK = jsonUserDatas.jsonUserDatas[i].CONDITION_NECK,
                        CONDITION_HEAD = jsonUserDatas.jsonUserDatas[i].CONDITION_HEAD,
                        CONDITION_CHEST = jsonUserDatas.jsonUserDatas[i].CONDITION_CHEST,
                        CONDITION_ARM_LEFT = jsonUserDatas.jsonUserDatas[i].CONDITION_ARM_LEFT,
                        CONDITION_FOREARM_LEFT = jsonUserDatas.jsonUserDatas[i].CONDITION_FOREARM_LEFT,
                        CONDITION_HAND_LEFT = jsonUserDatas.jsonUserDatas[i].CONDITION_HAND_LEFT,
                        CONDITION_ARM_RIGHT = jsonUserDatas.jsonUserDatas[i].CONDITION_ARM_RIGHT,
                        CONDITION_FOREARM_RIGHT = jsonUserDatas.jsonUserDatas[i].CONDITION_FOREARM_RIGHT,
                        CONDITION_HAND_RIGHT = jsonUserDatas.jsonUserDatas[i].CONDITION_HAND_RIGHT,
                        CONDITION_TORSO = jsonUserDatas.jsonUserDatas[i].CONDITION_TORSO,
                        CONDITION_PELVIS = jsonUserDatas.jsonUserDatas[i].CONDITION_PELVIS,
                        CONDITION_THIGH_LEFT = jsonUserDatas.jsonUserDatas[i].CONDITION_THIGH_LEFT,
                        CONDITION_CRUS_LEFT = jsonUserDatas.jsonUserDatas[i].CONDITION_CRUS_LEFT,
                        CONDITION_FOOT_LEFT = jsonUserDatas.jsonUserDatas[i].CONDITION_FOOT_LEFT,
                        CONDITION_THIGH_RIGHT = jsonUserDatas.jsonUserDatas[i].CONDITION_THIGH_RIGHT,
                        CONDITION_CRUS_RIGHT = jsonUserDatas.jsonUserDatas[i].CONDITION_CRUS_RIGHT,
                        CONDITION_FOOT_RIGHT = jsonUserDatas.jsonUserDatas[i].CONDITION_FOOT_RIGHT,

                        barbarian = jsonUserDatas.jsonUserDatas[i].barbarian,
                        baseSpeed = jsonUserDatas.jsonUserDatas[i].baseSpeed,
                        initProf = jsonUserDatas.jsonUserDatas[i].initProf
                    };
                    User user = GameObject.Find(data.username.ToString()).GetComponent<User>();
                    user.LoadUserDataRpc(i);
                    // Debug.Log($"{data.username} loading...");
                }
            }
        }

        JsonItems jsonItems = JsonConvert.DeserializeObject<JsonItems>(File.ReadAllText($"{Application.persistentDataPath}/items.json"));
        items.Clear();
        foreach(JsonItem item in jsonItems.items){

            items.Add(new item{
                
                name = item.name,
                cost = item.cost,
                value = item.value,
                type = item.type,
                size = item.size,
                amount = item.amount,
                weight = item.weight,
                itemInventory = item.itemInventory.ToString(),
                id = item.id,
                equippable = item.equippable,
                isEquipped = false
            });
        }
        yield return new WaitUntil(() => userDatas.Count > 0);
        foreach(userData userData in userDatas){

            User userI = GameObject.Find(userData.username.ToString()).GetComponent<User>();
            userI.backpack.ClearRpc(userI.name);
        }

        JsonInventories jsonInventories = JsonConvert.DeserializeObject<JsonInventories>(File.ReadAllText($"{Application.persistentDataPath}/inventories.json"));
        if(jsonInventories.inventories.Count() < userDatas.Count){

            JsonInventories newJsonInventories = new JsonInventories();
            newJsonInventories.inventories = new JsonInventory[userDatas.Count];

            for(int i = 0; i < userDatas.Count; i++){
                
                User userI = GameObject.Find(userDatas[i].username.ToString()).GetComponent<User>();
                try{

                    newJsonInventories.inventories[i] = jsonInventories.inventories[i];
                }
                catch (Exception){

                    newJsonInventories.inventories[i] = new JsonInventory(){
                        
                        username = userDatas[i].username.ToString(),
                        items = new JsonItem[0]
                    };
                }
            }

            output = JsonConvert.SerializeObject(newJsonInventories);

            File.WriteAllText($"{Application.persistentDataPath}/inventories.json", output);
            jsonInventories = JsonConvert.DeserializeObject<JsonInventories>(File.ReadAllText($"{Application.persistentDataPath}/inventories.json"));
        }
        for(int i = 0; i < userDatas.Count; i++){

            foreach(userData data in userDatas){

                if(data.username.ToString() == jsonInventories.inventories[i].username){

                    if(jsonInventories.inventories[i].items.Length != 0){
                        User userI = GameObject.Find(data.username.ToString()).GetComponent<User>();
                        foreach(JsonItem item in jsonInventories.inventories[i].items){

                            LoadInventoryRpc(userI.name, new item{

                                name = item.name,
                                cost = item.cost,
                                value = item.value,
                                type = item.type,
                                size = item.size,
                                amount = item.amount,
                                weight = item.weight,
                                itemInventory = item.itemInventory.ToString(),
                                id = item.id,
                                equippable = item.equippable,
                                isEquipped = false
                            });
                        }
                    }
                }
            }
        }

        SendItemInventoriesRpc();

        JsonBodyArrays jsonBodyArrays = JsonConvert.DeserializeObject<JsonBodyArrays>(File.ReadAllText($"{Application.persistentDataPath}/bodyarrays.json"));
        // Load all bodypart item slots
        // not implemented.
        if(jsonBodyArrays.bodyArrays.Count() < userDatas.Count){

            throw new NotImplementedException();
        }
        for(int i = 0; i < userDatas.Count; i++){

            throw new NotImplementedException();
        }

        if(InitialLoad){

            // yield return new WaitForSeconds(5f);
            InitialLoad = false;
            LoadData();
        }
        else{
            interpreter.user.health.SetInitialValuesRpc();
        }
    }

    bool recieved, recieved0;

    [Rpc(SendTo.Everyone)]
    public void SendItemInventoriesRpc(){

        for(int i = 0; i < userDatas.Count; i++){

            foreach(userData data in userDatas){

                if(data.username.ToString() == interpreter.GetUsername){

                    User userI = GameObject.Find(data.username.ToString()).GetComponent<User>();
                    for(int j = 0; j < userI.backpack.itemDisplays.Count; j++){

                        if(userI.backpack.itemDisplays[j] == null)
                            continue;

                        if(userI.backpack.itemDisplays[j].GetComponent<ItemDisplay>().type == Type.backpack){

                            RequestJsonRpc(data.username.ToString(), "host", $"/{data.username.ToString()} {userI.backpack.itemDisplays[j].GetComponent<ItemDisplay>().nameText.text}{userI.backpack.itemDisplays[j].GetComponent<ItemDisplay>().id} Inventory.json");
                            StartCoroutine(LoadSmallInv(userI, j));
                        }
                    }
                    recieved = false;
                }
            }
        }
    }

    IEnumerator LoadSmallInv(User userI, int index){

        if(userI == null)
            yield break;
        yield return new WaitUntil(() => recieved);
        try{

            userI.backpack.itemDisplays[index].transform.GetChild(0).GetComponentInChildren<InventorySmall>().LoadInventory();
        } catch{
            
        }
    }

    [Rpc(SendTo.Everyone)]
    public void SetItemStatsRpc(string usernameI, string itemName, int itemId, int cost = 0, int value = 0, int amount = 1, int weight = 1){

        if(interpreter.GetUsername != usernameI)
            return;
        
        User userI = GameObject.Find(usernameI).GetComponent<User>();
        for(int itemIndex = 0; itemIndex < userI.backpack.inventory.Count; itemIndex++){

            if(userI.backpack.inventory[itemIndex].name.ToString() == itemName && userI.backpack.inventory[itemIndex].id == itemId){

                userI.backpack.inventory[itemIndex] = new item{

                    name = userI.backpack.inventory[itemIndex].name,
                    cost = cost,
                    value = value,
                    type = userI.backpack.inventory[itemIndex].type,
                    size = userI.backpack.inventory[itemIndex].size,
                    amount = amount,
                    weight = weight,
                    itemInventory = userI.backpack.inventory[itemIndex].itemInventory,
                    id = userI.backpack.inventory[itemIndex].id,
                    equippable = userI.backpack.inventory[itemIndex].equippable,
                    isEquipped = false
                };
                foreach(NetworkObject networkObject in userI.backpack.itemDisplays){

                    if(networkObject == null)
                        continue;
                    ItemDisplay itemDisplay = networkObject.GetComponent<ItemDisplay>();

                    if(itemDisplay.nameText.text == userI.backpack.inventory[itemIndex].name.ToString()){

                        itemDisplay.weightText.text = $"{userI.backpack.inventory[itemIndex].weight} Lbs.";
                    }
                }
            }
        }
    }

    [Rpc(SendTo.Everyone)]
    public void ReorderInventoryRpc(string usernameI, itemShort[] itemsOrdered){

        // Debug.Log($"{usernameI} == {interpreter.GetUsername} ?");
        if(usernameI != interpreter.GetUsername)
            return;
        
        // Debug.Log("true. Continuing.");
        
        User userI = GameObject.Find(usernameI).GetComponent<User>();
            
        JsonInventory newOrderedJsonInventory = new JsonInventory();
        newOrderedJsonInventory.username = usernameI;
        newOrderedJsonInventory.items = new JsonItem[itemsOrdered.Length];

        for(int i = 0; i < itemsOrdered.Length; i++){

            for(int j = 0; j < userI.backpack.inventory.Count; j++){

                if(itemsOrdered[i].Equals(userI.backpack.inventory[j])){

                    newOrderedJsonInventory.items[i] = new JsonItem{

                        name = userI.backpack.inventory[j].name.ToString(),
                        cost = userI.backpack.inventory[j].cost,
                        value = userI.backpack.inventory[j].value,
                        type = userI.backpack.inventory[j].type,
                        size = userI.backpack.inventory[j].size,
                        amount = userI.backpack.inventory[j].amount,
                        weight = userI.backpack.inventory[j].weight,
                        itemInventory = userI.backpack.inventory[j].itemInventory.ToString(),
                        id = userI.backpack.inventory[j].id,
                        equippable = userI.backpack.inventory[j].equippable
                    };
                    break;
                }
            }
        }
        // Debug.Log("requesting json on channel 0");
        RequestJsonRpc(usernameI, "host", $"/inventories.json", 0);
        StartCoroutine(SetInventoryAndSave(newOrderedJsonInventory));

    }

    IEnumerator SetInventoryAndSave(JsonInventory jsonInv2Set){

        // Debug.Log("waiting for channel 0 request to return.");
        yield return new WaitUntil(() => recieved0);

        // Debug.Log("channel 0 request returned!");

        JsonInventories inventories = JsonConvert.DeserializeObject<JsonInventories>(File.ReadAllText($"{Application.persistentDataPath}/inventories.json"));

        for(int i = 0; i < inventories.inventories.Length; i++){

            if(inventories.inventories[i].username == jsonInv2Set.username){

                inventories.inventories[i] = jsonInv2Set;
                break;
            }
        }
        // File.WriteAllText($"{Application.persistentDataPath}/inventories.json", JsonConvert.SerializeObject(inventories));
        // Debug.Log("Calling save json rpc on all users.");
        SaveJsonRpc("/inventories.json", JsonConvert.SerializeObject(inventories));
        recieved0 = false;
    }

    // [Rpc(SendTo.Everyone)]
    // public void SetItemInventoryRpc(string usernameI, string itemName, int itemId, item[] items){

    //     if(interpreter.GetUsername != usernameI)
    //         return;
        
    //     User userI = GameObject.Find(usernameI).GetComponent<User>();
        
    //     foreach(item item in userI.backpack.inventory){

    //         if(item.name.ToString() == itemName && item.id == itemId){

    //             item.SetInventory(items);
    //         }
    //     }
    // }

    [Rpc(SendTo.Everyone)]
    public void SaveJsonRpc(string directory, string output){

        // Debug.Log(directory);
        // Debug.Log(output);
        File.WriteAllText(Application.persistentDataPath + directory, output);
        SendItemInventoriesRpc();
    }

    [Rpc(SendTo.Everyone)]
    public void RequestJsonRpc(string requestingUser, string requestedUser, string requestedJsonDirectory, int channel = -1){

        if(requestingUser == interpreter.GetUsername){

            if(channel == -1)
                recieved = false;
            else if(channel == 0)
                recieved0 = false;
        }
        if(requestedUser == interpreter.GetUsername){

            SendJsonRpc(File.ReadAllText(Application.persistentDataPath + requestedJsonDirectory), requestedJsonDirectory, requestingUser, channel);
        }
        if(requestedUser == "host"){

            if(IsHost){

                SendJsonRpc(File.ReadAllText(Application.persistentDataPath + requestedJsonDirectory), requestedJsonDirectory, requestingUser, channel);
            }
        }

    }
    [Rpc(SendTo.Everyone)]
    public void SendJsonRpc(string input, string directory, string sendTo, int channel = -1){

        if(interpreter.GetUsername == sendTo){

            File.WriteAllText(Application.persistentDataPath + directory, input);
            if(channel == -1)
                recieved = true;
            else if(channel == 0)
                recieved0 = true;
        }
    }

    [Rpc(SendTo.Server)]
    void SaveIdTallyRpc(){

        JsonItemIdTally jsonItemIdTally = new JsonItemIdTally();
        jsonItemIdTally.idTally = itemIdTally.Value;

        string output = JsonConvert.SerializeObject(jsonItemIdTally);
        File.WriteAllText($"{Application.persistentDataPath}/idTally.json", output);
    }
    [Rpc(SendTo.Server)]
    void LoadIdTallyRpc(){

        string input = File.ReadAllText($"{Application.persistentDataPath}/idTally.json");
        JsonItemIdTally jsonItemIdTally = JsonConvert.DeserializeObject<JsonItemIdTally>(input);

        itemIdTally.Value = jsonItemIdTally.idTally;
    }

    [Rpc(SendTo.Everyone)]
    public void LoadInventoryRpc(string usernameI, item item){

        if(usernameI != interpreter.GetUsername)
            return;

        User userI = GameObject.Find(usernameI).GetComponent<User>();

        userI.backpack.AddItemRpc(usernameI, item, false);
    }

    [Rpc(SendTo.Server)]
    public void AddPlayerDataRpc(NetworkObjectReference netRef, FixedString32Bytes username, ulong id){

        userData newUser = new userData{

            id = id,
            username = username,
            objRef = netRef
        };
        userDatas.Add(newUser);
    }

    public void OnEnable(){

        DontDestroyOnLoad(gameObject);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        // items.Dispose();
        // userDatas.Dispose();
    }
}
