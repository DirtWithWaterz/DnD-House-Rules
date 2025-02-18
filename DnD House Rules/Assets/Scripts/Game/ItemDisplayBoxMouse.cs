using TMPro;
using UnityEngine;

public class ItemDisplayBoxMouse : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text sizeText;
    public TMP_Text weightText;

    Camera cam;

    void Awake(){

        transform.GetChild(0).gameObject.SetActive(false);
    }
    
    void Start(){

        User user = GameObject.Find(GameManager.Singleton.interpreter.GetUsername).GetComponent<User>();
        cam = user.transform.GetChild(0).GetComponent<Camera>();
        transform.position = cam.ScreenToWorldPoint(Input.mousePosition);
        if(!Input.GetKey(KeyCode.LeftShift))
            transform.localPosition = new Vector3(90f, transform.localPosition.y, 0);
        else
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
        // Debug.Log("Fake was instantiated");
    }
    void Update(){

        transform.position = cam.ScreenToWorldPoint(Input.mousePosition);
        if(!Input.GetKey(KeyCode.LeftShift))
            transform.localPosition = new Vector3(90f, Mathf.Clamp(transform.localPosition.y, -80.5f, 80.5f), 0);
        else
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
    }
}
