using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class RestButton : MonoBehaviour
{
    public int index;
    bool hovering;

    User user;

    [SerializeField] Image background;
    [SerializeField] TMP_Text label;

    [SerializeField] GameObject popup;

    void Start(){

        background = transform.GetChild(0).GetComponent<Image>();
        label = transform.GetChild(1).GetComponent<TMP_Text>();
        user = transform.root.GetComponent<User>();
    }

    IEnumerator OnMouseOver(){

        if(!user.isInitialized.Value)
            yield break;
        background.color = Color.white;
        label.color = Color.black;
        if(GameManager.Singleton.interpreter.transform.root.GetChild(0).gameObject.activeInHierarchy)
            yield break;
        
        hovering = true;
        if(Input.GetMouseButtonDown(0)){

            yield return new WaitUntil(() => Input.GetMouseButtonUp(0));

            if(hovering && index==0){

                background.color = Color.black;
                label.color = Color.white;
                popup.SetActive(true);
            }
            else if(hovering && index==1){

                background.color = Color.black;
                label.color = Color.white;
                popup.SetActive(true);
            }
        }
    }

    void OnMouseExit(){

        hovering = false;
        background.color = Color.black;
        label.color = Color.white;
    }
}
