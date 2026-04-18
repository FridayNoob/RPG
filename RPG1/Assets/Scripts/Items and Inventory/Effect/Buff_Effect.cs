using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Buff effect", menuName = "Data/ Item effect/Buff effect")]
public class Buff_Effect : ItemEffect
{
    private PlayerStats playerStats;
    [SerializeField] private StatType buffType;
    [SerializeField] private int buffAmount;
    [SerializeField] private float buffDuration;

    public override void ExecuteEffect(Transform _enemyPosition)
    {
        playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
        playerStats.IncreaseStatBy(buffAmount, buffDuration, playerStats.GetStat(buffType));
    }

    
}
