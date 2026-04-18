using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemDrop : ItemDrop
{
    [Header("Player's drop")]
    [SerializeField] private float chanceToLoseItem;
    [SerializeField] private float chanceToLoseItems;

    public override void GenerateDrop()
    {
        Inventory inventory = Inventory.instance;

        List<InventoryItem> itemsToUnequip = new List<InventoryItem>();
        List<InventoryItem> materialsToRemove = new List<InventoryItem>();

        foreach(InventoryItem item in inventory.GetEquimentList())
        {
            if (Random.Range(0, 100) < chanceToLoseItem)
            {
                DropItem(item.data);
                itemsToUnequip.Add(item);
            }
        }

        for(int i = 0; i < itemsToUnequip.Count; i++)
        {
            inventory.UnequipItem(itemsToUnequip[i].data as ItemData_Equipment);
        }

        foreach(InventoryItem item in inventory.GetStashList())
        {
            DropItem(item.data);
            materialsToRemove.Add(item);
        }
        for(int i = 0; i < materialsToRemove.Count; i++)
        {
            inventory.RemoveItem(materialsToRemove[i].data);
        }
    }
}
