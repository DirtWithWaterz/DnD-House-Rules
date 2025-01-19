using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDisplaySmall : MonoBehaviour
{
    bool hovering;
    public TMP_Text nameText;
    public TMP_Text sizeText;
    public TMP_Text weightText;

    [SerializeField] RawImage background;
    // RectTransform transformRect;

    // [SerializeField] GameObject inventoryDisplayObject;
    // [SerializeField] RectTransform visuals;

    public Type type = Type.other;
    // public bool isOpen = false;

    void Start(){

        // transformRect = GetComponent<RectTransform>();
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
    }

    void OnMouseExit(){

        hovering = false;
        background.color = Color.black;
        nameText.color = Color.white;
        sizeText.color = Color.white;
        weightText.color = Color.white;
    }
}
