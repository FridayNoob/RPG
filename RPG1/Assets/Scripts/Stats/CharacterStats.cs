using System.Collections;
using UnityEngine;

public enum StatType
{
    strength,
    agility,
    intelligence,
    vitality,
    damage,
    critChance,
    critPower,
    health,
    armor,
    evasion,
    magicRes,
    fireDamage,
    iceDamage,
    lightingDamage
}
public class CharacterStats : MonoBehaviour
{

    [Header("Major stats")]
    public Stat strength;
    public Stat agility;
    public Stat intelligence;
    public Stat vitality;

    [Header("Offensive stats")]
    public Stat damage;
    public Stat critChance;
    public Stat critPower;

    [Header("Defensive stats")]
    public Stat maxHealth;
    public Stat armor;
    public Stat evasion;
    public Stat magicRsistance;

    [Header("Magic Stats")]
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightningDamage;
    private int igniteDamage;

    public bool isIgnited;
    public bool isChilled;
    public bool isShocked;
    public bool isDead { get; private set; }

    private float ignitedTimer;
    private float ignitedDamageCooldown = .3f;
    private float igniteDamageTimer;
    private float chilledTimer;
    private float shockedTimer;
    [SerializeField] private GameObject shockStrikPrefab;
    private int shockDamage;

    public int currentHealth;

    public System.Action onHealthChanged;

    private EntityFX fx;
    [SerializeField] private float ailmentsDuration = 4;

    private bool isVulnerable;
    //冲刺时玩家无敌
    public bool isInvincible { get; private set; }

    protected virtual void Start()
    {
        critPower.SetDefaultValue(150);
        currentHealth = GetMaxHealthValue();
        fx = GetComponent<EntityFX>();
    }

    protected virtual void Update()
    {
        ignitedTimer -= Time.deltaTime;
        igniteDamageTimer -= Time.deltaTime;
        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;

        if (ignitedTimer < 0)
            isIgnited = false;

        if (chilledTimer < 0)
            isChilled = false;

        if (shockedTimer < 0)
            isShocked = false;

        if(igniteDamageTimer < 0 && isIgnited)
        {
            ApplyIgniteDamage();
        }
    }

    public void MakeInvincible(bool _invincible)
    {
        isInvincible = _invincible;
    }
    public void MakeVulnerableFor(float _duration)
    {
        StartCoroutine(VulnerableCoroutine(_duration));
    }

    private IEnumerator VulnerableCoroutine(float _duration)
    {
        isVulnerable = true;

        yield return new WaitForSeconds(_duration);

        isVulnerable = false;
    }

    public virtual void IncreaseStatBy(int _modifier, float _duration, Stat _statToModify)
    {
        StartCoroutine(StatModCoroutine(_modifier, _duration, _statToModify));
    }

    private IEnumerator StatModCoroutine(int _modifier, float _duration, Stat _statToModify)
    {
        _statToModify.AddModifier(_modifier);

        yield return new WaitForSeconds(_duration);

        _statToModify.RemoveModifier(_modifier);
    }

    private void ApplyIgniteDamage()
    {
        igniteDamageTimer = ignitedDamageCooldown;

        DecreaseHealthBy(igniteDamage);
        Debug.Log("【造成燃烧伤害】");
        if (currentHealth < 0 && !isDead)
            Die();
    }

    public virtual void DoDamage(CharacterStats _targetStats)
    {
        bool isCriticalStrike = false;

        if (_targetStats.isInvincible)
            return;

        if (TargetCanAvoidAttack(_targetStats))
            return;
        //设置目标的击退方向
        _targetStats.GetComponent<Entity>().SetKnockbackDirection(transform);

        int totalDamage = damage.GetValue() + strength.GetValue();

        if (CanCrit())
        {
            totalDamage = CalculatingCriticalDamage(totalDamage);
            Debug.Log("暴击伤害：" + totalDamage);

            isCriticalStrike = true;
        }
        //生成打击特效
        fx.CreateHitFX(_targetStats.transform, isCriticalStrike);

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);

        _targetStats.TakeDamage(totalDamage);
        DoMagicDamage(_targetStats);


    }
    public virtual void TakeDamage(int _damage)
    {
        //造成伤害实际统计扣减HP

        if (isInvincible)
            return;

        DecreaseHealthBy(_damage);

        GetComponent<Entity>().DamageEffect();

        if (currentHealth <= 0 && !isDead)
            Die();
    }


    #region Magic damage and ailments
    public void  SetIgniteDamage(int _damage)
    {
        igniteDamage = _damage;
    }
    public void SetupShockStrikeDamage(int _damage)
    {
        shockDamage = _damage;
    }

    public virtual void DoMagicDamage(CharacterStats _targetStats)
    {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightningDamage = lightningDamage.GetValue();

        int totalMagicDamage = _fireDamage + _iceDamage + _lightningDamage + intelligence.GetValue();
        totalMagicDamage = CheckTargetResistance(_targetStats, totalMagicDamage);
        _targetStats.TakeDamage(totalMagicDamage);

        if (Mathf.Max(_fireDamage, _iceDamage, _lightningDamage) <= 0)
            return;

        //Debug.Log("造成魔法伤害");
        AttempToApplyAilments(_targetStats, _fireDamage, _iceDamage, _lightningDamage);
    }

    private  void AttempToApplyAilments(CharacterStats _targetStats, int _fireDamage, int _iceDamage, int _lightningDamage)
    {
        bool canApplyIginit = _fireDamage > _iceDamage && _fireDamage > _lightningDamage;
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _lightningDamage;
        bool canApplyShock = _lightningDamage > _fireDamage && _lightningDamage > _iceDamage;

        while (!canApplyIginit && !canApplyChill && !canApplyShock)
        {
            if (Random.value < .5f && _fireDamage > 0)
            {
                canApplyIginit = true;
                Debug.Log("燃烧");
                _targetStats.ApplyAilments(canApplyIginit, canApplyChill, canApplyShock);
                return;
            }
            if (Random.value < .5f && _iceDamage > 0)
            {
                canApplyChill = true;
                Debug.Log("冰冻");
                _targetStats.ApplyAilments(canApplyIginit, canApplyChill, canApplyShock);
                return;
            }
            if (Random.value < .5f && _lightningDamage > 0)
            {
                canApplyIginit = true;
                Debug.Log("麻痹");
                _targetStats.ApplyAilments(canApplyIginit, canApplyChill, canApplyShock);
                return;
            }
        }
        if (canApplyIginit)
            _targetStats.SetIgniteDamage(Mathf.RoundToInt(_fireDamage * .2f));

        if (canApplyShock)
            _targetStats.SetupShockStrikeDamage(Mathf.RoundToInt(_lightningDamage * .1f));

        _targetStats.ApplyAilments(canApplyIginit, canApplyChill, canApplyShock);
    }
    public void ApplyAilments(bool _ignite, bool _chill, bool _shock)
    {
        bool canApplyIgnite = !isIgnited && !isChilled && !isShocked;
        bool canApplyChill = !isIgnited && !isChilled && !isShocked;
        bool canApplyShock = !isIgnited && !isChilled;

        //Debug.Log("产生元素异常效果" + _ignite + _chill + _shock);

        if (_ignite && canApplyIgnite)
        {
            isIgnited = _ignite;
            ignitedTimer = ailmentsDuration;

            fx.IgniteFxFor(ailmentsDuration);
        }
        if (_chill && canApplyChill)
        {
            isChilled = _chill;
            chilledTimer = ailmentsDuration;

            float slowPercentage = .2f;
            GetComponent<Entity>().SlowEntityBy(slowPercentage, ailmentsDuration);

            fx.ChillFxFor(ailmentsDuration);

        }
        if (_shock && canApplyShock)
        {
            if (!isShocked)
            {
                ApplyShock(_shock);
            }
            else
            {
                if (GetComponent<Player>() != null)
                    return;
                HitNearestTargetWithShockStrike();
            }
        }
    }

    public void ApplyShock(bool _shock)
    {
        if (isShocked)
            return;
        isShocked = _shock;
        shockedTimer = ailmentsDuration;
        fx.ShockFxFor(ailmentsDuration);
    }

    private void HitNearestTargetWithShockStrike()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25);

        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && Vector2.Distance(transform.position, hit.transform.position) > 1)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);
                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }
            if (closestEnemy == null)
                closestEnemy = transform;
        }

        if (closestEnemy != null)
        {
            GameObject newShockStrike = Instantiate(shockStrikPrefab, transform.position, Quaternion.identity);
            newShockStrike.GetComponent<ShockStrike_Controller>().Setup(shockDamage, closestEnemy.GetComponent<CharacterStats>());
        }
    }

    #endregion

    #region Stats calculations
    private  int CheckTargetResistance(CharacterStats _targetStats, int totalMagicDamage)
    {
        totalMagicDamage -= _targetStats.magicRsistance.GetValue() + (_targetStats.intelligence.GetValue() * 3);
        totalMagicDamage = Mathf.Clamp(totalMagicDamage, 0, int.MaxValue);
        return totalMagicDamage;
    }

    protected  int CheckTargetArmor(CharacterStats _targetStats, int totalDamage)
    {
        if (_targetStats.isChilled)
            totalDamage -= Mathf.RoundToInt(_targetStats.armor.GetValue() * .8f);
        else
            totalDamage -= _targetStats.armor.GetValue();
        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);
        return totalDamage;
    }

    public virtual void OnEvasion()
    {

    }
    protected bool TargetCanAvoidAttack(CharacterStats _targetStats)
    {
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();

        if (isShocked)
            totalEvasion += 20;

        if (Random.Range(0, 100) < totalEvasion)
        {
            _targetStats.OnEvasion();
            return true;
        }
        return false;
    }
    protected bool CanCrit()
    {
        int totalCriticalChancce = critChance.GetValue() + agility.GetValue();

        if(Random.Range(0, 100) < totalCriticalChancce)
        {
            return true;
        }
        return false;
    }
    protected int CalculatingCriticalDamage(int _damage)
    {
        float totalCritPower = (critPower.GetValue() + strength.GetValue()) * 0.01f;

        float critDamage = _damage * totalCritPower;

        return Mathf.RoundToInt(critDamage);
    }

    public int GetMaxHealthValue()
    {
        return maxHealth.GetValue() + vitality.GetValue() * 5;
    }
    protected virtual void DecreaseHealthBy(int _damage)
    {
        //目标处于脆弱状态，受到的伤害增加
        if (isVulnerable)
            _damage = Mathf.RoundToInt(_damage * 1.1f);

        if (_damage > 0)
            fx.CreatePopUpText(_damage.ToString());

        currentHealth -= _damage;

        if (onHealthChanged != null)
            onHealthChanged();
    }

    public virtual void IncreaseHealthBy(int _amount)
    {
        currentHealth += _amount;

        if (onHealthChanged != null)
            onHealthChanged();
    }
    #endregion

    public void KillEntity()
    {
        if(!isDead)
            Die();
    }
    protected virtual void Die()
    {
        isDead = true;
    }
    public Stat GetStat(StatType _statType)
    {
        Stat statToModify = null;
        switch (_statType)
        {
            case StatType.strength:
                statToModify = strength; break;
            case StatType.agility:
                statToModify = agility; break;
            case StatType.intelligence:
                statToModify = intelligence; break;
            case StatType.vitality:
                statToModify = vitality; break;
            case StatType.damage:
                statToModify = damage; break;
            case StatType.critChance:
                statToModify = critChance; break;
            case StatType.critPower:
                statToModify = critPower; break;
            case StatType.health:
                statToModify = critPower; break;
            case StatType.armor:
                statToModify = armor; break;
            case StatType.evasion:
                statToModify = evasion; break;
            case StatType.magicRes:
                statToModify = magicRsistance; break;
            case StatType.fireDamage:
                statToModify = fireDamage; break;
            case StatType.iceDamage:
                statToModify = iceDamage; break;
            case StatType.lightingDamage:
                statToModify = fireDamage; break;
        }

        return statToModify;
    }
}
