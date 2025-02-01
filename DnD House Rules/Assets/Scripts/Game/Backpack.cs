using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class Backpack : NetworkBehaviour
{

    public NetworkList<item> inventory = new NetworkList<item>(null, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public List<NetworkObject> itemDisplays = new List<NetworkObject>();
    [SerializeField] GameObject Panel;

    User user;
    string username = "unknown";

    public NetworkVariable<int> capacity = new NetworkVariable<int>(4, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        user = transform.root.GetComponent<User>();
    }
    IEnumerator Start(){

        yield return new WaitUntil(() => user.isInitialized.Value);
        username = user.name;
    }

    // The armor slots are too fucking stupid to do it themselves.
    public void RunArmorSlotLogic(Camera cam, NetworkObject fake, item thisItem, ArmorSlot armorSlot){

        StartCoroutine(ArmorSlotLogic(cam, fake, thisItem, armorSlot));
    }

    IEnumerator ArmorSlotLogic(Camera cam, NetworkObject fake, item thisItem, ArmorSlot armorSlot){

        // Debug.Log("waiting for mouse button to be released...");
        yield return new WaitUntil(() => Input.GetMouseButtonUp(1));
        // Debug.Log("mouse button released. Raycasting...");
        // raycast from mouse y coordinate and this items x coordinate
        RaycastHit2D hit2D = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if(hit2D.collider != null){

            // if we hit a different item in the inventory, switch that items place in the hierarchy with this one.
            // Debug.Log(hit2D.transform.name);
            if(hit2D.transform.name.Contains("Item Display Box Small") && thisItem.type != Type.backpack){
                
                ItemDisplaySmall itemDisplay = hit2D.transform.GetComponent<ItemDisplaySmall>();
                if(itemDisplay.occupiedInventory.thisItemDisplay.thisItem.CapacityLogic(thisItem)){

                    for(int i = 0; i < thisItem.amount; i++){

                        itemDisplay.occupiedInventory.thisItemDisplay.thisItem.AddInventory(new item{

                            name = thisItem.name,
                            cost = thisItem.cost,
                            value = thisItem.value,
                            type = thisItem.type,
                            size = thisItem.size,
                            amount = 1,
                            weight = thisItem.weight / thisItem.amount,
                            itemInventory = thisItem.itemInventory,
                            id = thisItem.id,
                            equippable = thisItem.equippable,
                            isEquipped = false
                        }, itemDisplay.transform.GetSiblingIndex());
                    }
                    List<itemShort> itemShorts = new List<itemShort>();
                    foreach(ItemDisplay itemDisplay1 in itemDisplay.occupiedInventory.thisItemDisplay.occupiedInventory.transform.GetChild(1).GetChild(0).GetComponentsInChildren<ItemDisplay>()){

                        itemShorts.Add(new itemShort{

                            name = itemDisplay1.nameText.text,
                            id = itemDisplay1.id
                        });
                    }
                    GameManager.Singleton.ReorderInventoryRpc(GameManager.Singleton.interpreter.GetUsername, itemShorts.ToArray());
                    // Debug.Log($"Reorder inventory rpc called.");
                    // GameManager.Singleton.SaveData();
                    yield return new WaitForEndOfFrame();
                    switch(armorSlot.description.bodypart.slot[armorSlot.index].slotModifierType){

                        case SlotModifierType.ac:
                            armorSlot.description.bodypart.ac.Value -= armorSlot.description.bodypart.slot[armorSlot.index].item.value;
                            break;
                        case SlotModifierType.hp:
                            armorSlot.description.bodypart.maximumHP.Value -= armorSlot.description.bodypart.slot[armorSlot.index].item.value;
                            break;
                    }
                    armorSlot.description.bodypart.EmptySlot(armorSlot.index);
                    // Destroy(gameObject);
                }
            }
            else if(hit2D.transform.name.Contains("Item Display Box")){

                ItemDisplay itemDisplay = hit2D.transform.GetComponent<ItemDisplay>();
                
                if(itemDisplay.occupiedInventory.CapacityLogic(thisItem, $"{thisItem.value}:{thisItem.type}")){

                    // itemDisplay.occupiedInventory.RefreshItemDisplayBoxRpc(transform.root.name);
                    int siblingIndex = itemDisplay.transform.GetSiblingIndex();
                    itemDisplay.occupiedInventory.AddItemRpc(GameManager.Singleton.interpreter.GetUsername, thisItem, false, siblingIndex);
                    // GameManager.Singleton.SaveData();
                    List<itemShort> itemShorts = new List<itemShort>();
                    foreach(ItemDisplay itemDisplay1 in itemDisplay.occupiedInventory.transform.GetChild(1).GetChild(0).GetComponentsInChildren<ItemDisplay>()){

                        itemShorts.Add(new itemShort{

                            name = itemDisplay1.nameText.text,
                            id = itemDisplay1.id
                        });
                    }
                    GameManager.Singleton.ReorderInventoryRpc(GameManager.Singleton.interpreter.GetUsername, itemShorts.ToArray());
                    // Debug.Log($"Reorder inventory rpc called.");
                    yield return new WaitForEndOfFrame();
                    switch(armorSlot.description.bodypart.slot[armorSlot.index].slotModifierType){

                        case SlotModifierType.ac:
                            armorSlot.description.bodypart.ac.Value -= armorSlot.description.bodypart.slot[armorSlot.index].item.value;
                            break;
                        case SlotModifierType.hp:
                            armorSlot.description.bodypart.maximumHP.Value -= armorSlot.description.bodypart.slot[armorSlot.index].item.value;
                            break;
                    }
                    armorSlot.description.bodypart.EmptySlot(armorSlot.index);
                    // itemDisplay.occupiedInventory.RefreshItemDisplayBoxRpc(transform.root.name);
                    // Destroy(gameObject);
                }
            }
            else if(hit2D.transform.name.Contains("Slot")){

                if(thisItem.equippable){

                    ArmorSlot otherArmorSlot = hit2D.transform.GetComponent<ArmorSlot>();
                    if(otherArmorSlot.description.bodypart.slot[otherArmorSlot.index].Empty()){

                        otherArmorSlot.description.bodypart.slot[otherArmorSlot.index].item = new item{

                            name = thisItem.name,
                            cost = thisItem.cost,
                            value = thisItem.value,
                            type = thisItem.type,
                            size = thisItem.size,
                            amount = 1,
                            weight = thisItem.weight / thisItem.amount,
                            itemInventory = thisItem.itemInventory,
                            id = thisItem.id,
                            equippable = thisItem.equippable,
                            isEquipped = true
                        };
                        switch(otherArmorSlot.description.bodypart.slot[otherArmorSlot.index].item.type){

                            case Type.heavyArmor:
                                otherArmorSlot.description.bodypart.slot[otherArmorSlot.index].slotModifierType = SlotModifierType.ac;
                                break;
                            case Type.lightArmor:
                                otherArmorSlot.description.bodypart.slot[otherArmorSlot.index].slotModifierType = SlotModifierType.ac;
                                break;
                            case Type.capacityMult:
                                otherArmorSlot.description.bodypart.slot[otherArmorSlot.index].slotModifierType = SlotModifierType.storage;
                                break;
                            case Type.capacityMultL:
                                otherArmorSlot.description.bodypart.slot[otherArmorSlot.index].slotModifierType = SlotModifierType.storage;
                                break;
                            case Type.capacityMultS:
                                otherArmorSlot.description.bodypart.slot[otherArmorSlot.index].slotModifierType = SlotModifierType.storage;
                                break;
                            case Type.capacityMultT:
                                otherArmorSlot.description.bodypart.slot[otherArmorSlot.index].slotModifierType = SlotModifierType.storage;
                                break;
                            case Type.medical:
                                otherArmorSlot.description.bodypart.slot[otherArmorSlot.index].slotModifierType = SlotModifierType.hp;
                                break;
                            default:
                                otherArmorSlot.description.bodypart.slot[otherArmorSlot.index].slotModifierType = SlotModifierType.none;
                                break;
                        }
                        armorSlot.description.bodypart.EmptySlot(armorSlot.index);
                    }
                    else{

                        // swap the item in the armor slot with this item.
                        // throw new NotImplementedException();
                        item otherItem = otherArmorSlot.description.bodypart.slot[otherArmorSlot.index].item;
                        otherArmorSlot.description.bodypart.slot[otherArmorSlot.index].item = new item{

                            name = thisItem.name,
                            cost = thisItem.cost,
                            value = thisItem.value,
                            type = thisItem.type,
                            size = thisItem.size,
                            amount = 1,
                            weight = thisItem.weight / thisItem.amount,
                            itemInventory = thisItem.itemInventory,
                            id = thisItem.id,
                            equippable = thisItem.equippable,
                            isEquipped = true
                        };
                        switch(otherArmorSlot.description.bodypart.slot[otherArmorSlot.index].item.type){

                            case Type.heavyArmor:
                                otherArmorSlot.description.bodypart.slot[otherArmorSlot.index].slotModifierType = SlotModifierType.ac;
                                break;
                            case Type.lightArmor:
                                otherArmorSlot.description.bodypart.slot[otherArmorSlot.index].slotModifierType = SlotModifierType.ac;
                                break;
                            case Type.capacityMult:
                                otherArmorSlot.description.bodypart.slot[otherArmorSlot.index].slotModifierType = SlotModifierType.storage;
                                break;
                            case Type.capacityMultL:
                                otherArmorSlot.description.bodypart.slot[otherArmorSlot.index].slotModifierType = SlotModifierType.storage;
                                break;
                            case Type.capacityMultS:
                                otherArmorSlot.description.bodypart.slot[otherArmorSlot.index].slotModifierType = SlotModifierType.storage;
                                break;
                            case Type.capacityMultT:
                                otherArmorSlot.description.bodypart.slot[otherArmorSlot.index].slotModifierType = SlotModifierType.storage;
                                break;
                            case Type.medical:
                                otherArmorSlot.description.bodypart.slot[otherArmorSlot.index].slotModifierType = SlotModifierType.hp;
                                break;
                            default:
                                otherArmorSlot.description.bodypart.slot[otherArmorSlot.index].slotModifierType = SlotModifierType.none;
                                break;
                        }

                        armorSlot.description.bodypart.slot[armorSlot.index].item = new item{

                            name = otherItem.name,
                            cost = otherItem.cost,
                            value = otherItem.value,
                            type = otherItem.type,
                            size = otherItem.size,
                            amount = 1,
                            weight = otherItem.weight / otherItem.amount,
                            itemInventory = otherItem.itemInventory,
                            id = otherItem.id,
                            equippable = otherItem.equippable,
                            isEquipped = true
                        };
                        switch(otherItem.type){

                            case Type.heavyArmor:
                                armorSlot.description.bodypart.slot[armorSlot.index].slotModifierType = SlotModifierType.ac;
                                break;
                            case Type.lightArmor:
                                armorSlot.description.bodypart.slot[armorSlot.index].slotModifierType = SlotModifierType.ac;
                                break;
                            case Type.capacityMult:
                                armorSlot.description.bodypart.slot[armorSlot.index].slotModifierType = SlotModifierType.storage;
                                break;
                            case Type.capacityMultL:
                                armorSlot.description.bodypart.slot[armorSlot.index].slotModifierType = SlotModifierType.storage;
                                break;
                            case Type.capacityMultS:
                                armorSlot.description.bodypart.slot[armorSlot.index].slotModifierType = SlotModifierType.storage;
                                break;
                            case Type.capacityMultT:
                                armorSlot.description.bodypart.slot[armorSlot.index].slotModifierType = SlotModifierType.storage;
                                break;
                            case Type.medical:
                                armorSlot.description.bodypart.slot[armorSlot.index].slotModifierType = SlotModifierType.hp;
                                break;
                            default:
                                armorSlot.description.bodypart.slot[armorSlot.index].slotModifierType = SlotModifierType.none;
                                break;
                        }
                        
                    }
                }
            }
        }
        else{
            // Debug.Log("Raycast did not hit any colliders.");
        }
        // Debug.Log("Destroying fake.");
        Destroy(fake.gameObject);
        GameManager.Singleton.SaveData();
        armorSlot.transform.GetChild(0).gameObject.SetActive(true);
    }

    [Rpc(SendTo.Everyone)]
    public void AddItemRpc(string usernameI, item item, bool updateTally = true, int siblingIndex = -1){

        if(usernameI != username)
            return;
        if(!IsOwner)
            return;
        
        // Debug.Log("owner client check passed.");
        // Debug.Log("entering for loop vvv");

        for(int i = 0; i < inventory.Count; i++){

            // Debug.Log($"checking index {i}...");
            // Debug.Log($"{item.name.ToString()} == {inventory[i].name.ToString()} && {item.size} != {Size.L}?");
            if(item.name.ToString() == inventory[i].name.ToString() && item.size != Size.L && !(item.size == Size.S && item.amount + inventory[i].amount > 2) && !(item.size == Size.T && item.amount + inventory[i].amount > 4) && item.type != Type.backpack){
                // Debug.Log($"true");
                
                if(CapacityLogic(inventory[i])){

                    inventory[i] = new item(){

                        name = inventory[i].name,
                        cost = inventory[i].cost,
                        value = inventory[i].value,
                        type = inventory[i].type,
                        size = inventory[i].size,
                        amount = inventory[i].amount + 1,
                        weight = inventory[i].weight + (inventory[i].weight / inventory[i].amount),
                        itemInventory = inventory[i].itemInventory,
                        id = inventory[i].id,
                        equippable = inventory[i].equippable,
                        isEquipped = false
                    };
                    itemDisplays[i].GetComponent<ItemDisplay>().sizeText.text = $"{inventory[i].amount}{inventory[i].size}";
                    itemDisplays[i].GetComponent<ItemDisplay>().weightText.text = $"{inventory[i].weight} Lbs.";
                }
                if(updateTally)
                    GameManager.Singleton.UpdateIdTallyRpc();
                return;
            }
            else{

                // Debug.Log("false");
            }
        }
        // Debug.Log("exiting for loop ^^^");
        // Debug.Log("entering capacity logic vvv");
        if(CapacityLogic(item)){
            // Debug.Log("true");
            // Debug.Log("adding item to inventory...");
            if(siblingIndex == -1)
                siblingIndex = inventory.Count;

            // Debug.Log(siblingIndex);

            inventory.Insert(siblingIndex, new item{

                name = item.name,
                cost = item.cost,
                value = item.value,
                type = item.type,
                size = item.size,
                amount = item.amount,
                weight = item.weight,
                itemInventory = item.itemInventory,
                id = item.id,
                equippable = item.equippable,
                isEquipped = false
            });
            RefreshItemDisplayBoxRpc(username);
            if(updateTally)
                GameManager.Singleton.UpdateIdTallyRpc();
        }
    }

    public bool CapacityLogic(item item, string subtract = null){

        int capacityL = capacity.Value, capacityS = capacity.Value * 2, capacityT = capacity.Value * 4;
        
        for(int i = 0; i < user.itemSlots.Count; i++){

            item inventoryItem = user.itemSlots[i].item;

            if(inventoryItem.id == -1)
                continue;

            switch(inventoryItem.type){

                case Type.capacityMult:
                    capacityL += inventoryItem.value;
                    capacityS += inventoryItem.value*2;
                    capacityT += inventoryItem.value*4;
                    break;
                case Type.capacityMultL:
                    capacityL += inventoryItem.value;
                    break;
                case Type.capacityMultS:
                    capacityS += inventoryItem.value;
                    break;
                case Type.capacityMultT:
                    capacityT += inventoryItem.value;
                    break;
            }
        }
        if(subtract != null){

            string[] args = subtract.Split(":");

            switch(args[1]){

                case "capacityMult":
                    capacityL -= int.Parse(args[0]);
                    capacityS -= int.Parse(args[0])*2;
                    capacityT -= int.Parse(args[0])*4;
                    break;
                case "capacityMultL":
                    capacityL -= int.Parse(args[0]);
                    break;
                case "capacityMultS":
                    capacityS -= int.Parse(args[0]);
                    break;
                case "capacityMultT":
                    capacityT -= int.Parse(args[0]);
                    break;
            }
        }
        // Debug.Log($"setting large item capacity: {capacityL}");
        // Debug.Log($"setting small item capacity: {capacityS}");
        // Debug.Log($"setting tiny item capacity: {capacityT}");

        switch(item.size){

            case Size.L:
                // Debug.Log($"inventory.Count ({inventory.Count}) == 0?");
                if(inventory.Count == 0)
                    return true;
                float CountL = 0;
                foreach(item inventoryItem in inventory){

                    CountL += inventoryItem.size == Size.S ? inventoryItem.amount*0.5f : inventoryItem.size == Size.T ? inventoryItem.amount*0.25f : inventoryItem.amount;
                }
                // Debug.Log($"total count of all large items in inventory: {CountL} < large item capacity ({capacityL})?");
                if(Mathf.Ceil(CountL + item.amount) <= capacityL)
                    return true;
                break;
            case Size.S:
                // Debug.Log($"inventory.Count ({inventory.Count}) == 0?");
                if(inventory.Count == 0)
                    return true;
                float CountS = 0;
                foreach(item inventoryItem in inventory){
                    
                    CountS += inventoryItem.size == Size.L ? inventoryItem.amount*2 : inventoryItem.size == Size.T ? inventoryItem.amount*0.5f : inventoryItem.amount;
                }
                // Debug.Log($"total count of all small items in inventory: {CountS} < small item capacity ({capacityS})?");
                if(Mathf.Ceil(CountS + item.amount) <= capacityS)
                    return true;
                break;
            case Size.T:
                // Debug.Log($"inventory.Count ({inventory.Count}) == 0?");
                if(inventory.Count == 0)
                    return true;
                float CountT = 0;
                foreach(item inventoryItem in inventory){

                    CountT += inventoryItem.size == Size.L ? inventoryItem.amount*4 : inventoryItem.size == Size.S ? inventoryItem.amount*2 : inventoryItem.amount;
                }
                // Debug.Log($"total count of all tiny items in inventory: {CountT} < tiny item capacity ({capacityT})?");
                if(Mathf.Ceil(CountT + item.amount) <= capacityT)
                    return true;
                break;
            default:
                return false;
        }

        return false;
    }

    [Rpc(SendTo.Everyone)]
    public void RefreshItemDisplayBoxRpc(string usernameI){

        if(username != usernameI)
            return;
        // Debug.Log($"Refreshing display box for {usernameI}.");

        GameObject userObject = GameObject.Find(usernameI);
        if (userObject == null)
        {
            // Debug.LogError($"No GameObject found with the name '{usernameI}'. returning.");
            return;
        }

        User user = userObject.GetComponent<User>();
        if (user == null)
        {
            // Debug.LogError($"The GameObject '{usernameI}' does not have a User component. returning.");
            return;
        }
        if(itemDisplays.Count > 0){

            foreach(NetworkObject itemDis in itemDisplays){

                if(itemDis != null)
                    Destroy(itemDis.gameObject);
            }
            itemDisplays.Clear();
        }
        foreach(item item in inventory){

            NetworkObject itemDisplayBox = Instantiate(GameManager.Singleton.itemDisplayObject, Panel.transform);
            ItemDisplay itemDisplay = itemDisplayBox.GetComponent<ItemDisplay>();
            itemDisplay.nameText.text = item.name.ToString();
            itemDisplay.sizeText.text = $"{item.amount}{item.size}";
            itemDisplay.weightText.text = $"{item.weight} Lbs.";
            itemDisplay.type = item.type;
            itemDisplay.id = item.id;
            itemDisplay.thisItem = item;
            itemDisplay.occupiedInventory = this;
            itemDisplays.Add(itemDisplayBox);
        }
    }

    [Rpc(SendTo.Everyone)]
    public void RemoveItemRpc(string usernameI, string itemName, bool checkId = false, int itemId = 0){

        if(usernameI != username)
            return;
        if(!IsOwner)
            return;

        for(int i = 0; i < inventory.Count; i++){

            if(itemName == inventory[i].name.ToString() && (checkId ? itemId == inventory[i].id : true)){

                if(inventory[i].amount > 1){

                    inventory[i] = new item(){

                        name = inventory[i].name,
                        cost = inventory[i].cost,
                        value = inventory[i].value,
                        type = inventory[i].type,
                        size = inventory[i].size,
                        amount = inventory[i].amount - 1,
                        weight = inventory[i].weight - (inventory[i].weight / inventory[i].amount),
                        itemInventory = inventory[i].itemInventory,
                        id = inventory[i].id,
                        equippable = inventory[i].equippable,
                        isEquipped = false
                    };
                    itemDisplays[i].GetComponent<ItemDisplay>().sizeText.text = $"{inventory[i].amount}{inventory[i].size}";
                    itemDisplays[i].GetComponent<ItemDisplay>().weightText.text = $"{inventory[i].weight} Lbs.";
                }
                else{

                    inventory.Remove(inventory[i]);
                    RefreshItemDisplayBoxRpc(username);
                }
                break;
            }
        }
    }

    [Rpc(SendTo.Everyone)]
    public void ClearRpc(string usernameI){

        if(usernameI != username)
            return;
        if(!IsOwner)
            return;

        inventory.Clear();
        RefreshItemDisplayBoxRpc(usernameI);
    }

    // public override void OnDestroy()
    // {
    //     // inventory.Dispose(); // Dispose to release unmanaged memory
    //     base.OnDestroy();
    // }
    public override void OnNetworkDespawn()
    {
        inventory.Dispose(); // Ensure disposal when network despawns
        base.OnNetworkDespawn();
    }
}
