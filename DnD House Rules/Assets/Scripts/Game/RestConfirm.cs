using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class RestConfirm : MonoBehaviour
{
    public int index;
    bool hovering;

    User user;

    [SerializeField] Image background;
    [SerializeField] TMP_Text label;

    [SerializeField] Rest rest;

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

        hovering = true;
        if(Input.GetMouseButtonDown(0)){

            yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
            if(hovering){

                background.color = Color.black;
                label.color = Color.white;
                rest.DoRest(index);
                switch(index){

                    case 0:
                        GameManager.Singleton.interpreter.NoticeRpc($"notify all {transform.root.name} took a long rest, healing 1 hp on their {rest.bodypartDict1[rest.selectedBodypartIndex]}");
                        break;
                    case 1:
                        GameManager.Singleton.interpreter.NoticeRpc($"notify all {transform.root.name} took a short rest.");
                        break;
                }
            }
        }

    }

    void OnMouseExit(){

        hovering = false;
        background.color = Color.black;
        label.color = Color.white;
    }
}
