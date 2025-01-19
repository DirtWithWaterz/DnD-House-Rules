using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CalendarButton : MonoBehaviour
{
    CalendarUI calendarUI;
    public int index;
    bool hovering;

    User user;

    [SerializeField] Image background;
    [SerializeField] TMP_Text label;

    [SerializeField] GameObject bottomEdge;

    void Awake(){

        
        calendarUI = transform.parent.GetComponent<CalendarUI>();

        if(background == null || label == null || bottomEdge == null){

            background = transform.GetChild(0).GetComponent<Image>();
            label = transform.GetChild(5).GetComponent<TMP_Text>();

            bottomEdge = transform.GetChild(4).gameObject;
        }
    }

    void Start(){

        user = transform.root.GetComponent<User>();
    }

    IEnumerator OnMouseOver(){
        if(!user.isInitialized.Value)
            yield break;
        if(!bottomEdge.activeInHierarchy && index!=-1)
            yield break;

        background.color = Color.white;
        label.color = Color.black;

        hovering = true;

        if(Input.GetMouseButtonDown(0)){

            yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
            if(hovering && index!=-1){

                calendarUI.OpenTab(index);
                background.color = Color.black;
                label.color = Color.white;
                // user.UpdateUserDataRpc(NetworkManager.Singleton.LocalClientId);
            }
            else if(hovering && index==-1){

                GameManager.Singleton.terminal.transform.GetChild(0).gameObject.SetActive(true);
                user.screen.GetComponent<Canvas>().enabled = false;
                user.transform.position += Vector3.up*100;
            }
        }
    }

    void OnMouseExit(){

        hovering = false;
        background.color = Color.black;
        label.color = Color.white;
    }
}
