using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Door : InteractiveItem
{
    [Header("要切换的场景名")]
    [SerializeField]
    private string sceneName = "";
    // 跳关逻辑
    protected override void Execute()
    {
        tipText.gameObject.SetActive(false);
        isPlayerNear = false;
        AsyncSceneLoadManager.instance.LoadGameScene(sceneName);
        Debug.Log("进入下一关");

        
        // 播放宝箱打开动画（如果有Animator组件）
        
        // 示例2：生成道具/金币
        // Instantiate(itemPrefab, transform.position, Quaternion.identity);
        // 示例3：禁用碰撞体，防止重复打开
        // GetComponent<Collider2D>().enabled = false;
        // 示例4：播放打开音效
        // GetComponent<AudioSource>().Play();
    }

    // --- 可选：新Input System适配（如果用Unity新输入系统）---
    // 把Update里的Input.GetKeyDown替换成下面的代码：
    // using UnityEngine.InputSystem; // 顶部添加命名空间
    // if (Keyboard.current.eKey.wasPressedThisFrame)
    // {
    //     OpenChest();
    // }


}
