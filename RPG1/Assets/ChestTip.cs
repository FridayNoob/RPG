using UnityEngine;
using TMPro; // 必须引用TextMeshPro的命名空间

public class ChestTip : InteractiveItem
{
    private ItemDrop dropSystem;

    protected override void Start()
    {
        base.Start();
        dropSystem = GetComponent<ItemDrop>();
    }
    protected override void Execute()
    {
        // 1. 打开后隐藏提示，防止重复触发
        tipText.gameObject.SetActive(false);
        isPlayerNear = false;

        // 2. 👇 这里写你自己的宝箱打开逻辑：
        Debug.Log("宝箱已打开！");
        // 播放宝箱打开动画（如果有Animator组件）
        isUseful = false;
        GetComponent<Animator>().SetBool("IsOpen", !isUseful);
        dropSystem.GenerateDrop();
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