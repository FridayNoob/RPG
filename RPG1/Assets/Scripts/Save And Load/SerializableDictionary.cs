using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    //将字典数据分别存储在两个列表中

    [SerializeField] private List<TKey> keys = new List<TKey>();
    [SerializeField] private List<TValue> values = new List<TValue>();

    public void OnBeforeSerialize()
    {
        //序列化之前将字典中的键值对拆开分别保存到键列表和值列表中
        keys.Clear();
        values.Clear();

        foreach(KeyValuePair<TKey, TValue> pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }
    public void OnAfterDeserialize()
    {
        //序列化之后把键列表和值列表中相应的键值对加入的字典中
        this.Clear();

        if(keys.Count != values.Count)
        {
            Debug.Log("Keys count not equals values count");
        }

        for(int i = 0; i < keys.Count; i++)
        {
            this.Add(keys[i], values[i]);
            //Debug.Log(keys[i] + "   --   " + values[i]);
        }
    }


}
