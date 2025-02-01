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

    bool hovering;
    public TMP_Text nameText;
    public TMP_Text sizeText;
    public TMP_Text weightText;

    [SerializeField] RawImage background;
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


    IEnumerator OnMouseOver(){

        hovering = true;
        background.color = Color.white;
        nameText.color = Color.black;
        sizeText.color = Color.black;
        weightText.color = Color.black;

        if(Input.GetMouseButtonDown(0)){
            
            yield return new WaitUntil(()=> Input.GetMouseButtonUp(0));
            if(hovering){

                if(type == Type.backpack){

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
                }
            }
        }

        if(Input.GetMouseButtonDown(1) && !(thisItem.type == Type.backpack && isOpen)){

            // instantiate a new gameobject that follows the mouse
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
                        foreach(ItemDisplay itemDisplay1 in occupiedInventory.transform.GetChild(1).GetChild(0).GetComponentsInChildren<ItemDisplay>()){

                            itemShorts.Add(new itemShort{

                                name = itemDisplay1.nameText.text,
                                id = itemDisplay1.id
                            });
                        }
                        GameManager.Singleton.ReorderInventoryRpc(GameManager.Singleton.interpreter.GetUsername, itemShorts.ToArray());
                        // Debug.Log($"Reorder inventory rpc called.");
                        // GameManager.Singleton.SaveData();
                        yield return new WaitForEndOfFrame();
                        for(int i = 0; i < occupiedInventory.inventory.Count; i++){

                            if(occupiedInventory.inventory[i].name.ToString() == thisItem.name.ToString() && occupiedInventory.inventory[i].id == thisItem.id){

                                // Debug.Log($"removing {occupiedInventory.inventory[i].name.ToString()} with id: {occupiedInventory.inventory[i].id}");
                                occupiedInventory.inventory.RemoveAt(i);
                                // GameManager.Singleton.SaveData();
                            }
                        }
                        Destroy(gameObject);
                    }
                }
                else if(hit2D.transform.name.Contains("Item Display Box")){

                    transform.SetSiblingIndex(hit2D.transform.GetSiblingIndex());
                    // ItemDisplay otherItemDisplay = hit2D.transform.GetComponent<ItemDisplay>();

                    List<itemShort> itemShorts = new List<itemShort>();
                    foreach(ItemDisplay itemDisplay in occupiedInventory.transform.GetChild(1).GetChild(0).GetComponentsInChildren<ItemDisplay>()){

                        itemShorts.Add(new itemShort{

                            name = itemDisplay.nameText.text,
                            id = itemDisplay.id
                        });
                    }
                    GameManager.Singleton.ReorderInventoryRpc(GameManager.Singleton.interpreter.GetUsername, itemShorts.ToArray());
                    // Debug.Log($"Reorder inventory rpc called.");
                }
                else if(hit2D.transform.name.Contains("Slot")){

                    if(thisItem.equippable){

                        ArmorSlot armorSlot = hit2D.transform.GetComponent<ArmorSlot>();
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
                                isEquipped = true
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
                                case Type.medical:
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
                                isEquipped = true
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
                                case Type.medical:
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
                            occupiedInventory.RemoveItemRpc(GameManager.Singleton.interpreter.GetUsername, thisItem.name.ToString(), true, thisItem.id);
                        }
                    }
                }
            }
            Destroy(fake.gameObject);
            GameManager.Singleton.SaveData();
            transform.GetChild(0).gameObject.SetActive(true);


        }
    }

    void OnMouseExit(){

        hovering = false;
        background.color = Color.black;
        nameText.color = Color.white;
        sizeText.color = Color.white;
        weightText.color = Color.white;
    }
}
