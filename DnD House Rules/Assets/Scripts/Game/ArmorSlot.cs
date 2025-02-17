using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ArmorSlot : MonoBehaviour
{
    public TMP_Text itemName, bonus;
    public RawImage background;

    public bool hovering, selected;

    public Description description;
    public int index;

    public User user;

    void OnMouseOver(){

        hovering = true;
        background.color = Color.white;
        itemName.color = Color.black;
        bonus.color = Color.black;

        if(Input.GetMouseButtonDown(0)){

            GameManager.Singleton.SaveDataRpc();
            item thisItem = description.bodypart.slot[index].item;
            if(thisItem.id == -1)
                return;
            user.backpack.RunArmorSlotLogic(thisItem, 1, this);
        }

        if(Input.GetMouseButtonDown(1) && Input.GetKey(KeyCode.LeftShift)){

            GameManager.Singleton.SaveDataRpc();
            item thisItem = description.bodypart.slot[index].item;
            if(thisItem.id == -1)
                return;
            // instantiate a new gameobject that follows the mouse
            NetworkObject fake = Instantiate(GameManager.Singleton.itemDisplayBoxMouse, transform.parent.parent.parent.parent.parent.GetChild(3));
            ItemDisplayBoxMouse fakeDisplay = fake.GetComponent<ItemDisplayBoxMouse>();
            fakeDisplay.nameText.text = thisItem.name.ToString();
            fakeDisplay.sizeText.text = $"{thisItem.amount}{thisItem.size}";
            fakeDisplay.weightText.text = $"{thisItem.weight} Lbs.";
            fake.transform.localScale *= 1.1f;
            transform.GetChild(0).gameObject.SetActive(false);
            fake.transform.GetChild(0).gameObject.SetActive(true);
            user.backpack.RunArmorSlotLogic(thisItem, 0, this, fake);
        }
    }
    void OnMouseExit(){

        if(!selected){

            hovering = false;
            background.color = Color.black;
            itemName.color = Color.white;
            bonus.color = Color.white;
        }
    }
}
