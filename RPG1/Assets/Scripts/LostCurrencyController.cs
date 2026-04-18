using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LostCurrencyController : MonoBehaviour
{
    //掉落的灵魂点数量
    public int currency;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //玩家经过时捡起掉落的灵魂点
        if(collision.GetComponent<Player>() != null)
        {
            PlayerManager.instance.currency += this.currency;
            Destroy(this.gameObject);
        }
    }
}
