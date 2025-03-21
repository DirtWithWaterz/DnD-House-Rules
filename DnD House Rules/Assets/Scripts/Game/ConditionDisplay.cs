using System;
using System.Collections;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public struct condition:IEquatable<condition>,INetworkSerializable{

    public FixedString32Bytes name;
    public FixedString4096Bytes data;
    public FixedString32Bytes bodypart;

    public bool Equals(condition other)
    {
        return name == other.name && bodypart == other.bodypart;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref name);
        serializer.SerializeValue(ref data);
        serializer.SerializeValue(ref bodypart);
    }
}

public class ConditionDisplay : MonoBehaviour
{

    public condition thisCondition;
    public ConditionsUI conditionsUI;

    public bool hovering, selected;
    public TMP_Text bodypartNameText;
    public TMP_Text conditionNameText;

    public RawImage background;
    RectTransform transformRect;

    // public GameObject inventoryDisplayObject;
    // [SerializeField] RectTransform visuals;

    public bool isOpen = false;

    public Camera cam;

    void Start(){

        transformRect = GetComponent<RectTransform>();
        User user = GameObject.Find(GameManager.Singleton.interpreter.GetUsername).GetComponent<User>();
        cam = user.transform.GetChild(0).GetComponent<Camera>();
        // Debug.Log("Item display was instantiated");
    }

    void Update(){

        if(Input.GetMouseButtonUp(0) && selected){
            
            RaycastHit2D hit2D = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if(hit2D.collider != null){

                if(hit2D.transform.name.Contains("Scroll")){

                    conditionsUI.iPanel_conditionInfo.text = "Information regarding specific conditions will be listed here. Any conditions currently afflicting the body will show up on the right. Left-clicik one of the items and its information will show up here.";

                    background.color = Color.black;
                    bodypartNameText.color = Color.white;
                    conditionNameText.color = Color.white;
                    selected = false;
                }
            }
        }
    }

    IEnumerator OnMouseOver(){

        hovering = true;
        background.color = Color.white;
        bodypartNameText.color = Color.black;
        conditionNameText.color = Color.black;

        if(Input.GetMouseButtonDown(0)){
            
            yield return new WaitUntil(()=> Input.GetMouseButtonUp(0));

            if(hovering){
                selected = true;
                foreach(NetworkObject netObj in conditionsUI.conditionDisplays){

                    if(netObj == null)
                        continue;

                    ConditionDisplay ConditionDisplay = netObj.GetComponent<ConditionDisplay>();
                    if(ConditionDisplay.Equals(this))
                        continue;
                    
                    ConditionDisplay.selected = false;
                    ConditionDisplay.background.color = Color.black;
                    ConditionDisplay.bodypartNameText.color = Color.white;
                    ConditionDisplay.conditionNameText.color = Color.white;
                }
            }
        }
    }

    void OnMouseExit(){

        if(!selected){

            hovering = false;
            background.color = Color.black;
            bodypartNameText.color = Color.white;
            conditionNameText.color = Color.white;
        }
    }
}
