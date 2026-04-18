using UnityEngine;

public class EnemyStats : CharacterStats
{
    private Enemy enemy;
    private ItemDrop myDropSystem;

    public Stat soulsDropAmount;

    [Header("Level details")]
    [SerializeField] private int level = 1;
    [Range(0f, 1f)]
    [SerializeField] private float percentageModifer = .4f;
    protected override void Start()
    {
        soulsDropAmount.SetDefaultValue(100);
        ApplyLevelModifiers();
        myDropSystem = GetComponent<ItemDrop>();
        base.Start();
        enemy = GetComponent<Enemy>();
    }

    private void ApplyLevelModifiers()
    {
        Modify(strength);
        Modify(agility);
        Modify(intelligence);
        Modify(vitality);

        Modify(damage);
        Modify(critChance);
        Modify(critPower);

        Modify(maxHealth);
        Modify(armor);
        Modify(evasion);
        Modify(magicRsistance);

        Modify(fireDamage);
        Modify(iceDamage);
        Modify(lightningDamage);

        Modify(soulsDropAmount);
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
        //Debug.Log(enemy.name + "受到伤害: " + _damage);
    }

    protected override void Die()
    {
        base.Die();
        enemy.Die();
        //敌人死后掉落灵魂点
        PlayerManager.instance.currency += soulsDropAmount.GetValue();
        myDropSystem.GenerateDrop();

        //敌人死亡5S后销毁尸体
        Destroy(gameObject, 5f);

    }

    private void Modify(Stat _stat)
    {
        for(int i = 1; i < level; i++)
        {
            float modifier = _stat.GetValue() * percentageModifer;
            _stat.AddModifier(Mathf.RoundToInt(modifier));
        }
    }
}
