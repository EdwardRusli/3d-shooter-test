using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemData itemData;
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            print(itemData.name);
            print("triggered");
            other.gameObject.GetComponent<PlayerController>().AddItem(itemData);
        }
    }
}
