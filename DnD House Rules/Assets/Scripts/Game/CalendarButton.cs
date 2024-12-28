using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CalendarButton : MonoBehaviour
{
    CalendarUI calendarUI;
    public int index;

    User user;

    [SerializeField] Image background;
    [SerializeField] TMP_Text label;

    [SerializeField] GameObject bottomEdge;

    void Awake(){

        
        calendarUI = GameObject.Find("Calendar UI").GetComponent<CalendarUI>();

        if(background == null || label == null || bottomEdge == null){

            background = transform.GetChild(0).GetComponent<Image>();
            label = transform.GetChild(5).GetComponent<TMP_Text>();

            bottomEdge = transform.GetChild(4).gameObject;
        }
    }

    void Start(){

        user = GameObject.FindObjectOfType<User>();
    }

    void OnMouseOver(){

        if(!bottomEdge.activeInHierarchy && index!=-1)
            return;

        background.color = Color.white;
        label.color = Color.black;


        if(Input.GetMouseButtonUp(0) && index!=-1){

            calendarUI.OpenTab(index);
            background.color = Color.black;
            label.color = Color.white;
        }
        else if(Input.GetMouseButtonUp(0) && index==-1){

            GameManager.Singleton.terminal.SetActive(true);
            user.screen.SetActive(false);
        }

    }

    void OnMouseExit(){

        background.color = Color.black;
        label.color = Color.white;
    }
}
