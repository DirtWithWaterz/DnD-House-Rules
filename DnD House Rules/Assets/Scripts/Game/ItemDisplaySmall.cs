using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ItemDisplaySmall : MonoBehaviour
{

    public item thisItem;
    public InventorySmall occupiedInventory;

    bool hovering;
    public TMP_Text nameText;
    public TMP_Text sizeText;
    public TMP_Text weightText;

    [SerializeField] RawImage background;

    Camera cam;

    // RectTransform transformRect;

    // [SerializeField] GameObject inventoryDisplayObject;
    // [SerializeField] RectTransform visuals;

    public Type type = Type.other;
    // public bool isOpen = false;

    void Start(){

        // transformRect = GetComponent<RectTransform>();
        cam = occupiedInventory.thisItemDisplay.cam;
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

                // if(type == Type.backpack){

                //     if(isOpen){

                //         transformRect.sizeDelta -= Vector2.up * 100;
                //         inventoryDisplayObject.SetActive(false);
                //         visuals.localPosition -= Vector3.up * 50;
                //         isOpen = false;
                //         LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
                //         GetComponent<BoxCollider2D>().offset -= Vector2.up * 50;
                //     }
                //     else{

                //         transformRect.sizeDelta += Vector2.up * 100;
                //         inventoryDisplayObject.SetActive(true);
                //         visuals.localPosition += Vector3.up * 50;
                //         isOpen = true;
                //         LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
                //         GetComponent<BoxCollider2D>().offset += Vector2.up * 50;
                //     }
                // }
            }
        }
    
        if(Input.GetMouseButtonDown(1)){

            // instantiate a new gameobject that follows the mouse
            NetworkObject fake = Instantiate(GameManager.Singleton.itemDisplayBoxMouse, transform.parent.parent.parent.parent.parent.parent);
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
                
                if(hit2D.transform.name.Contains("Item Display Box Small")){

                    transform.SetSiblingIndex(hit2D.transform.GetSiblingIndex());
                }
                else if(hit2D.transform.name.Contains("Item Display Box")){

                    ItemDisplay itemDisplay = hit2D.transform.GetComponent<ItemDisplay>();
                    
                    // itemDisplay.occupiedInventory.RefreshItemDisplayBoxRpc(transform.root.name);
                    itemDisplay.occupiedInventory.inventory.Add(thisItem);
                    // GameManager.Singleton.SaveData();
                    yield return new WaitForEndOfFrame();
                    itemDisplay.occupiedInventory.RefreshItemDisplayBoxRpc(transform.root.name);

                    List<item> thisItemInventory = occupiedInventory.thisItemDisplay.thisItem.GetInventory().ToList();

                    for(int i = 0; i < thisItemInventory.Count; i++){

                        if(thisItemInventory[i].name.ToString() == thisItem.name.ToString() && thisItemInventory[i].id == thisItem.id){

                            Debug.Log($"removing {thisItemInventory[i].name.ToString()} with id: {thisItemInventory[i].id} from the index: {i}");
                            thisItemInventory.RemoveAt(i);
                            i = -1;
                        }
                    }
                    occupiedInventory.thisItemDisplay.thisItem.SetInventory(thisItemInventory.ToArray());
                    // itemDisplay.occupiedInventory.RefreshItemDisplayBoxRpc(transform.root.name);
                    // Destroy(gameObject);
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
