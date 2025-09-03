using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabButton : MonoBehaviour
{
    bool hovering;
    User user;

    bool index;

    [SerializeField] Image background;
    [SerializeField] TMP_Text label;

    [SerializeField] GameObject descT;
    [SerializeField] GameObject skillT;

    // [SerializeField] Collider2D coll;

    // Start is called before the first frame update
    void Start()
    {
        user = transform.root.GetComponent<User>();

        skillT.GetComponent<RectTransform>().localPosition += Vector3.right * 1000;
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
            if (hovering)
            {

                switch (index)
                {

                    case true:

                        descT.GetComponent<RectTransform>().localPosition -= Vector3.right * 1000;
                        skillT.GetComponent<RectTransform>().localPosition += Vector3.right * 1000;
                        label.text = "OPEN SKILL TAB";
                        index = !index;
                        break;
                    case false:

                        skillT.GetComponent<RectTransform>().localPosition -= Vector3.right * 1000;
                        descT.GetComponent<RectTransform>().localPosition += Vector3.right * 1000;
                        label.text = "CLOSE SKILL TAB";
                        index = !index;
                        break;
                }

                if (background != null)
                    background.color = Color.black;
                if (label != null)
                    label.color = Color.white;

                hovering = false;
                // Physics.SyncTransforms();
            }
        }
    }

    void OnMouseExit()
    {

        hovering = false;
        if(background != null)
            background.color = Color.black;
        if(label != null)
            label.color = Color.white;
    }
}
