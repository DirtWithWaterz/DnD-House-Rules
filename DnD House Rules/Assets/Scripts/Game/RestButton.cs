using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class RestButton : MonoBehaviour
{
    public int index;

    User user;

    [SerializeField] Image background;
    [SerializeField] TMP_Text label;

    void Start(){

        background = transform.GetChild(0).GetComponent<Image>();
        label = transform.GetChild(1).GetComponent<TMP_Text>();
        user = transform.root.GetComponent<User>();
    }

    void OnMouseOver(){
        if(!user.isInitialized.Value)
            return;

        background.color = Color.white;
        label.color = Color.black;


        if(Input.GetMouseButtonUp(0) && index==0){

            background.color = Color.black;
            label.color = Color.white;
            throw new NotImplementedException();
        }
        else if(Input.GetMouseButtonUp(0) && index==1){

            background.color = Color.black;
            label.color = Color.white;
            throw new NotImplementedException();
        }
    }

    void OnMouseExit(){

        background.color = Color.black;
        label.color = Color.white;
    }
}
