using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AsyncSceneLoadManager : MonoBehaviour
{
    public static AsyncSceneLoadManager instance;

    public Slider loadingSlider; // 加载进度条（UI Slider）
    public TextMeshProUGUI progressText;    // 进度百分比文本（UI Text）

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        HideUI();
    }
    // 异步加载场景的核心方法,暴露给外部调用
    public void LoadSceneAsync(string sceneName)
    {
        // 开启协程执行异步加载（Unity中异步加载必须用协程/回调）
        ShowUI();
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    // 协程处理异步加载逻辑
    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        // 开始异步加载，返回AsyncOperation对象（可获取加载进度）
        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName);
        // 禁止加载完成后自动切换场景（可选，手动控制切换时机）
        asyncOp.allowSceneActivation = false;

        // 循环获取加载进度，直到加载完成
        while (!asyncOp.isDone)
        {
            // 进度值范围：0~0.9（0.9代表加载完成，1.0代表切换场景）
            float progress = Mathf.Clamp01(asyncOp.progress / 0.9f);
            // 更新UI进度
            loadingSlider.value = progress;
            progressText.text = $"{Mathf.Round(progress * 100)}%";

            // 加载完成后（进度到0.9），手动允许切换场景
            if (asyncOp.progress >= 0.9f)
            {
                loadingSlider.value = 1f;
                progressText.text = "100% - 准备进入场景";
                // 可添加延迟/确认按钮，再设置为true
                asyncOp.allowSceneActivation = true;
            }

            yield return null; // 等待下一帧，避免卡死
        }

        Debug.Log($"场景 {sceneName} 加载完成并切换");
    }

    // 示例：挂载到按钮点击事件
    public void LoadGameScene(string sceneName)
    {
        LoadSceneAsync(sceneName); // 替换为你的场景名称
    }

    private void ShowUI()
    {
        loadingSlider.enabled = true;
        progressText.enabled = true;
    }

    private void HideUI()
    {
        loadingSlider.enabled = false;
        progressText.enabled = false;
    }
}
