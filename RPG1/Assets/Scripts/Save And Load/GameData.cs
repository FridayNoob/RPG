using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class GameData
{
    //保存游戏当前状态的所有数据的类

    //玩家货币/灵魂点
    public int currency;
    //玩家库存：<UID，库存数>
    public SerializableDictionary<string, int> inventory;
    //装备栏
    public List<string> equipmentID;

    //学会的技能
    public SerializableDictionary<string, bool> skillTree;

    //存档点
    public SerializableDictionary<string, bool> checkpoints;
    //距离玩家最近的存档点
    public string closestCheckpointID;

    //玩家掉落
    public int lostCurrencyAmount;
    public float lostCurrencyX;
    public float lostCurrencyY;

    //玩家音量设置
    public SerializableDictionary<string, float> volumnSettings;
    public GameData()
    {
        this.lostCurrencyAmount = 0;
        this.lostCurrencyX = 0;
        this.lostCurrencyY = 0;

        this.currency = 0;
        inventory = new SerializableDictionary<string, int>();
        equipmentID = new List<string>();
        skillTree = new SerializableDictionary<string, bool>();
        checkpoints = new SerializableDictionary<string, bool>();
        closestCheckpointID = string.Empty;
        volumnSettings = new SerializableDictionary<string, float>();
    }
}
