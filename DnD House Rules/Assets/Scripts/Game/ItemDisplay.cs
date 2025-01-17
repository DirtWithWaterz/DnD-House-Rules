using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDisplay : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text sizeText;
    public TMP_Text weightText;

    [SerializeField] RawImage background;
    RectTransform transformRect;

    public Type type = Type.other;
    public bool isOpen = false;

    void Start(){

        transformRect = GetComponent<RectTransform>();
    }

    void OnMouseOver(){

        background.color = Color.white;
        nameText.color = Color.black;
        sizeText.color = Color.black;
        weightText.color = Color.black;

        if(Input.GetMouseButtonUp(0)){

            if(type == Type.backpack){

                if(isOpen){

                    transformRect.sizeDelta -= Vector2.up * 15;
                    isOpen = false;
                    LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
                }
                else{

                    transformRect.sizeDelta += Vector2.up * 15;
                    isOpen = true;
                    LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
                }
            }
        }
    }

    void OnMouseExit(){

        background.color = Color.black;
        nameText.color = Color.white;
        sizeText.color = Color.white;
        weightText.color = Color.white;
    }
}
