using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Archer : Enemy
{
    [Header("Archer specific info")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private float arrowSpeed;
    [SerializeField] private int arrowDamage;

    public Vector2 jumpVelocity;
    public float jumpCooldown;
    [HideInInspector] public float lasttimeJump;

    public float safeDistance;  //弓箭手与玩家距离小于此值时，弓箭手就会往后跳


    [Header("Additional collision check")]
    [SerializeField] private Transform groundBehindCheck;
    [SerializeField] private Vector2 groundBehindCheckSize;

    #region States

    public ArcherIdleState idleState { get; private set; }
    public ArcherMoveState moveState { get; private set; }
    public ArcherDeadState deadState { get; private set; }
    public ArcherBattleState battleState { get; private set; }
    public ArcherAttackState attackState { get; private set; }
    public ArcherStunnedState stunnedState { get; private set; }

    public ArcherJumpState jumpState { get; private set; }
    #endregion

    protected override void Awake()
    {
        base.Awake();

        idleState = new ArcherIdleState(stateMachine, this, "Idle", this);
        moveState = new ArcherMoveState(stateMachine, this, "Move", this);
        deadState = new ArcherDeadState(stateMachine, this, "Idle", this);
        battleState = new ArcherBattleState(stateMachine, this, "Idle", this);
        attackState = new ArcherAttackState(stateMachine, this, "Attack", this);
        stunnedState = new ArcherStunnedState(stateMachine, this, "Stunned", this);
        jumpState = new ArcherJumpState(stateMachine, this, "Jump", this);
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initailize(idleState);
    }

    public override void AnimationSpecialAttackTrigger()
    {
        GameObject newArrow = Instantiate(arrowPrefab, attackCheck.position, Quaternion.identity);

        newArrow.GetComponent<Arrow_Controller>().SetupArrow(arrowSpeed * facingDir, stats);

    }

    public override bool CanBeStunned()
    {
        if (base.CanBeStunned())
        {
            stateMachine.ChangeState(stunnedState);
            return true;
        }
        return false;
    }

    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState(deadState);

    }
    //检查后面是否是悬崖
    public bool GroundBehindCheck() => Physics2D.BoxCast(groundBehindCheck.position, groundBehindCheckSize, 0, Vector2.zero,0, whatIsGround);

    public bool WallBehindCheck() => Physics2D.Raycast(wallCheck.position, Vector2.right * -facingDir, wallCheckDistance + 2, whatIsGround);

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.DrawWireCube(groundBehindCheck.position, groundBehindCheckSize);
    }
}
