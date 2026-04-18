using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    private Player player;
    protected override void Start()
    {
        base.Start();
        player = GetComponent<Player>();
    }
    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
        //Debug.Log(player.name +  "受到伤害: " + _damage);
    }

    protected override void Die()
    {
        base.Die();
        player.Die();
        GetComponent<PlayerItemDrop>()?.GenerateDrop();
        //玩家死亡后掉落一定数量灵魂点
        GameManager.instance.lostCurrencyAmount = PlayerManager.instance.currency;
        PlayerManager.instance.currency = 0;
    }

    protected override void DecreaseHealthBy(int _damage)
    {
        base.DecreaseHealthBy(_damage);

        //当敌人伤害大于玩家血量的30%时，玩家会被击退
        if(_damage > GetMaxHealthValue() * .3f)
        {
            player.SetupKnockbackPower(new Vector2(7, 10));
            Debug.Log("High damage take!");
            AudioManager.instance.PlaySFX(13, player.transform);
            player.fx.ScreenShake(player.fx.shakeHighDamage);
        }

        ItemData_Equipment currentArmor = Inventory.instance.GetEquipment(EquipmentType.Armor);

        currentArmor?.Effect(player.transform);
    }

    public override void OnEvasion()
    {
        player.skill.dodge.CreateMirageOnDodge();
    }

    public void CloneDoDamage(CharacterStats _targetStats, float _multiplier)
    {
        if (TargetCanAvoidAttack(_targetStats))
            return;

        int totalDamage = damage.GetValue() + strength.GetValue();

        if (_multiplier > 0)
            totalDamage = Mathf.RoundToInt(totalDamage * _multiplier);

        if (CanCrit())
        {
            totalDamage = CalculatingCriticalDamage(totalDamage);
            Debug.Log("暴击伤害：" + totalDamage);
        }

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);

        _targetStats.TakeDamage(totalDamage);
        DoMagicDamage(_targetStats);
    }
}
