using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using UnityEngine;


//添加组件要求
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(EnemyStats))]
[RequireComponent(typeof(EntityFX))]
[RequireComponent(typeof(ItemDrop))]
public class Enemy : Entity
{
    private float defaultSpeed;

    [Header("Stunned info")]
    public Vector2 stunnedDirection = new Vector2(7, 12);
    public float stunnedDuration = 1;
    protected bool canBeStunned;
    [SerializeField] protected GameObject counterImage;

    [SerializeField] protected LayerMask whatIsPlayer;

    [Header("Attack info")]
    public float agroDistance = 2;
    public float attackDistance = 2;
    //快慢刀
    public float attackCooldown = 0.4f;
    public float minAttackCooldown = 0.35f;
    public float maxAttackCooldown = 0.35f;

    [HideInInspector] public float lastAttacked;

    [Header("Move info")]
    public float moveSpeed = 1.5f;
    public float idleTime = 1;
    public float battleTime = 10;

    public EnemyStateMachine stateMachine;

    public CapsuleCollider2D cd;

    public EntityFX fx { get; private set; }

    public string lastAnimBoolName { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        stateMachine = new EnemyStateMachine();

        defaultSpeed = moveSpeed;
    }

    protected override void Start()
    {
        base.Start();
        fx = GetComponent<EntityFX>();
        cd = GetComponent<CapsuleCollider2D>();
    }

    protected override void Update()
    {
        base.Update();

        //if (!IsGroundDetected())
        //    Debug.Log("敌人离开地面");
        //if(IsWallDetected())
        //    Debug.Log("敌人撞墙");
        //Debug.Log(IsPlayerDetected().cd.gameObject.name+" I see");

        stateMachine.currentState.Update();




    }

    public override void DamageEffect()
    {
        base.DamageEffect();
        fx.StartCoroutine("FlashFX");
    }
    #region Player Detect
    public virtual RaycastHit2D IsPlayerDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, 9, whatIsPlayer);

    //画攻击判定线
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + attackDistance * facingDir, transform.position.y));
    }
    #endregion

    public virtual void AnimationFinishTrigger() => stateMachine.currentState.AnimationFinishTrigger();

    #region Counter Attack Window
    public virtual void OpenCounterWindown()
    {
        canBeStunned = true;
        counterImage.SetActive(true);
    }

    public virtual void CloseCounterWindow()
    {
        canBeStunned = false;
        counterImage.SetActive(false);
    }
    #endregion

    public virtual bool CanBeStunned()
    {
        if(canBeStunned)
        {
            CloseCounterWindow();
            return true;
        }
        return false;
    }

    public virtual void FreezeTime(bool _timeFrozen)
    {
        if (_timeFrozen)
        {
            moveSpeed = 0;
            anim.speed = 0;
        }
        else
        {
            moveSpeed = defaultSpeed;
            anim.speed = 1;
        }
    }

    public virtual void FreezeTimeFor(float _second)
    {
        StartCoroutine(FreezeTimeCoroutine(_second));
    }
    protected virtual IEnumerator FreezeTimeCoroutine(float _seconds)
    {
        FreezeTime(true);

        yield return new WaitForSeconds(_seconds);

        FreezeTime(false);
    }

    public virtual void AssignLastAnimName(string _animBoolName)
    {
        lastAnimBoolName = _animBoolName;
    }

    public override void SlowEntityBy(float _slowPercentage, float _slowSDuraiton)
    {
        moveSpeed= moveSpeed * (1 - _slowPercentage);
        anim.speed = anim.speed * (1 - _slowPercentage);
        Invoke("ReturnDefaultSpeed", _slowSDuraiton);
    }

    public override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();
        moveSpeed = defaultSpeed;
    }

    public virtual void AnimationSpecialAttackTrigger() 
    { 

    }
}
