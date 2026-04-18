using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSound : MonoBehaviour
{
    [SerializeField] private int areaSoundIndex;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //当玩家进入某个区域时会播放环境音
        if(collision.GetComponent<Player>() != null)
        {
            AudioManager.instance.PlaySFX(areaSoundIndex, null);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            if(AudioManager.instance != null)
                AudioManager.instance.StopWithTime(areaSoundIndex);
        }
    }
}
