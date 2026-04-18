using System.Text;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif

public enum ItemType
{
    Material,
    Equipment
}

[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Item")]
public class ItemData : ScriptableObject
{
    //物品数据，主要由物品类型、物品名称和物品图标组成
    public ItemType itemType;
    public string itemName;
    public Sprite icon;
    public string itemID;

    [Range(0, 100)]
    public float dropChance;

    protected StringBuilder sb = new StringBuilder();

    public virtual string GetDescription()
    {
        return "";
    }

    private void OnValidate()
    {
#if UNITY_EDITOR
        string path = AssetDatabase.GetAssetPath(this);
        itemID = AssetDatabase.AssetPathToGUID(path);
#endif
    }
}
