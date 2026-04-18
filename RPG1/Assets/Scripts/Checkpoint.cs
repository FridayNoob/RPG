using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    //保存存档点物体动画
    private Animator anim;
    //存档点唯一GUID
    public string id;
    //存档点是否被激活
    public bool activationStatus;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //只要玩家经过，就激活存档点
        if (collision.GetComponent<Player>() != null)
            ActivateCheckpoint();
    }

    public void ActivateCheckpoint()
    {
        if (activationStatus)
            return;

        AudioManager.instance.PlaySFX(8, transform);
        //激活存档点
        anim.SetBool("Active", true);
        activationStatus = true;
    }
    [ContextMenu("Generate checkpoint id")]
    public void GenerateID()
    {
        //生成唯一的GUID
        id = System.Guid.NewGuid().ToString();
    }
}
