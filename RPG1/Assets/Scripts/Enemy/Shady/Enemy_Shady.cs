using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class Enemy_Shady : Enemy
{
    [Header("Shady specifics")]
    public float battleStateMoveSpeed = 5f;

    [SerializeField] private GameObject explosivePrefab;
    [SerializeField] private float growSpeed;
    [SerializeField] private float maxSize;
    #region States

    public ShadyIdleState idleState { get;private set; }
    public ShadyBattleState battleState { get; private set; }
    public ShadyDeadState deadState { get; private set; }
    public ShadyMoveState moveState { get; private set; }
    public ShadyStunnedState stunnedState { get; private set; }
    #endregion
    protected override void Awake() 
    {
        base.Awake();
        idleState = new ShadyIdleState(stateMachine, this, "Idle", this);
        battleState = new ShadyBattleState(stateMachine, this, "MoveFast", this);
        deadState = new ShadyDeadState(stateMachine, this, "Dead", this);
        moveState = new ShadyMoveState(stateMachine, this, "Move", this);
        stunnedState = new ShadyStunnedState(stateMachine, this, "Stunned", this);
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initailize(idleState);
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

    public override void AnimationSpecialAttackTrigger()
    {
        GameObject newExplosive = Instantiate(explosivePrefab, attackCheck.position, Quaternion.identity);

        newExplosive.GetComponent<shadyExplosive_Controller>().SetUpExplosive(stats, growSpeed, maxSize, attackCheckRadius);

        cd.enabled = false;
        rb.gravityScale = 0;

    }

    public void SelfDestroy() => Destroy(gameObject);
}
