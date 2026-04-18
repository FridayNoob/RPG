using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveManager
{
    //一种接口：包含加载和存储游戏两种方法声明，任何需要保存数据的类需要实现本接口
    void LoadData(GameData _gameData);

    void SaveData(ref GameData _gameData);
}
