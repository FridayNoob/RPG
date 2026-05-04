using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestBag : MonoBehaviour
{
    public static TestBag instance;
    [Header("测试用的stash物品data")]
    public ItemData testStashPrefab;
    [Header("生成stash物品数量")]
    public int stashObjCount = 30;

    [Header("测试用的inventory物品data")]
    public ItemData testInventoryPrefab;
    [Header("生成stash物品数量")]
    public int inventoryObjCount = 30;


    private void Awake()
    {
        //保证只有一个Inventory物体，对应一个Inventory脚本
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

   
    [ContextMenu("生成stash+inventory测试物品")]
    public void Generate()
    {
        //StartCoroutine(GenerateItems());
        StashGenerateBatch();
        InventoryGenerateBatch();
    }

    //void GenerateItemsNow()
    //{
    //    Sprite[] allSprites = Resources.LoadAll<Sprite>("Graphics/Icons/BG 6");
    //    for (int i = 0; i < stashObjCount; i++)
    //    {
    //        ItemData newItemData = Instantiate(testStashPrefab);

    //        newItemData.icon = allSprites[i % allSprites.Length];
    //        StopAllCoroutines();
    //        Inventory.instance.AddItem(newItemData);
            
    //    }
    //}

    /** <summary>
     * 批量生成物品(材料+道具装备)并添加到背包，减少UI刷新次数，提高性能
     * </summary>
     */
    public void StashGenerateBatch()
    {
        List<ItemData> list = new List<ItemData>(stashObjCount);
        Sprite[] allSprites = Resources.LoadAll<Sprite>("Graphics/Icons/BG 6");
        for (int i = 0; i < stashObjCount; i++)
        {
            ItemData newItemData = Instantiate(testStashPrefab);
            newItemData.icon = allSprites[i % allSprites.Length];
            list.Add(newItemData);
        }
        // 批量添加（一次性刷新 UI）
        if (Inventory.instance != null)
            Inventory.instance.AddItemsBatch(list);
        else
            Debug.LogError("Inventory.instance is null. Ensure Inventory exists and initialized before generating items.");
    }

    public void InventoryGenerateBatch()
    {
        List<ItemData> list = new List<ItemData>(inventoryObjCount);
        Sprite[] allSprites = Resources.LoadAll<Sprite>("Graphics/Icons/BG 6");
        for (int i = 0; i < inventoryObjCount; i++)
        {
            ItemData newItemData = Instantiate(testInventoryPrefab);
            newItemData.icon = allSprites[i % allSprites.Length];
            list.Add(newItemData);
        }
        // 批量添加（一次性刷新 UI）
        if (Inventory.instance != null)
            Inventory.instance.AddItemsBatch(list);
        else
            Debug.LogError("Inventory.instance is null. Ensure Inventory exists and initialized before generating items.");
    }

    //IEnumerator GenerateItems()
    //{
    //    for (int i = 0; i < stashObjCount; i++)
    //    {
    //        ItemData newItemData = Instantiate(testStashPrefab);
    //        Inventory.instance.AddItem(newItemData);
    //        yield return null;
    //    }
    //}
}
