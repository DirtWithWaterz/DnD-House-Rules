using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemList : NetworkBehaviour
{

    GameManager gameManager = GameManager.Singleton;

    public GameObject panel;

    public List<ItemListBox> displays;

    [Rpc(SendTo.Everyone)]
    public void displayRpc(string usernameI)
    {

        if (usernameI != transform.root.name)
            return;

        for (int i = 0; i < gameManager.items.Count; i++)
        {

            item item = gameManager.items[i];

            NetworkObject netObj = Instantiate(gameManager.itemListBox, panel.transform);

            ItemListBox listBox = netObj.GetComponent<ItemListBox>();

            listBox.itemName.text = item.name.ToString();
            listBox.weight.text = item.weight.ToString() + " Lbs.";
            listBox.amount.text = item.amount.ToString() + item.size.ToString();

            listBox.index = i;

            listBox.user = transform.root.GetComponent<User>();
            listBox.itemList = this;

            displays.Add(listBox);
        }
    }

    public NetworkList<item> getItems() {

        return gameManager.items;
    }
}
