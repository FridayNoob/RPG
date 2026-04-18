using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    //保存游戏：Manger的数据=》GameData=》存档文件
    //加载游戏：存档文件=》GameData=》Manager的数据

    //存档文件名称
    [SerializeField] private string fileName;
    //是否对存档进行加密
    [SerializeField] private bool encryptData;
    //游戏数据对象
    private GameData gameData;
    //单例模式中本类的实例对象
    public static SaveManager instance;
    //需要保存数据的对象
    private List<ISaveManager> saveManagers;
    //与文件系统打交道的对象
    private FileDataHandler dataHandler;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    private void Start()
    {
        //获取需要保存数据的Manager对象
        saveManagers = FindAllSaveManager();
        //指定存档位置和存档文件名
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);
        //游戏开始时加载文档
        LoadGame();
    }

    [ContextMenu("Delete save file")]
    public void DeleteSaveData()
    {
        //删除存档文件
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);
        dataHandler.Delete();
    }
    public void NewGame()
    {
        gameData = new GameData();
    }

    public void LoadGame()
    {
        //用存档文件初始化GameData对象
        gameData = dataHandler.Load();

        if (gameData == null)
        {
            Debug.Log("No saved data found!");
            NewGame();
        }
        //使用GameData对象初始化仓库、技能等物体
        foreach(ISaveManager saveManager in saveManagers)
        {
            saveManager.LoadData(gameData);
        }

    }

    public void SaveGame()
    {
        //先从各个Manager收集数据保存到GameData对象
        foreach(ISaveManager saveManager in saveManagers)
        {
            saveManager.SaveData(ref gameData);
        }
        //再将GameData对象序列化保存为文档，存到磁盘中
        dataHandler.Save(gameData);

        //foreach(KeyValuePair<string, bool> pair in gameData.skillTree)
        //{
        //    Debug.Log(pair.Key + "  //   " +  pair.Value);
        //}
    }

    private void OnApplicationQuit()
    {
        //游戏退出时自动保存
        SaveGame();
    }

    private List<ISaveManager> FindAllSaveManager()
    {
        //查找所有继承了MonoBehavior类并且实现了自定义接口ISaveManager的类的对象
        IEnumerable<ISaveManager> saveManagers = FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveManager>();

        return new List<ISaveManager>(saveManagers);
    }

    public bool HasSavedData()
    {
        //判断是否有存档
        if (dataHandler.Load() != null)
        {
            return true;
        }
        return false;
    }
}
