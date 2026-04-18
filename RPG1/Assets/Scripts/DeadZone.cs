using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //如果是敌人或者玩家掉落，就杀死敌人/玩家
        //如果是物品掉落，直接销毁物品
        if (collision.GetComponent<CharacterStats>())
            collision.GetComponent<CharacterStats>().KillEntity();
        else
            Destroy(collision.gameObject);
    }
}
