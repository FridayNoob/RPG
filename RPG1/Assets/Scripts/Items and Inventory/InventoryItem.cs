using System;

[Serializable]
public class InventoryItem
{
    //仓库物品，表示放在仓库的物品，主要由物品数据和物品数量组成
    public ItemData data;
    public int stackSize;

    public InventoryItem(ItemData _newItemData)
    {
        data = _newItemData;
        AddStack();
    }

    public void AddStack() => stackSize++;

    public void RemoveStack() => stackSize--;

} 
