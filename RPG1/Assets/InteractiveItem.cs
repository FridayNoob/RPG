using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractiveItem : MonoBehaviour
{
    [Header("UI配置")]

    [Header("UI配置预制体")]
    public TextMeshProUGUI tipTextPrefab;
    [Header("UI画布")]
    public Canvas uiCanvas;
    [Header("UI显示文本")]
    public string tipMessage = "";

    protected TextMeshProUGUI tipText = null;
    [Header("UI相对于传送门的屏幕偏移（向上50像素）")]
    public Vector2 uiOffset = new Vector2(0, 50f);
    [Header("字体大小")]
    public int fondSize = 36;
    [Header("交互配置")]
    [Tooltip("交互按键，默认E")]
    public KeyCode openKey = KeyCode.E;
    [Tooltip("玩家的Tag，确保玩家对象Tag一致")]
    public string playerTag = "Player";

    // 内部状态：玩家是否在附近、缓存玩家Transform
    protected bool isPlayerNear = false;
    protected Transform playerTransform;

    public bool isUseful = true; // 是否可用

    protected virtual void Start()
    {
        // 只在启动时初始化一次UI
        InitUI();
    }

    private void InitUI()
    {
        if (tipText == null)
        {
            // 生成独立的UI文本，每个物体一个
            tipText = Instantiate(tipTextPrefab, uiCanvas.transform);
        }
        tipText.gameObject.SetActive(true);
    }

    protected void UpdateTipContentAndPos()
    {
        if (tipText == null) return;
        tipText.gameObject.SetActive(true);
        // 赋值当前物体自己的tipMessage（独立！）
        tipText.text = tipMessage;
        tipText.fontSize = fondSize;
        // 转换世界坐标到屏幕坐标
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        
        tipText.transform.position = screenPos + (Vector3)uiOffset;
    }

    // 当玩家进入触发区域
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isUseful == false) return;
        // 仅当进入的是玩家时，触发逻辑
        if (other.CompareTag(playerTag))
        {

            if (tipText == null)
            {
                Debug.LogError("tipText is null!");
                return;
            }


            isPlayerNear = true;
            playerTransform = other.transform;
            // 显示提示UI
            tipText.gameObject.SetActive(true);

        }
    }

    // 当玩家离开宝箱触发区域
    private void OnTriggerExit2D(Collider2D other)
    {

        if (other.CompareTag(playerTag))
        {
            isPlayerNear = false;
            playerTransform = null;
            // 隐藏提示UI
            tipText.gameObject.SetActive(false);
        }
    }

    // 每帧更新：处理UI位置跟随、按键检测
    private void Update()
    {
        // 玩家在附近时，更新UI位置，让提示始终在宝箱上方
        if (isPlayerNear && isUseful == true)
        {
            UpdateTipContentAndPos();

            // 检测E键按下
            if (Input.GetKeyDown(openKey))
            {
                Execute();
            }
        }
    }

    // 跳关逻辑
    virtual protected void Execute()
    {
        tipText.gameObject.SetActive(false);
        isPlayerNear = false;

        Debug.Log("执行交互");
    }
}
