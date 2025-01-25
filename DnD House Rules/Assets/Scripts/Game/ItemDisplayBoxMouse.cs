using TMPro;
using UnityEngine;

public class ItemDisplayBoxMouse : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text sizeText;
    public TMP_Text weightText;

    Camera cam;

    void Start(){

        User user = GameObject.Find(GameManager.Singleton.interpreter.GetUsername).GetComponent<User>();
        cam = user.transform.GetChild(0).GetComponent<Camera>();
    }
    void Update(){

        transform.position = cam.ScreenToWorldPoint(Input.mousePosition);
        if(!Input.GetKey(KeyCode.LeftShift))
            transform.localPosition = new Vector3(90f, transform.localPosition.y, 0);
        else
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
    }
}
