using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ItemDisplay : MonoBehaviour
{
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

    [SerializeField] ItemDisplayBoxMouse fake;

    Camera cam;

    IEnumerator Start(){

        transformRect = GetComponent<RectTransform>();
        User user = GameObject.Find(GameManager.Singleton.interpreter.GetUsername).GetComponent<User>();
        cam = user.transform.GetChild(0).GetComponent<Camera>();
        fake = user.GetComponentInChildren<ItemDisplayBoxMouse>();
        yield return new WaitUntil(() => user.isInitialized.Value);
        fake.gameObject.SetActive(false);
    }

    void Update(){

        if(fake.isActiveAndEnabled)
            fake.transform.position = cam.ScreenToWorldPoint(Input.mousePosition);
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
            fake.gameObject.SetActive(true);
            fake.nameText.text = nameText.text;
            fake.sizeText.text = sizeText.text;
            fake.weightText.text = weightText.text;
            yield return new WaitUntil(() => Input.GetMouseButtonUp(1));
            // raycast from mouse y coordinate and this items x coordinate
            RaycastHit2D hit2D = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if(hit2D.collider != null){

                // if we hit a different item in the inventory, switch that items place in the hierarchy with this one.
                transform.SetSiblingIndex(hit2D.transform.GetSiblingIndex());
            }
            fake.gameObject.SetActive(false);

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
