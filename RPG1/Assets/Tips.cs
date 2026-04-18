using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class Tips : MonoBehaviour
{
    [Header("UI配置预制体")]
    public TextMeshProUGUI tipTextPrefab;
    [Header("UI画布")]
    public Canvas uiCanvas;

    // 每个物体独立的UI文本
    private TextMeshProUGUI tipText = null;
    [Tooltip("UI显示文本")]
    public string tipMessage = "";

    void Start()
    {
        // 只在启动时初始化一次UI
        InitUI();
    }

    private void OnValidate()
    {
        UpdateTipContentAndPos();
    }

    // Update只负责更新位置和文本，不重复创建UI
    private void Update()
    {
        UpdateTipContentAndPos();
    }

    // 初始化：只执行一次（创建UI文本）
    private void InitUI()
    {
        if (tipText == null)
        {
            // 生成独立的UI文本，每个物体一个
            tipText = Instantiate(tipTextPrefab, uiCanvas.transform);
        }
        tipText.gameObject.SetActive(true);
    }

    // 每帧更新：文本内容 + 屏幕位置
    private void UpdateTipContentAndPos()
    {
        if (tipText == null) return;
        
        // 赋值当前物体自己的tipMessage（独立！）
        tipText.text = tipMessage;

        // 转换世界坐标到屏幕坐标
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        tipText.transform.position = screenPos;
    }

    // 物体销毁时，同步销毁自己的UI文本
    private void OnDestroy()
    {
        if (tipText != null)
        {
            Destroy(tipText.gameObject);
        }
    }
}