using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour,ISaveManager
{
    private Transform playerTransform;

    //所有的检查点对象
    [SerializeField] private Checkpoint[] checkpoints;
    //单例
    public static GameManager instance;
    //存储距离玩家最近的检查点
    [SerializeField] private string closestCheckpointID;

    [Header("Lost currency")]
    [SerializeField] private GameObject lostCurrencyPrefab;
    public int lostCurrencyAmount;
    [SerializeField] private float lostCurrencyX;
    [SerializeField] private float lostCurrencyY;
    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    private void Start()
    {
        checkpoints = FindObjectsOfType<Checkpoint>();
        playerTransform = PlayerManager.instance.player.transform;
    }

    public void RestartGame()
    {
        //重新加载游戏
        SaveManager.instance.SaveGame();
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void LoadData(GameData _gameData)
    {
        //加载游戏时，只要存档点状态为true，就将它激活
        StartCoroutine(LoadWithDelay(_gameData));
    }

    private void LoadCheckpoints(GameData _gameData)
    {
        foreach (KeyValuePair<string, bool> pair in _gameData.checkpoints)
        {
            foreach (Checkpoint checkpoint in checkpoints)
            {
                if (pair.Key == checkpoint.id && pair.Value == true)
                {
                    checkpoint.ActivateCheckpoint();
                }
            }
        }
    }

    private void LoadLostCurrency(GameData _gameData)
    {
        //加载灵魂掉落物
        lostCurrencyAmount = _gameData.lostCurrencyAmount;
        lostCurrencyX = _gameData.lostCurrencyX;
        lostCurrencyY = _gameData.lostCurrencyY;

        if(lostCurrencyAmount > 0)
        {
            GameObject newLostCurrency = Instantiate(lostCurrencyPrefab, new Vector3(lostCurrencyX, lostCurrencyY), Quaternion.identity);
            newLostCurrency.GetComponent<LostCurrencyController>().currency = lostCurrencyAmount;
        }
        //生成掉落物后，用于下次生成掉落物的变量重置为0，避免重复生成某个数量的掉落物
        lostCurrencyAmount = 0;
    }

    private IEnumerator LoadWithDelay(GameData _gameData)
    {
        yield return new WaitForSeconds(.1f);
        //该方法如果在Start方法之前执行（在其他脚本中被调用），会导致存档点数据加载异常
        LoadCheckpoints(_gameData);
        LoadClosestCheckpoint(_gameData);
        LoadLostCurrency(_gameData);
    }

    public void SaveData(ref GameData _gameData)
    {
        _gameData.lostCurrencyAmount = lostCurrencyAmount;
        _gameData.lostCurrencyX = playerTransform.position.x;
        _gameData.lostCurrencyY = playerTransform.position.y;

        _gameData.checkpoints.Clear();


        foreach(Checkpoint checkpoint in checkpoints)
        {
            _gameData.checkpoints.Add(checkpoint.id, checkpoint.activationStatus);
        }
        if(FindClosestCheckpoint() != null)
            _gameData.closestCheckpointID = FindClosestCheckpoint().id;
    }
    private void LoadClosestCheckpoint(GameData _gameData)
    {
        //玩家进入游戏时处于最近的存档点位置
        if (_gameData.closestCheckpointID == null)
            return;

        closestCheckpointID = _gameData.closestCheckpointID;
        foreach (Checkpoint checkpoint in checkpoints)
        {
            if (closestCheckpointID == checkpoint.id)
            {
                playerTransform.position = checkpoint.transform.position;
            }
        }
    }

    private Checkpoint FindClosestCheckpoint()
    {
        //返回距离玩家最近的存档点对象
        float closestDistance = Mathf.Infinity;
        Checkpoint closestCheckpoint = null;

        foreach(Checkpoint checkpoint in checkpoints)
        {
            float distanceToCheckpoint = Vector2.Distance(playerTransform.position, checkpoint.transform.position);

            if(distanceToCheckpoint < closestDistance)
            {
                closestDistance = distanceToCheckpoint;
                closestCheckpoint = checkpoint;
            }
        }
        return closestCheckpoint;
    }

    public void PauseGame(bool _pause)
    {
        //暂停游戏
        if (_pause)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }
}
