using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeathSaveButton : MonoBehaviour
{
    bool hovering;
    [SerializeField] byte index;
    User user;

    [SerializeField] List<SpriteRenderer> saveDots;

    [SerializeField] SpriteRenderer sr;

    [SerializeField] Image background;
    [SerializeField] TMP_Text label;

    // Start is called before the first frame update
    void Start()
    {
        user = transform.root.GetComponent<User>();
    }

    IEnumerator OnMouseOver(){
        if(!user.isInitialized.Value)
            yield break;

        if(background != null)
            background.color = Color.white;
        if(label != null)
            label.color = Color.black;

        hovering = true;

        if(Input.GetMouseButtonDown(0)){

            yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
            if(hovering){

                switch(index){

                    case 0:
                        sr.color = Color.green;
                        break;
                    case 1:
                        foreach(SpriteRenderer saveSR in saveDots){

                            saveSR.color = Color.white;
                        }
                        break;
                }

                if(background != null)
                    background.color = Color.black;
                if(label != null)
                    label.color = Color.white;
            }
        }
        if(Input.GetMouseButtonDown(1)){

            yield return new WaitUntil(() => Input.GetMouseButtonUp(1));
            if(hovering){

                if(index == 0)
                    sr.color = Color.red;
            }
        }
    }

    void OnMouseExit(){

        hovering = false;
        if(background != null)
            background.color = Color.black;
        if(label != null)
            label.color = Color.white;
    }
}
