using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ArmorSlot : MonoBehaviour
{
    public TMP_Text itemName, bonus;
    [SerializeField] RawImage background;

    public Description description;
    public int index;

    Camera cam;

    User user;

    void Start(){

        user = GameObject.Find(GameManager.Singleton.interpreter.GetUsername).GetComponent<User>();
        cam = user.transform.GetChild(0).GetComponent<Camera>();
    }

    void OnMouseOver(){

        background.color = Color.white;
        itemName.color = Color.black;
        bonus.color = Color.black;

        if(Input.GetMouseButtonDown(1) && Input.GetKey(KeyCode.LeftShift)){

            item thisItem = description.bodypart.slot[index].item;
            // instantiate a new gameobject that follows the mouse
            NetworkObject fake = Instantiate(GameManager.Singleton.itemDisplayBoxMouse, transform.parent.parent.parent.parent.parent.GetChild(3));
            ItemDisplayBoxMouse fakeDisplay = fake.GetComponent<ItemDisplayBoxMouse>();
            fakeDisplay.nameText.text = thisItem.name.ToString();
            fakeDisplay.sizeText.text = $"{thisItem.amount}{thisItem.size}";
            fakeDisplay.weightText.text = $"{thisItem.weight} Lbs.";
            fake.transform.localScale *= 1.1f;
            transform.GetChild(0).gameObject.SetActive(false);
            user.backpack.RunArmorSlotLogic(cam, fake, thisItem, this);
        }
    }
    void OnMouseExit(){

        background.color = Color.black;
        itemName.color = Color.white;
        bonus.color = Color.white;
    }
}
