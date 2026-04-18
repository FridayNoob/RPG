using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject_Trigger : MonoBehaviour
{
    private ItemObject myItemObject => GetComponentInParent<ItemObject>();
    private bool isPlayerInRange = false;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && isPlayerInRange)
        {
            if (myItemObject == null)
            {
                Debug.LogError("ItemObject_Trigger: myItemObject is null!");
                return;
            }
            myItemObject.PickUpItem();
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isPlayerInRange = true;

    }



    private void OnTriggerExit2D(Collider2D collision)
    {
        isPlayerInRange = false;
    }
}
