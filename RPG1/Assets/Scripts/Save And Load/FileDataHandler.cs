using UnityEngine;
using System;
using System.IO;

public class FileDataHandler
{
    //沟通C#程序和文件系统文件的桥梁

    //文件目录路径（绝对）
    private string dataDirPath = "";
    //文件名
    private string dataFileName = "";

    //是否加密
    private bool encryptData = false;
    private string codeWord = "YOURARECOOL";

    public FileDataHandler(string _dataDirPath, string _dataFileName, bool _encryptData)
    {
        dataDirPath = _dataDirPath;
        dataFileName = _dataFileName;
        encryptData = _encryptData;
    }

    public void Save(GameData _gameData)
    {
        //将GameData对象中的数据转换为JSON格式保存为文件存档
        //指定存放位置
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            //将GameData对象转换为JSON字符串
            string dataToSave = JsonUtility.ToJson(_gameData, true);

            if (encryptData)
                dataToSave = EncryptDecrypt(dataToSave);

            using (FileStream fileStream = new FileStream(fullPath, FileMode.Create))
            {
                using(StreamWriter writer = new StreamWriter(fileStream))
                {
                    writer.Write(dataToSave);
                }
            }
        }
        catch(Exception e)
        {
            Debug.LogError("Error on tring to save data to file:" + fullPath + "\n" + e);
        }
    }

    public GameData Load()
    {
        //将存档数据读取到程序中，并将其转化为GameData对象进行返回
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        GameData loadData = null;

        try
        {
            string dataToLoad = "";

            using (FileStream fileStream = new FileStream(fullPath, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    dataToLoad = reader.ReadToEnd();
                }
            }
            //将JSON字符串转换为GameData对象
            if (encryptData)
                dataToLoad = EncryptDecrypt(dataToLoad);

            loadData = JsonUtility.FromJson<GameData>(dataToLoad);

        }
        catch(Exception e)
        {
            Debug.Log("Error on trying to laod data from:" + fullPath + "\n" + e);
        }

        return loadData;
    }

    public void Delete()
    {
        //删除存档
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }

    private string EncryptDecrypt(string _data)
    {
        //数据加密解密
        string modifiedData = "";

        for(int i = 0; i < _data.Length; i++)
        {
            modifiedData += (char)(_data[i] ^ codeWord[i % codeWord.Length]);
        }

        return modifiedData;
    }
}
