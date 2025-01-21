using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
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

        if(Input.GetMouseButtonDown(1)){

            // instantiate a new gameobject that follows the mouse
            NetworkObject fake = Instantiate(GameManager.Singleton.itemDisplayBoxMouse, transform.parent.parent);
            ItemDisplayBoxMouse fakeDisplay = fake.GetComponent<ItemDisplayBoxMouse>();
            fakeDisplay.nameText.text = nameText.text;
            fakeDisplay.sizeText.text = sizeText.text;
            fakeDisplay.weightText.text = weightText.text;
            fake.transform.localScale *= 1.1f;
            transform.GetChild(0).gameObject.SetActive(false);
            yield return new WaitUntil(() => Input.GetMouseButtonUp(1));
            // raycast from mouse y coordinate and this items x coordinate
            RaycastHit2D hit2D = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if(hit2D.collider != null){

                // if we hit a different item in the inventory, switch that items place in the hierarchy with this one.
                
                if(hit2D.transform.name.Contains("Item Display Box Small") && thisItem.type != Type.backpack){

                    ItemDisplaySmall itemDisplay = hit2D.transform.GetComponent<ItemDisplaySmall>();
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
                            id = thisItem.id
                        });
                    }
                    // GameManager.Singleton.SaveData();
                    yield return new WaitForEndOfFrame();
                    for(int i = 0; i < occupiedInventory.inventory.Count; i++){

                        if(occupiedInventory.inventory[i].name.ToString() == thisItem.name.ToString() && occupiedInventory.inventory[i].id == thisItem.id){

                            Debug.Log($"removing {occupiedInventory.inventory[i].name.ToString()} with id: {occupiedInventory.inventory[i].id}");
                            occupiedInventory.inventory.RemoveAt(i);
                            // GameManager.Singleton.SaveData();
                        }
                    }
                    Destroy(gameObject);
                }
                else if(hit2D.transform.name.Contains("Item Display Box")){

                    transform.SetSiblingIndex(hit2D.transform.GetSiblingIndex());
                    // ItemDisplay otherItemDisplay = hit2D.transform.GetComponent<ItemDisplay>();
                }
            }
            Destroy(fake.gameObject);
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
