using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestBag : MonoBehaviour
{
    public static TestBag instance;
    [Header("测试用的仓库物品data")]
    public ItemData testItemDataPrefab;
    [Header("生成物品数量")]
    public int objCount = 30;
    private void Awake()
    {
        //保证只有一个Inventory物体，对应一个Inventory脚本
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

   
    [ContextMenu("生成测试物品")]
    public void Generate()
    {
        //StartCoroutine(GenerateItems());
        GenerateItemsNow();
    }

    void GenerateItemsNow()
    {
        Sprite[] allSprites = Resources.LoadAll<Sprite>("Graphics/Icons/BG 6");
        for (int i = 0; i < objCount; i++)
        {
            ItemData newItemData = Instantiate(testItemDataPrefab);

            newItemData.icon = allSprites[i % allSprites.Length];
            Inventory.instance.AddItem(newItemData);
            
        }
    }

    IEnumerator GenerateItems()
    {
        for (int i = 0; i < objCount; i++)
        {
            ItemData newItemData = Instantiate(testItemDataPrefab);
            Inventory.instance.AddItem(newItemData);
            yield return null;
        }
    }
}
