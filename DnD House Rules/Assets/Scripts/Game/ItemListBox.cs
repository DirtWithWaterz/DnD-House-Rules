using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ItemListBox : MonoBehaviour
{
    public TMP_Text itemName, weight, amount;
    public RawImage background;
    public bool hovering, selected;
    public User user;

    public ItemList itemList;
    public int index;

    void OnMouseOver(){

        hovering = true;
        background.color = Color.white;
        itemName.color = Color.black;
        weight.color = Color.black;
        amount.color = Color.black;

        if(Input.GetMouseButtonDown(0)){

            GameManager.Singleton.SaveDataRpc();
            item thisItem = itemList.getItems()[index];
            if (thisItem.id == -1)
                return;
            user.backpack.RunItemListLogic(thisItem, 1, this);
        }

        if (Input.GetMouseButtonDown(1) && Input.GetKey(KeyCode.LeftShift))
        {

            GameManager.Singleton.SaveDataRpc();
            item newItem = itemList.getItems()[index];

            item thisItem = new item
            {
                name = newItem.name.ToString() + (newItem.type == Type.backpack ? GameManager.Singleton.itemIdTally.Value : "") + " ",
                cost = newItem.cost,
                value = newItem.value,
                type = newItem.type,
                size = newItem.size,
                amount = newItem.amount,
                weight = newItem.weight,
                itemInventory = $"/{user.name} {newItem.name}{GameManager.Singleton.itemIdTally.Value} {GameManager.Singleton.itemIdTally.Value} Inventory.json",
                id = GameManager.Singleton.itemIdTally.Value,
                equippable = newItem.equippable,
                isEquipped = false,
                bodyparts = newItem.bodyparts,
                metadata = newItem.metadata
            };

            if (thisItem.id == -1)
                return;
            // instantiate a new gameobject that follows the mouse
            NetworkObject fake = Instantiate(GameManager.Singleton.itemDisplayBoxMouse, transform.root.GetChild(1));
            ItemDisplayBoxMouse fakeDisplay = fake.GetComponent<ItemDisplayBoxMouse>();
            fakeDisplay.nameText.text = thisItem.name.ToString();
            fakeDisplay.sizeText.text = $"{thisItem.amount}{thisItem.size}";
            fakeDisplay.weightText.text = $"{thisItem.weight} Lbs.";
            fake.transform.localScale *= 1.1f;
            fake.name = "fake";
            transform.GetChild(0).gameObject.SetActive(false);
            fake.transform.GetChild(0).gameObject.SetActive(true);
            user.backpack.RunItemListLogic(thisItem, 0, this, fake);
        }
    }
    void OnMouseExit(){

        if(!selected){

            hovering = false;
            background.color = Color.black;
            itemName.color = Color.white;
            weight.color = Color.white;
            amount.color = Color.white;
        }
    }
}
