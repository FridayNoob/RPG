using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy_DeathBringer : Enemy
{
    public bool bossFightBegin;

    [Header("Spell cast details")]
    [SerializeField] private GameObject spellPrefab;
    public int spellAmount;
    public float spellCooldown;

    public float lastTimeSpellCast;
    [SerializeField] private float spellStateCooldown;

    [Header("Teleport details")]
    [SerializeField]private Vector2 surroundingCheckSize;   //检测传送目标位置是否有障碍
    [SerializeField] private BoxCollider2D arena;   //圆形竞技场

    public float chanceToTeleport;  ///随着攻击次数增加，传送几率也随之增加
    public float defaultChanceToTeleport = 25;

    #region States

    public DeathBringerIdleState idleState { get; private set; }
    public DeathBringerAttackState attackState { get; private set; }
    public DeathBringerBattleState battleState { get; private set; }
    public DeathBringerDeadState deadState { get; private set; }
    public DeathBringerSpellCastState spellCastState { get; private set; }
    public DeathBringerTeleportState teleportState { get; private set;}

    #endregion

    protected override void Awake()
    {
        base.Awake();

        SetDefaultFacingDir(-1);

        idleState = new DeathBringerIdleState(stateMachine, this, "Idle", this);
        attackState = new DeathBringerAttackState(stateMachine, this, "Attack", this);
        battleState = new DeathBringerBattleState(stateMachine, this, "Move", this);
        deadState = new DeathBringerDeadState(stateMachine, this, "Dead", this);
        spellCastState = new DeathBringerSpellCastState(stateMachine, this, "SpellCast", this);
        teleportState = new DeathBringerTeleportState(stateMachine, this, "Teleport", this);
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initailize(idleState);
    }

    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState(deadState);
    }

    public void FindPosition()
    {
        float x = Random.Range(arena.bounds.min.x +3, arena.bounds.max.x - 3);  //+3/-3是为了不让敌人落在边界上
        float y = Random.Range(arena.bounds.min.y + 3, arena.bounds.max.y - 3);

        transform.position = new Vector3(x, y);
        transform.position = new Vector3(transform.position.x, transform.position.y - GroundBelow().distance + (cd.size.y / 2));

        if(!GroundBelow() || SomethingIsAround())
        {
            Debug.Log("Look for new position!");
            FindPosition();
        }
    }

    private RaycastHit2D GroundBelow() => Physics2D.Raycast(transform.position, Vector2.down, 100, whatIsGround);

    private bool SomethingIsAround() => Physics2D.BoxCast(transform.position, surroundingCheckSize, 0, Vector2.zero, 0, whatIsGround);

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - GroundBelow().distance));
        Gizmos.DrawWireCube(transform.position, surroundingCheckSize);
    }

    public bool CanTeleport()
    {
        if(Random.Range(0, 100) < chanceToTeleport)
        {
            chanceToTeleport = defaultChanceToTeleport;
            return true;
        }
        return false;
    }

    public bool CanSpellCast()
    {
        if (Time.time > lastTimeSpellCast + spellStateCooldown)
        {
            return true;
        }
        return false;
    }

    public void CastSpell()
    {
        Player player = PlayerManager.instance.player;

        float xOffset = 0;

        if (player.rb.velocity.x != 0)
            xOffset = player.facingDir * 2;
        Vector3 spellPosition = new Vector3(player.transform.position.x + xOffset, player.transform.position.y + 1.5f);

        GameObject newSpell = Instantiate(spellPrefab, spellPosition, Quaternion.identity);
        newSpell.GetComponent<DeathBringerSpell_Controller>().SetUpSpell(stats);

    }
}
