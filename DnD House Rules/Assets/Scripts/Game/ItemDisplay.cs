using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ItemDisplay : MonoBehaviour
{

    public item thisItem;
    public Backpack occupiedInventory;

    public bool hovering, selected;
    public TMP_Text nameText;
    public TMP_Text sizeText;
    public TMP_Text weightText;

    public RawImage background;
    RectTransform transformRect;

    public GameObject inventoryDisplayObject;
    [SerializeField] RectTransform visuals;

    public Type type = Type.other;
    public int id;
    public bool isOpen = false;

    public Camera cam;

    void Start(){

        transformRect = GetComponent<RectTransform>();
        User user = GameObject.Find(GameManager.Singleton.interpreter.GetUsername).GetComponent<User>();
        cam = user.transform.GetChild(0).GetComponent<Camera>();
        // Debug.Log("Item display was instantiated");
    }

    void Update(){

        if(Input.GetKeyUp(KeyCode.Delete) && selected){

            occupiedInventory.RemoveItemRpc(GameManager.Singleton.interpreter.GetUsername, thisItem.name.ToString(), true, thisItem.id);
        }
        if(Input.GetMouseButtonUp(0) && selected){
            
            RaycastHit2D hit2D = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if(hit2D.collider != null){

                if(hit2D.transform.name.Contains("Scroll")){

                    occupiedInventory.iPanel_itemName.text = "";
                    occupiedInventory.iPanel_itemInfo.text = "Left-Click an item in the inventory to see information regarding it's application. Said information will show up here.";

                    background.color = Color.black;
                    nameText.color = Color.white;
                    sizeText.color = Color.white;
                    weightText.color = Color.white;
                    selected = false;
                }
            }
        }
    }

    public static bool BackpackOpen(Backpack occupiedInventory, out List<InventorySmall> inventorySmall){

        inventorySmall = new List<InventorySmall>();
        foreach(NetworkObject netObj in occupiedInventory.itemDisplays){

            if(netObj == null)
                continue;

            ItemDisplay itemDisplay = netObj.GetComponent<ItemDisplay>();
            if(itemDisplay.thisItem.type != Type.backpack)
                continue;
            if(itemDisplay.isOpen){

                inventorySmall.Add(itemDisplay.inventoryDisplayObject.GetComponent<InventorySmall>());
            }
        }
        if(inventorySmall.Count > 0)
            return true;
        else
            return false;
    }

    IEnumerator OnMouseOver(){

        hovering = true;
        background.color = Color.white;
        nameText.color = Color.black;
        sizeText.color = Color.black;
        weightText.color = Color.black;

        if(Input.GetMouseButtonDown(0)){
            
            yield return new WaitUntil(()=> Input.GetMouseButtonUp(0));

            if(hovering){
                selected = true;
                foreach(NetworkObject netObj in occupiedInventory.itemDisplays){

                    if(netObj == null)
                        continue;

                    ItemDisplay itemDisplay = netObj.GetComponent<ItemDisplay>();
                    if(itemDisplay.Equals(this))
                        continue;
                    
                    itemDisplay.selected = false;
                    itemDisplay.background.color = Color.black;
                    itemDisplay.nameText.color = Color.white;
                    itemDisplay.sizeText.color = Color.white;
                    itemDisplay.weightText.color = Color.white;
                }
                if(BackpackOpen(occupiedInventory, out List<InventorySmall> inventorySmalls)){

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
                foreach(ArmorSlot slot in occupiedInventory.description.armorSlots){

                    // if(!slot.gameObject.activeInHierarchy)
                    //     continue;
                    
                    slot.selected = false;
                    slot.background.color = Color.black;
                    slot.itemName.color = Color.white;
                    slot.bonus.color = Color.white;
                }
                occupiedInventory.iPanel_itemName.text = thisItem.name.ToString();
                switch(type){

                    case Type.backpack:
                        if(isOpen){

                            transformRect.sizeDelta -= Vector2.up * 100;
                            inventoryDisplayObject.SetActive(false);
                            visuals.localPosition -= Vector3.up * 50;
                            isOpen = false;
                            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
                            GetComponent<BoxCollider2D>().offset -= Vector2.up * 50;
                        }
                        else{

                            transformRect.sizeDelta += Vector2.up * 100;
                            inventoryDisplayObject.SetActive(true);
                            visuals.localPosition += Vector3.up * 50;
                            isOpen = true;
                            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
                            GetComponent<BoxCollider2D>().offset += Vector2.up * 50;
                        }
                        occupiedInventory.iPanel_itemInfo.text = $"Holds {thisItem.value} large items worth of space, {thisItem.value*2} small items worth of space, or {thisItem.value*4} tiny items worth of space.";
                        break;
                    case Type.other:

                        occupiedInventory.iPanel_itemInfo.text = $"This items information is unable to be processed do to the complications of it's nature. Please query the DM for additional information.";
                        break;
                    case Type.food:

                        occupiedInventory.iPanel_itemInfo.text = "A food item, it will nourish you.";
                        break;
                    case Type.heavyArmor:

                        occupiedInventory.iPanel_itemInfo.text = $"Heavy armor, +{thisItem.value} ac to applied body part. ";
                        break;
                    case Type.lightArmor:

                        occupiedInventory.iPanel_itemInfo.text = $"Light armor, +{thisItem.value} ac to applied body part. ";
                        break;
                    case Type.medical:

                        occupiedInventory.iPanel_itemInfo.text = $"Heals {thisItem.value} hp when applied to a body part.";
                        break;
                    case Type.weapon:

                        occupiedInventory.iPanel_itemInfo.text = "No relevant information to display.";
                        break;
                    case Type.healthMult:

                        occupiedInventory.iPanel_itemInfo.text = $"Increases the maximum hp of the relevant body part by {thisItem.value}";
                        break;
                    case Type.capacityMult:

                        occupiedInventory.iPanel_itemInfo.text = $"Adds {thisItem.value} large items worth of space, {thisItem.value*2} small items worth of space, or {thisItem.value*4} tiny items worth of space, to your main inventory.";
                        break;
                    case Type.capacityMultL:

                        occupiedInventory.iPanel_itemInfo.text = $"Adds {thisItem.value} large items worth of space to your main inventory.";
                        break;
                    case Type.capacityMultS:

                        occupiedInventory.iPanel_itemInfo.text = $"Adds {thisItem.value} small items worth of space to your main inventory.";
                        break;
                    case Type.capacityMultT:

                        occupiedInventory.iPanel_itemInfo.text = $"Adds {thisItem.value} tiny items worth of space to your main inventory.";
                        break;
                }
                if(thisItem.name.ToString().Contains("bullet proof")){

                    occupiedInventory.iPanel_itemInfo.text += "Prevents lethal piercing damage to the protected body part, up to its specified protection level ";
                    occupiedInventory.iPanel_itemInfo.text += $"({thisItem.metadata.ToString()}).";
                }

            }
        }

        if(Input.GetMouseButtonDown(1) && !(thisItem.type == Type.backpack && isOpen)){

            // instantiate a new gameobject that follows the mouse
            GameManager.Singleton.SaveDataRpc();
            NetworkObject fake = Instantiate(GameManager.Singleton.itemDisplayBoxMouse, !Input.GetKey(KeyCode.LeftShift) ? transform.parent.parent : transform.parent.parent.parent);
            ItemDisplayBoxMouse fakeDisplay = fake.GetComponent<ItemDisplayBoxMouse>();
            fakeDisplay.nameText.text = nameText.text;
            fakeDisplay.sizeText.text = sizeText.text;
            fakeDisplay.weightText.text = weightText.text;
            fake.transform.localScale *= 1.1f;
            transform.GetChild(0).gameObject.SetActive(false);
            yield return new WaitForEndOfFrame();
            fake.transform.GetChild(0).gameObject.SetActive(true);
            yield return new WaitUntil(() => Input.GetMouseButtonUp(1));
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
                                isEquipped = false,
                                bodyparts = thisItem.bodyparts,
                                metadata = thisItem.metadata
                            }, itemDisplay.transform.GetSiblingIndex());
                        }
                        // Debug.Log($"Reorder inventory rpc called.");
                        // GameManager.Singleton.SaveDataRpc();
                        GameManager.Singleton.RequestJsonRpc(GameManager.Singleton.interpreter.GetUsername, "host", $"/{GameManager.Singleton.interpreter.GetUsername} {itemDisplay.occupiedInventory.thisItemDisplay.nameText.text}{itemDisplay.occupiedInventory.thisItemDisplay.id} Inventory.json");
                        yield return new WaitForEndOfFrame();
                        for(int i = 0; i < occupiedInventory.inventory.Count; i++){

                            if(occupiedInventory.inventory[i].name.ToString() == thisItem.name.ToString() && occupiedInventory.inventory[i].id == thisItem.id){

                                // Debug.Log($"removing {occupiedInventory.inventory[i].name.ToString()} with id: {occupiedInventory.inventory[i].id}");
                                occupiedInventory.inventory.RemoveAt(i);
                                // GameManager.Singleton.SaveDataRpc();
                            }
                        }
                        List<itemShort> itemShorts = new List<itemShort>();
                        foreach(ItemDisplay itemDisplay1 in occupiedInventory.transform.GetChild(4).GetChild(0).GetComponentsInChildren<ItemDisplay>()){

                            itemShorts.Add(new itemShort{

                                name = itemDisplay1.nameText.text,
                                id = itemDisplay1.id
                            });
                        }
                        GameManager.Singleton.ReorderInventoryRpc(GameManager.Singleton.interpreter.GetUsername, itemShorts.ToArray());
                        itemDisplay.occupiedInventory.LoadInventory();
                        Destroy(gameObject);
                    }
                }
                else if(hit2D.transform.name.Contains("Item Display Box")){

                    transform.SetSiblingIndex(hit2D.transform.GetSiblingIndex());
                    // ItemDisplay otherItemDisplay = hit2D.transform.GetComponent<ItemDisplay>();

                    List<itemShort> itemShorts = new List<itemShort>();
                    foreach(ItemDisplay itemDisplay in occupiedInventory.transform.GetChild(4).GetChild(0).GetComponentsInChildren<ItemDisplay>()){

                        itemShorts.Add(new itemShort{

                            name = itemDisplay.nameText.text,
                            id = itemDisplay.id
                        });
                    }
                    GameManager.Singleton.ReorderInventoryRpc(GameManager.Singleton.interpreter.GetUsername, itemShorts.ToArray());
                    // Debug.Log($"Reorder inventory rpc called.");
                }
                else if(hit2D.transform.name.Contains("Slot")){

                    ArmorSlot armorSlot = hit2D.transform.GetComponent<ArmorSlot>();
                    foreach(string bodypartName in thisItem.GetBodyparts()){

                        if(bodypartName == armorSlot.description.bodypart.name){

                            if(thisItem.equippable){

                                if(armorSlot.description.bodypart.slot[armorSlot.index].Empty()){

                                    armorSlot.description.bodypart.slot[armorSlot.index].item = new item{

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
                                    switch(armorSlot.description.bodypart.slot[armorSlot.index].item.type){

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
                                    switch(armorSlot.description.bodypart.slot[armorSlot.index].slotModifierType){

                                        case SlotModifierType.ac:
                                            armorSlot.description.bodypart.ac.Value += thisItem.value;
                                            break;
                                        case SlotModifierType.hp:
                                            armorSlot.description.bodypart.maximumHP.Value += thisItem.value;
                                            break;
                                    }
                                    transform.root.GetComponent<User>().UpdateNetworkedSlotsRpc(GameManager.Singleton.interpreter.GetUsername);
                                    occupiedInventory.RemoveItemRpc(GameManager.Singleton.interpreter.GetUsername, thisItem.name.ToString(), true, thisItem.id);
                                }
                                else if(occupiedInventory.CapacityLogic(armorSlot.description.bodypart.slot[armorSlot.index].item)){

                                    // swap the item in the armor slot with this item.
                                    // throw new NotImplementedException();
                                    occupiedInventory.AddItemRpc(GameManager.Singleton.interpreter.GetUsername, armorSlot.description.bodypart.slot[armorSlot.index].item, false);
                                    switch(armorSlot.description.bodypart.slot[armorSlot.index].slotModifierType){

                                        case SlotModifierType.ac:
                                            armorSlot.description.bodypart.ac.Value -= armorSlot.description.bodypart.slot[armorSlot.index].item.value;
                                            break;
                                        case SlotModifierType.hp:
                                            armorSlot.description.bodypart.maximumHP.Value -= armorSlot.description.bodypart.slot[armorSlot.index].item.value;
                                            break;
                                    }
                                    armorSlot.description.bodypart.slot[armorSlot.index].item = new item{

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
                                    switch(armorSlot.description.bodypart.slot[armorSlot.index].item.type){

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
                                    switch(armorSlot.description.bodypart.slot[armorSlot.index].slotModifierType){

                                        case SlotModifierType.ac:
                                            armorSlot.description.bodypart.ac.Value += thisItem.value;
                                            break;
                                        case SlotModifierType.hp:
                                            armorSlot.description.bodypart.maximumHP.Value += thisItem.value;
                                            break;
                                    }
                                    transform.root.GetComponent<User>().UpdateNetworkedSlotsRpc(GameManager.Singleton.interpreter.GetUsername);
                                    occupiedInventory.RemoveItemRpc(GameManager.Singleton.interpreter.GetUsername, thisItem.name.ToString(), true, thisItem.id);
                                }
                            }
                            break;
                        }
                    }
                }
                else if(hit2D.transform.name.Contains("BODY")){

                    Bodypart bodypart = hit2D.transform.GetComponent<Bodypart>();

                    switch(thisItem.type){

                        case Type.medical:
                            bodypart.currentHP.Value += thisItem.value;
                            occupiedInventory.RemoveItemRpc(GameManager.Singleton.interpreter.GetUsername, thisItem.name.ToString(), true, thisItem.id);
                            break;
                        case Type.food: // implement hunger?
                            bodypart.user.AddHungiesRpc(GameManager.Singleton.interpreter.GetUsername, thisItem.value);
                            if(bodypart.name == Health.bodypartDictionary[0]){

                                occupiedInventory.RemoveItemRpc(GameManager.Singleton.interpreter.GetUsername, thisItem.name.ToString(), true, thisItem.id);
                            }
                            break;
                    }
                }
            }
            Destroy(fake.gameObject);
            GameManager.Singleton.SaveDataRpc();
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    void OnMouseExit(){

        if(!selected){

            hovering = false;
            background.color = Color.black;
            nameText.color = Color.white;
            sizeText.color = Color.white;
            weightText.color = Color.white;
        }
    }
}
