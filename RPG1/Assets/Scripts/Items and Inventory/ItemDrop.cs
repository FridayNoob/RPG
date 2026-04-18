using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    //控制敌人死亡后物品掉落
    //描述掉落几率和可能掉落物品
    [SerializeField] private int possibleItemDrop;
    [SerializeField] private ItemData[] possibleDrop;
    private List<ItemData> dropList = new List<ItemData>();

    [SerializeField] private GameObject dropPrefab;

    public virtual void GenerateDrop()
    {
        //根据掉落几率生成掉落物品列表
        for (int i = 0; i < possibleDrop.Length; i++)
        {
            if (Random.Range(0, 100) <= possibleDrop[i].dropChance)
                dropList.Add(possibleDrop[i]);
        }
        //从掉落物品列表中随机选择物品进行掉落，直到达到可能掉落物品数量或掉落物品列表为空
        for (int i = 0; i < possibleItemDrop; i++)
        {
            //如果掉落物品列表为空，则随机放一件物品
            if (dropList.Count <= 0)
            {
                Debug.LogWarning("DropList is empty, dropping random item from possibleDrop.");
                DropItem(possibleDrop[Random.Range(0, possibleDrop.Length - 1)]);
                continue;
            }

            ItemData randomItem = dropList[Random.Range(0, dropList.Count - 1)];
            dropList.Remove(randomItem);
            DropItem(randomItem);

            if (dropList.Count <= 0)
                break;
        }
    }

    protected void DropItem(ItemData item)
    {
 
        GameObject newDrop = Instantiate(dropPrefab, transform.position, Quaternion.identity);
        
        Vector2 randomVelocity = new Vector2(Random.Range(-5, 5), Random.Range(15, 20));

        newDrop.GetComponent<ItemObject>().SetupItem(item, randomVelocity);
    }
}
