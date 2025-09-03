using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Backpack : NetworkBehaviour
{

    public NetworkList<item> inventory = new NetworkList<item>(null, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public List<NetworkObject> itemDisplays = new List<NetworkObject>();
    [SerializeField] GameObject Panel;

    public NetworkVariable<int> 
        soljik = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner), 
        brine = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner), 
        penc = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    User user;
    string username = "unknown";

    Camera cam;

    public NetworkVariable<int> capacity = new NetworkVariable<int>(4, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public TMP_Text soljikText, brineText, pencText;

    public TMP_Text iPanel_itemName, iPanel_itemInfo;

    public Description description;

    bool mouseDownCoOn = false;

    IEnumerator Inst = null;
    ArmorSlot slotPriv = null;
    ItemListBox itemPriv = null;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        user = transform.root.GetComponent<User>();
    }
    IEnumerator Start(){

        yield return new WaitUntil(() => user.isInitialized.Value);
        if(!user.IsOwner && !IsHost)
            Destroy(gameObject);
        username = user.name;
        cam = user.transform.GetChild(0).GetComponent<Camera>();
    }

    // The armor slots are too fucking stupid to do it themselves.
    public void RunArmorSlotLogic(item thisItem, int type, ArmorSlot armorSlot, NetworkObject fake = null){


        switch(type){

            case 0:
                slotPriv = armorSlot;
                Inst = ArmorSlotLogic(fake, thisItem, armorSlot);
                StartCoroutine(Inst);
                break;
            case 1:
                StartCoroutine(ArmorSlotLogic(thisItem, armorSlot));
                break;

        }
    }

    IEnumerator ArmorSlotLogic(NetworkObject fake, item thisItem, ArmorSlot armorSlot){

        // Debug.Log("waiting for mouse button to be released...");
        mouseDownCoOn = true;
        yield return new WaitUntil(() => Input.GetMouseButtonUp(1));
        mouseDownCoOn = false;
        // Debug.Log("mouse button released. Raycasting...");
        // raycast from mouse y coordinate and this items x coordinate
        RaycastHit2D hit2D = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if(hit2D.collider != null){

            // if we hit a different item in the inventory, switch that items place in the hierarchy with this one.
            // Debug.Log(hit2D.transform.name);
            if(Input.GetKey(KeyCode.LeftShift) && hit2D.transform.name.Contains("Item Display Box Small") && thisItem.type != Type.backpack){
                
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
                            isEquipped = false,
                            bodyparts = thisItem.bodyparts,
                            metadata = thisItem.metadata
                        }, itemDisplay.transform.GetSiblingIndex());
                    }
                    // List<itemShort> itemShorts = new List<itemShort>();
                    // foreach(ItemDisplay itemDisplay1 in itemDisplay.occupiedInventory.thisItemDisplay.occupiedInventory.transform.GetChild(4).GetChild(0).GetComponentsInChildren<ItemDisplay>()){

                    //     itemShorts.Add(new itemShort{

                    //         name = itemDisplay1.nameText.text,
                    //         id = itemDisplay1.id
                    //     });
                    // }
                    // GameManager.Singleton.ReorderInventoryRpc(GameManager.Singleton.interpreter.GetUsername, itemShorts.ToArray());
                    // Debug.Log($"Reorder inventory rpc called.");
                    // GameManager.Singleton.SaveDataRpc();
                    
                    yield return new WaitForEndOfFrame();
                    GameManager.Singleton.RequestJsonRpc(GameManager.Singleton.interpreter.GetUsername, "host", $"/{GameManager.Singleton.interpreter.GetUsername} {itemDisplay.occupiedInventory.thisItemDisplay.nameText.text}{itemDisplay.occupiedInventory.thisItemDisplay.id} Inventory.json");
                    switch(armorSlot.description.bodypart.slot[armorSlot.index].slotModifierType){

                        case SlotModifierType.ac:
                            armorSlot.description.bodypart.ac.Value -= armorSlot.description.bodypart.slot[armorSlot.index].item.value;
                            break;
                        case SlotModifierType.hp:
                            armorSlot.description.bodypart.maximumHP.Value -= armorSlot.description.bodypart.slot[armorSlot.index].item.value;
                            break;
                    }
                    transform.root.GetComponent<User>().UpdateNetworkedSlotsRpc(GameManager.Singleton.interpreter.GetUsername);
                    armorSlot.description.bodypart.EmptySlot(armorSlot.index);
                    yield return new WaitForEndOfFrame();
                    itemDisplay.occupiedInventory.LoadInventory();
                    // Destroy(gameObject);
                }
            }
            else if(Input.GetKey(KeyCode.LeftShift) && hit2D.transform.name.Contains("Item Display Box")){

                ItemDisplay itemDisplay = hit2D.transform.GetComponent<ItemDisplay>();
                
                if(itemDisplay.occupiedInventory.CapacityLogic(thisItem, $"{thisItem.value}:{thisItem.type}")){

                    // itemDisplay.RefreshItemDisplayBoxRpc(transform.root.name);
                    int siblingIndex = itemDisplay.transform.GetSiblingIndex();
                    itemDisplay.occupiedInventory.AddItemRpc(GameManager.Singleton.interpreter.GetUsername, thisItem, false, siblingIndex);
                    // GameManager.Singleton.SaveDataRpc();
                    // List<itemShort> itemShorts = new List<itemShort>();
                    // foreach(ItemDisplay itemDisplay1 in itemDisplay.transform.GetChild(4).GetChild(0).GetComponentsInChildren<ItemDisplay>()){

                    //     itemShorts.Add(new itemShort{

                    //         name = itemDisplay1.nameText.text,
                    //         id = itemDisplay1.id
                    //     });
                    // }
                    // GameManager.Singleton.ReorderInventoryRpc(GameManager.Singleton.interpreter.GetUsername, itemShorts.ToArray());
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
                    transform.root.GetComponent<User>().UpdateNetworkedSlotsRpc(GameManager.Singleton.interpreter.GetUsername);
                    armorSlot.description.bodypart.EmptySlot(armorSlot.index);
                    // itemDisplay.RefreshItemDisplayBoxRpc(transform.root.name);
                    // Destroy(gameObject);
                }
            }
            else if(Input.GetKey(KeyCode.LeftShift) && hit2D.transform.name.Contains("Slot")){

                foreach(string bodypartName in thisItem.GetBodyparts()){

                    if(bodypartName == armorSlot.description.bodypart.name){

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
                                    isEquipped = true,
                                    bodyparts = thisItem.bodyparts,
                                    metadata = thisItem.metadata
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
                                    case Type.healthMult:
                                        otherArmorSlot.description.bodypart.slot[otherArmorSlot.index].slotModifierType = SlotModifierType.hp;
                                        break;
                                    default:
                                        otherArmorSlot.description.bodypart.slot[otherArmorSlot.index].slotModifierType = SlotModifierType.none;
                                        break;
                                }
                                armorSlot.description.bodypart.EmptySlot(armorSlot.index);
                                yield return new WaitForEndOfFrame();
                                armorSlot.transform.GetChild(0).gameObject.SetActive(true);
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
                                    isEquipped = true,
                                    bodyparts = thisItem.bodyparts,
                                    metadata = thisItem.metadata
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
                                    case Type.healthMult:
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
                                    isEquipped = true,
                                    bodyparts = otherItem.bodyparts,
                                    metadata = otherItem.metadata
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
                                    case Type.healthMult:
                                        armorSlot.description.bodypart.slot[armorSlot.index].slotModifierType = SlotModifierType.hp;
                                        break;
                                    default:
                                        armorSlot.description.bodypart.slot[armorSlot.index].slotModifierType = SlotModifierType.none;
                                        break;
                                }
                                yield return new WaitForEndOfFrame();
                                armorSlot.transform.GetChild(0).gameObject.SetActive(true);
                            }
                        }
                        break;
                    }
                }
            }
            else if(Input.GetKey(KeyCode.LeftShift) && hit2D.transform.name.Contains("Scroll")){

                if(CapacityLogic(thisItem, $"{thisItem.value}:{thisItem.type}")){

                    AddItemRpc(GameManager.Singleton.interpreter.GetUsername, thisItem, false);
                    // List<itemShort> itemShorts = new List<itemShort>();
                    // foreach(ItemDisplay itemDisplay1 in transform.GetChild(4).GetChild(0).GetComponentsInChildren<ItemDisplay>()){

                    //     itemShorts.Add(new itemShort{

                    //         name = itemDisplay1.nameText.text,
                    //         id = itemDisplay1.id
                    //     });
                    // }
                    // GameManager.Singleton.ReorderInventoryRpc(GameManager.Singleton.interpreter.GetUsername, itemShorts.ToArray());

                    yield return new WaitForEndOfFrame();
                    switch(armorSlot.description.bodypart.slot[armorSlot.index].slotModifierType){

                        case SlotModifierType.ac:
                            armorSlot.description.bodypart.ac.Value -= armorSlot.description.bodypart.slot[armorSlot.index].item.value;
                            break;
                        case SlotModifierType.hp:
                            armorSlot.description.bodypart.maximumHP.Value -= armorSlot.description.bodypart.slot[armorSlot.index].item.value;
                            break;
                    }
                    transform.root.GetComponent<User>().UpdateNetworkedSlotsRpc(GameManager.Singleton.interpreter.GetUsername);
                    armorSlot.description.bodypart.EmptySlot(armorSlot.index);
                }
            }
            else if(Input.GetKey(KeyCode.LeftShift) && hit2D.transform.name.Contains("Inventory") && thisItem.type != Type.backpack){

                InventorySmall inventorySmall = hit2D.transform.GetComponent<InventorySmall>();
                if(inventorySmall.thisItemDisplay.thisItem.CapacityLogic(thisItem)){

                    for(int i = 0; i < thisItem.amount; i++){

                        inventorySmall.thisItemDisplay.thisItem.AddInventory(new item{

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
                            isEquipped = false,
                            bodyparts = thisItem.bodyparts,
                            metadata = thisItem.metadata
                        });
                    }
                    // List<itemShort> itemShorts = new List<itemShort>();
                    // foreach(ItemDisplay itemDisplay1 in itemDisplay.occupiedInventory.thisItemDisplay.occupiedInventory.transform.GetChild(4).GetChild(0).GetComponentsInChildren<ItemDisplay>()){

                    //     itemShorts.Add(new itemShort{

                    //         name = itemDisplay1.nameText.text,
                    //         id = itemDisplay1.id
                    //     });
                    // }
                    // GameManager.Singleton.ReorderInventoryRpc(GameManager.Singleton.interpreter.GetUsername, itemShorts.ToArray());
                    // Debug.Log($"Reorder inventory rpc called.");
                    // GameManager.Singleton.SaveDataRpc();
                    
                    yield return new WaitForEndOfFrame();
                    GameManager.Singleton.RequestJsonRpc(GameManager.Singleton.interpreter.GetUsername, "host", $"/{GameManager.Singleton.interpreter.GetUsername} {inventorySmall.thisItemDisplay.nameText.text}{inventorySmall.thisItemDisplay.id} Inventory.json");
                    switch(armorSlot.description.bodypart.slot[armorSlot.index].slotModifierType){

                        case SlotModifierType.ac:
                            armorSlot.description.bodypart.ac.Value -= armorSlot.description.bodypart.slot[armorSlot.index].item.value;
                            break;
                        case SlotModifierType.hp:
                            armorSlot.description.bodypart.maximumHP.Value -= armorSlot.description.bodypart.slot[armorSlot.index].item.value;
                            break;
                    }
                    transform.root.GetComponent<User>().UpdateNetworkedSlotsRpc(GameManager.Singleton.interpreter.GetUsername);
                    armorSlot.description.bodypart.EmptySlot(armorSlot.index);
                    yield return new WaitForEndOfFrame();
                    inventorySmall.LoadInventory();
                    // Destroy(gameObject);
                }
            }
        }
        else{
            // Debug.Log("Raycast did not hit any colliders.");
        }
        // Debug.Log("Destroying fake.");
        if(fake == null)
            yield break;
        Destroy(fake.gameObject);
        transform.root.GetComponent<User>().UpdateNetworkedSlotsRpc(GameManager.Singleton.interpreter.GetUsername);
        yield return new WaitForEndOfFrame();
        armorSlot.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void RunItemListLogic(item thisItem, int type, ItemListBox itemListBox, NetworkObject fake = null){

        switch(type){

            case 0:
                itemPriv = itemListBox;
                Inst = ItemListLogic(fake, thisItem, itemListBox);
                StartCoroutine(Inst);
                break;
            case 1:
                StartCoroutine(ItemListLogic(thisItem, itemListBox));
                break;

        }
    }

    IEnumerator ItemListLogic(NetworkObject fake, item thisItem, ItemListBox itemListBox){

        // Debug.Log("waiting for mouse button to be released...");
        mouseDownCoOn = true;
        yield return new WaitUntil(() => Input.GetMouseButtonUp(1));
        mouseDownCoOn = false;
        // Debug.Log("mouse button released. Raycasting...");
        // raycast from mouse y coordinate and this items x coordinate
        RaycastHit2D hit2D = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit2D.collider != null)
        {

            // if we hit a different item in the inventory, switch that items place in the hierarchy with this one.
            // Debug.Log(hit2D.transform.name);
            if (Input.GetKey(KeyCode.LeftShift) && hit2D.transform.name.Contains("Item Display Box Small") && thisItem.type != Type.backpack)
            {

                ItemDisplaySmall itemDisplay = hit2D.transform.GetComponent<ItemDisplaySmall>();
                if (itemDisplay.occupiedInventory.thisItemDisplay.thisItem.CapacityLogic(thisItem))
                {

                    for (int i = 0; i < thisItem.amount; i++)
                    {

                        itemDisplay.occupiedInventory.thisItemDisplay.thisItem.AddInventory(new item
                        {

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
                            isEquipped = false,
                            bodyparts = thisItem.bodyparts,
                            metadata = thisItem.metadata
                        }, itemDisplay.transform.GetSiblingIndex());
                    }
                    // List<itemShort> itemShorts = new List<itemShort>();
                    // foreach(ItemDisplay itemDisplay1 in itemDisplay.occupiedInventory.thisItemDisplay.occupiedInventory.transform.GetChild(4).GetChild(0).GetComponentsInChildren<ItemDisplay>()){

                    //     itemShorts.Add(new itemShort{

                    //         name = itemDisplay1.nameText.text,
                    //         id = itemDisplay1.id
                    //     });
                    // }
                    // GameManager.Singleton.ReorderInventoryRpc(GameManager.Singleton.interpreter.GetUsername, itemShorts.ToArray());
                    // Debug.Log($"Reorder inventory rpc called.");
                    // GameManager.Singleton.SaveDataRpc();

                    yield return new WaitForEndOfFrame();
                    GameManager.Singleton.RequestJsonRpc(GameManager.Singleton.interpreter.GetUsername, "host", $"/{GameManager.Singleton.interpreter.GetUsername} {itemDisplay.occupiedInventory.thisItemDisplay.nameText.text}{itemDisplay.occupiedInventory.thisItemDisplay.id} Inventory.json");

                    transform.root.GetComponent<User>().UpdateNetworkedSlotsRpc(GameManager.Singleton.interpreter.GetUsername);

                    yield return new WaitForEndOfFrame();
                    itemDisplay.occupiedInventory.LoadInventory();
                    // Destroy(gameObject);
                    GameManager.Singleton.UpdateIdTallyRpc();
                }
            }
            else if (Input.GetKey(KeyCode.LeftShift) && hit2D.transform.name.Contains("Item Display Box"))
            {

                ItemDisplay itemDisplay = hit2D.transform.GetComponent<ItemDisplay>();

                if (itemDisplay.occupiedInventory.CapacityLogic(thisItem, $"{thisItem.value}:{thisItem.type}"))
                {

                    // itemDisplay.RefreshItemDisplayBoxRpc(transform.root.name);
                    int siblingIndex = itemDisplay.transform.GetSiblingIndex();
                    itemDisplay.occupiedInventory.AddItemRpc(GameManager.Singleton.interpreter.GetUsername, thisItem, false, siblingIndex);
                    // GameManager.Singleton.SaveDataRpc();
                    // List<itemShort> itemShorts = new List<itemShort>();
                    // foreach(ItemDisplay itemDisplay1 in itemDisplay.transform.GetChild(4).GetChild(0).GetComponentsInChildren<ItemDisplay>()){

                    //     itemShorts.Add(new itemShort{

                    //         name = itemDisplay1.nameText.text,
                    //         id = itemDisplay1.id
                    //     });
                    // }
                    // GameManager.Singleton.ReorderInventoryRpc(GameManager.Singleton.interpreter.GetUsername, itemShorts.ToArray());
                    // Debug.Log($"Reorder inventory rpc called.");
                    yield return new WaitForEndOfFrame();

                    transform.root.GetComponent<User>().UpdateNetworkedSlotsRpc(GameManager.Singleton.interpreter.GetUsername);
                    GameManager.Singleton.UpdateIdTallyRpc();
                    // itemDisplay.RefreshItemDisplayBoxRpc(transform.root.name);
                    // Destroy(gameObject);
                }
            }
            else if (Input.GetKey(KeyCode.LeftShift) && hit2D.transform.name.Contains("Slot"))
            {

                if (thisItem.equippable)
                {

                    ArmorSlot otherArmorSlot = hit2D.transform.GetComponent<ArmorSlot>();
                    if (otherArmorSlot.description.bodypart.slot[otherArmorSlot.index].Empty())
                    {

                        otherArmorSlot.description.bodypart.slot[otherArmorSlot.index].item = new item
                        {

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
                            isEquipped = true,
                            bodyparts = thisItem.bodyparts,
                            metadata = thisItem.metadata
                        };
                        switch (otherArmorSlot.description.bodypart.slot[otherArmorSlot.index].item.type)
                        {

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
                            case Type.healthMult:
                                otherArmorSlot.description.bodypart.slot[otherArmorSlot.index].slotModifierType = SlotModifierType.hp;
                                break;
                            default:
                                otherArmorSlot.description.bodypart.slot[otherArmorSlot.index].slotModifierType = SlotModifierType.none;
                                break;
                        }
                        yield return new WaitForEndOfFrame();
                        itemListBox.transform.GetChild(0).gameObject.SetActive(true);
                        GameManager.Singleton.UpdateIdTallyRpc();
                    }
                    else
                    {

                        yield return new WaitForEndOfFrame();
                        itemListBox.transform.GetChild(0).gameObject.SetActive(true);
                    }
                }
            }
            else if (Input.GetKey(KeyCode.LeftShift) && hit2D.transform.name.Contains("Scroll"))
            {

                if (CapacityLogic(thisItem, $"{thisItem.value}:{thisItem.type}"))
                {

                    AddItemRpc(GameManager.Singleton.interpreter.GetUsername, thisItem, false);
                    // List<itemShort> itemShorts = new List<itemShort>();
                    // foreach(ItemDisplay itemDisplay1 in transform.GetChild(4).GetChild(0).GetComponentsInChildren<ItemDisplay>()){

                    //     itemShorts.Add(new itemShort{

                    //         name = itemDisplay1.nameText.text,
                    //         id = itemDisplay1.id
                    //     });
                    // }
                    // GameManager.Singleton.ReorderInventoryRpc(GameManager.Singleton.interpreter.GetUsername, itemShorts.ToArray());

                    yield return new WaitForEndOfFrame();

                    transform.root.GetComponent<User>().UpdateNetworkedSlotsRpc(GameManager.Singleton.interpreter.GetUsername);
                    GameManager.Singleton.UpdateIdTallyRpc();
                }
            }
            else if (Input.GetKey(KeyCode.LeftShift) && hit2D.transform.name.Contains("Inventory") && thisItem.type != Type.backpack)
            {

                InventorySmall inventorySmall = hit2D.transform.GetComponent<InventorySmall>();
                if (inventorySmall.thisItemDisplay.thisItem.CapacityLogic(thisItem))
                {

                    for (int i = 0; i < thisItem.amount; i++)
                    {

                        inventorySmall.thisItemDisplay.thisItem.AddInventory(new item
                        {

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
                            isEquipped = false,
                            bodyparts = thisItem.bodyparts,
                            metadata = thisItem.metadata
                        });
                    }
                    // List<itemShort> itemShorts = new List<itemShort>();
                    // foreach(ItemDisplay itemDisplay1 in itemDisplay.occupiedInventory.thisItemDisplay.occupiedInventory.transform.GetChild(4).GetChild(0).GetComponentsInChildren<ItemDisplay>()){

                    //     itemShorts.Add(new itemShort{

                    //         name = itemDisplay1.nameText.text,
                    //         id = itemDisplay1.id
                    //     });
                    // }
                    // GameManager.Singleton.ReorderInventoryRpc(GameManager.Singleton.interpreter.GetUsername, itemShorts.ToArray());
                    // Debug.Log($"Reorder inventory rpc called.");
                    // GameManager.Singleton.SaveDataRpc();

                    yield return new WaitForEndOfFrame();
                    GameManager.Singleton.RequestJsonRpc(GameManager.Singleton.interpreter.GetUsername, "host", $"/{GameManager.Singleton.interpreter.GetUsername} {inventorySmall.thisItemDisplay.nameText.text}{inventorySmall.thisItemDisplay.id} Inventory.json");

                    transform.root.GetComponent<User>().UpdateNetworkedSlotsRpc(GameManager.Singleton.interpreter.GetUsername);

                    yield return new WaitForEndOfFrame();
                    inventorySmall.LoadInventory();
                    // Destroy(gameObject);
                    GameManager.Singleton.UpdateIdTallyRpc();
                }
            }
        }
        else
        {
            // Debug.Log("Raycast did not hit any colliders.");
        }
        // Debug.Log("Destroying fake.");
        if(fake == null)
            yield break;
        Destroy(fake.gameObject);
        transform.root.GetComponent<User>().UpdateNetworkedSlotsRpc(GameManager.Singleton.interpreter.GetUsername);
        yield return new WaitForEndOfFrame();
        itemListBox.transform.GetChild(0).gameObject.SetActive(true);
    }

    void Update(){

        if(Input.GetKeyUp(KeyCode.LeftShift) && mouseDownCoOn){

            // if key released vvv
            // stop coroutine and safely reset vars
            StopCoroutine(Inst);
            Destroy(GameObject.Find("fake"));
            transform.root.GetComponent<User>().UpdateNetworkedSlotsRpc(GameManager.Singleton.interpreter.GetUsername);
            if(slotPriv != null)
                slotPriv.transform.GetChild(0).gameObject.SetActive(true);
            if (itemPriv != null)
                itemPriv.transform.GetChild(0).gameObject.SetActive(true);
            mouseDownCoOn = false;
        }
        if(Input.GetMouseButtonUp(0)){
            
            foreach(ArmorSlot armorSlot in description.armorSlots){

                if(armorSlot.selected){

                    RaycastHit2D hit2D = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                    if(hit2D.collider != null){

                        if(hit2D.transform.name.Contains("Scroll")){

                            iPanel_itemName.text = "";
                            iPanel_itemInfo.text = "Left-Click an item in the inventory to see information regarding it's application. Said information will show up here.";

                            armorSlot.selected = false;
                            armorSlot.background.color = Color.black;
                            armorSlot.itemName.color = Color.white;
                            armorSlot.bonus.color = Color.white;
                        }
                    }
                }
            }
            foreach (ItemListBox listBox in user.itemList.displays)
            {

                if (listBox.selected)
                {

                    RaycastHit2D hit2D = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                    if (hit2D.collider != null)
                    {

                        if (hit2D.transform.name.Contains("Scroll"))
                        {

                            iPanel_itemName.text = "";
                            iPanel_itemInfo.text = "Left-Click an item in the inventory to see information regarding it's application. Said information will show up here.";

                            listBox.selected = false;
                            listBox.background.color = Color.black;
                            listBox.itemName.color = Color.white;
                            listBox.weight.color = Color.white;
                            listBox.amount.color = Color.white;
                        }
                    }
                }
            }
        }
    }

    IEnumerator ArmorSlotLogic(item thisItem, ArmorSlot thisArmorSlot){

        yield return new WaitUntil(()=> Input.GetMouseButtonUp(0));

        if(thisArmorSlot.hovering){

            thisArmorSlot.selected = true;
            
            foreach (ItemListBox listBox in user.itemList.displays)
            {
                
                listBox.selected = false;
                listBox.background.color = Color.black;
                listBox.itemName.color = Color.white;
                listBox.weight.color = Color.white;
                listBox.amount.color = Color.white;
            }
            foreach (ArmorSlot slot in thisArmorSlot.description.armorSlots)
            {

                // if(!slot.gameObject.activeInHierarchy)
                //     continue;

                if (slot.Equals(thisArmorSlot))
                    continue;

                slot.selected = false;
                slot.background.color = Color.black;
                slot.itemName.color = Color.white;
                slot.bonus.color = Color.white;
            }
            foreach(NetworkObject netObj in itemDisplays){

                if(netObj == null)
                    continue;

                ItemDisplay itemDisplay = netObj.GetComponent<ItemDisplay>();
                
                itemDisplay.selected = false;
                itemDisplay.background.color = Color.black;
                itemDisplay.nameText.color = Color.white;
                itemDisplay.sizeText.color = Color.white;
                itemDisplay.weightText.color = Color.white;
            }
            if(ItemDisplay.BackpackOpen(this, out List<InventorySmall> inventorySmalls)){

                for(int i = 0; i < inventorySmalls.Count; i++){

                    foreach(NetworkObject netObj in inventorySmalls[i].itemDisplays){

                        if(netObj == null)
                            continue;

                        ItemDisplaySmall itemDisplay = netObj.GetComponent<ItemDisplaySmall>();
                        
                        itemDisplay.selected = false;
                        itemDisplay.background.color = Color.black;
                        itemDisplay.nameText.color = Color.white;
                        itemDisplay.sizeText.color = Color.white;
                        itemDisplay.weightText.color = Color.white;
                    }
                }
            }

            iPanel_itemName.text = thisItem.name.ToString();
            switch(thisItem.type){

                case Type.backpack:

                    iPanel_itemInfo.text = $"Holds {thisItem.value} large items worth of space, {thisItem.value*2} small items worth of space, or {thisItem.value*4} tiny items worth of space.";
                    break;
                case Type.other:

                    iPanel_itemInfo.text = $"This items information is unable to be processed do to the complications of it's nature. Please query the DM for additional information.";
                    break;
                case Type.food:

                    iPanel_itemInfo.text = "A food item, it will nourish you.";
                    break;
                case Type.heavyArmor:

                    iPanel_itemInfo.text = $"Heavy armor, +{thisItem.value} ac to applied body part. ";
                    break;
                case Type.lightArmor:

                    iPanel_itemInfo.text = $"Light armor, +{thisItem.value} ac to applied body part. ";
                    break;
                case Type.medical:

                    iPanel_itemInfo.text = $"Heals {thisItem.value} hp when applied to a body part.";
                    break;
                case Type.weapon:

                    iPanel_itemInfo.text = "No relevant information to display.";
                    break;
                case Type.healthMult:

                    iPanel_itemInfo.text = $"Increases the maximum hp of the relevant body part by {thisItem.value}";
                    break;
                case Type.capacityMult:

                    iPanel_itemInfo.text = $"Adds {thisItem.value} large items worth of space, {thisItem.value*2} small items worth of space, or {thisItem.value*4} tiny items worth of space, to your main inventory.";
                    break;
                case Type.capacityMultL:

                    iPanel_itemInfo.text = $"Adds {thisItem.value} large items worth of space to your main inventory.";
                    break;
                case Type.capacityMultS:

                    iPanel_itemInfo.text = $"Adds {thisItem.value} small items worth of space to your main inventory.";
                    break;
                case Type.capacityMultT:

                    iPanel_itemInfo.text = $"Adds {thisItem.value} tiny items worth of space to your main inventory.";
                    break;
            }
            if(thisItem.name.ToString().Contains("bullet proof")){

                iPanel_itemInfo.text += "Prevents lethal piercing damage to the protected body part, up to its specified protection level ";
                iPanel_itemInfo.text += $"({thisItem.metadata.ToString()}).";
            }
        }
    }

    IEnumerator ItemListLogic(item thisItem, ItemListBox thisItemListBox){

        yield return new WaitUntil(()=> Input.GetMouseButtonUp(0));

        if(thisItemListBox.hovering){

            thisItemListBox.selected = true;

            foreach (ItemListBox listBox in thisItemListBox.itemList.displays)
            {

                if (listBox.Equals(thisItemListBox))
                    continue;
                
                listBox.selected = false;
                listBox.background.color = Color.black;
                listBox.itemName.color = Color.white;
                listBox.weight.color = Color.white;
                listBox.amount.color = Color.white;
            }

            foreach (ArmorSlot slot in user.health.description.armorSlots)
            {

                // if(!slot.gameObject.activeInHierarchy)
                //     continue;

                slot.selected = false;
                slot.background.color = Color.black;
                slot.itemName.color = Color.white;
                slot.bonus.color = Color.white;
            }
            foreach(NetworkObject netObj in itemDisplays){

                if(netObj == null)
                    continue;

                ItemDisplay itemDisplay = netObj.GetComponent<ItemDisplay>();
                
                itemDisplay.selected = false;
                itemDisplay.background.color = Color.black;
                itemDisplay.nameText.color = Color.white;
                itemDisplay.sizeText.color = Color.white;
                itemDisplay.weightText.color = Color.white;
            }
            if(ItemDisplay.BackpackOpen(this, out List<InventorySmall> inventorySmalls)){

                for(int i = 0; i < inventorySmalls.Count; i++){

                    foreach(NetworkObject netObj in inventorySmalls[i].itemDisplays){

                        if(netObj == null)
                            continue;

                        ItemDisplaySmall itemDisplay = netObj.GetComponent<ItemDisplaySmall>();
                        
                        itemDisplay.selected = false;
                        itemDisplay.background.color = Color.black;
                        itemDisplay.nameText.color = Color.white;
                        itemDisplay.sizeText.color = Color.white;
                        itemDisplay.weightText.color = Color.white;
                    }
                }
            }

            iPanel_itemName.text = thisItem.name.ToString();
            switch(thisItem.type){

                case Type.backpack:

                    iPanel_itemInfo.text = $"Holds {thisItem.value} large items worth of space, {thisItem.value*2} small items worth of space, or {thisItem.value*4} tiny items worth of space.";
                    break;
                case Type.other:

                    iPanel_itemInfo.text = $"This items information is unable to be processed do to the complications of it's nature. Please query the DM for additional information.";
                    break;
                case Type.food:

                    iPanel_itemInfo.text = "A food item, it will nourish you.";
                    break;
                case Type.heavyArmor:

                    iPanel_itemInfo.text = $"Heavy armor, +{thisItem.value} ac to applied body part. ";
                    break;
                case Type.lightArmor:

                    iPanel_itemInfo.text = $"Light armor, +{thisItem.value} ac to applied body part. ";
                    break;
                case Type.medical:

                    iPanel_itemInfo.text = $"Heals {thisItem.value} hp when applied to a body part.";
                    break;
                case Type.weapon:

                    iPanel_itemInfo.text = "No relevant information to display.";
                    break;
                case Type.healthMult:

                    iPanel_itemInfo.text = $"Increases the maximum hp of the relevant body part by {thisItem.value}";
                    break;
                case Type.capacityMult:

                    iPanel_itemInfo.text = $"Adds {thisItem.value} large items worth of space, {thisItem.value*2} small items worth of space, or {thisItem.value*4} tiny items worth of space, to your main inventory.";
                    break;
                case Type.capacityMultL:

                    iPanel_itemInfo.text = $"Adds {thisItem.value} large items worth of space to your main inventory.";
                    break;
                case Type.capacityMultS:

                    iPanel_itemInfo.text = $"Adds {thisItem.value} small items worth of space to your main inventory.";
                    break;
                case Type.capacityMultT:

                    iPanel_itemInfo.text = $"Adds {thisItem.value} tiny items worth of space to your main inventory.";
                    break;
            }
            if(thisItem.name.ToString().Contains("bullet proof")){

                iPanel_itemInfo.text += "Prevents lethal piercing damage to the protected body part, up to its specified protection level ";
                iPanel_itemInfo.text += $"({thisItem.metadata.ToString()}).";
            }
        }
    }

    [Rpc(SendTo.Everyone)]
    public void AddItemRpc(string usernameI, item item, bool updateTally = true, int siblingIndex = -1)
    {
        if(usernameI != username)
            return;
        if(!IsOwner)
            return;
        
        // First, try to merge with an existing matching item.
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
                        isEquipped = false,
                        bodyparts = inventory[i].bodyparts,
                        metadata = inventory[i].metadata
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
        
        // No matching item found; insert at the given siblingIndex.
        if (CapacityLogic(item))
        {
            if (siblingIndex == -1)
                siblingIndex = inventory.Count;
            
            // Insert the new item at the specified index.
            inventory.Insert(siblingIndex, new item()
            {
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
                isEquipped = false,
                bodyparts = item.bodyparts,
                metadata = item.metadata
            });
            
            // After insertion, check if the item immediately following the inserted one is a duplicate.
            // That is, if the new item at index 'siblingIndex' has the same id as the one at 'siblingIndex+1'
            // then remove the duplicate (the one that was shifted down).
            if (siblingIndex < inventory.Count - 1)
            {
                if (inventory[siblingIndex].id == inventory[siblingIndex + 1].id)
                {
                    inventory.RemoveAt(siblingIndex + 1);
                }
            }
            
            RefreshItemDisplayBoxRpc(username);
            if (updateTally)
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

                if (inventory[i].type == Type.backpack)
                {

                    string path = Application.persistentDataPath + "/" + GameManager.Singleton.interpreter.GetUsername + inventory[i].itemInventory.ToString();
                    if(File.Exists(path))
                        File.Delete(path);

                    inventory.Remove(inventory[i]);
                    RefreshItemDisplayBoxRpc(username);
                }
                else if (inventory[i].amount > 1)
                {

                    inventory[i] = new item()
                    {

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
                        isEquipped = false,
                        bodyparts = inventory[i].bodyparts,
                        metadata = inventory[i].metadata
                    };
                    itemDisplays[i].GetComponent<ItemDisplay>().sizeText.text = $"{inventory[i].amount}{inventory[i].size}";
                    itemDisplays[i].GetComponent<ItemDisplay>().weightText.text = $"{inventory[i].weight} Lbs.";
                }
                else
                {

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

    [Rpc(SendTo.Everyone)]
    public void AddMoneyRpc(string usernameI, MoneyType moneyType, int amount){

        if(usernameI != username)
            return;
        if(!IsOwner)
            return;

        switch(moneyType){

            case MoneyType.soljik:
                soljik.Value += amount;
                soljikText.text = $"Soljik: {soljik.Value:0000000000000}";
                break;
            case MoneyType.brine:
                brine.Value += amount;
                brineText.text = $"Brine: {brine.Value:000}";
                break;
            case MoneyType.penc:
                penc.Value += amount;
                pencText.text = $"Penc: {penc.Value:000}";
                break;
        }
    }
    [Rpc(SendTo.Everyone)]
    public void RemoveMoneyRpc(string usernameI, MoneyType moneyType, int amount){

        if(usernameI != username)
            return;
        if(!IsOwner)
            return;

        switch(moneyType){

            case MoneyType.soljik:
                soljik.Value -= amount;
                soljikText.text = $"Soljik: {soljik.Value:0000000000000}";
                break;
            case MoneyType.brine:
                brine.Value -= amount;
                brineText.text = $"Brine: {brine.Value:000}";
                break;
            case MoneyType.penc:
                penc.Value -= amount;
                pencText.text = $"Penc: {penc.Value:000}";
                break;
        }
    }
}
public enum MoneyType{

    soljik = 0,
    brine = 1,
    penc = 2
}