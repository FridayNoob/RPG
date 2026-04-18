using System.Collections;
using UnityEngine;

public class Player : Entity
{
    public GameObject sword { get; private set; }

    [Header("Attack details")]
    public Vector2[] attackMovement;
    public float counterAttackDuration = .2f;

    [Header("Move info")]
    public float moveSpeed = 12f;
    public float jumpForce;
    public float swordReturnImpact;
    private float defaultMoveSpeed;
    private float defaultJumpForce;

    public bool isBusy;
    [Header("Dash info")]
    public float dashSpeed;
    public float dashDuration;
    private float defaultDashSpeed;

    

    public PlayerFX fx { get; private set; }

    public SkillManager skill { get; private set; }

    public float dashDir { get; private set; }

    #region States
    public PlayerStateMachine stateMachine { get; private set; }
    
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }

    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState { get; private set; }

    public PlayerDashState dashState { get; private set; }

    public PlayerWallSlideState wallSlideState { get; private set; }
    public PlayerWallJumpState wallJumpState { get; private set; }
    public PlayerPrimaryAttackState primaryAttackState { get; private set; }
    public PlayerCounterAttackState counterAttackState { get; private set; }

    public PlayerAimSwordState aimSwordState { get; private set; }
    public PlayerCatchSwordState catchSwordState { get; private set; }
    public PlayerBlackholeState blackholeState { get; private set; }
    public PlayerDeadState deadState { get; private set; }
    #endregion

    protected override void Awake()
    {
        base.Awake();

        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        airState = new PlayerAirState(this, stateMachine, "Jump");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        wallSlideState = new PlayerWallSlideState(this, stateMachine, "WallSlide");
        wallJumpState = new PlayerWallJumpState(this, stateMachine, "Jump");
        primaryAttackState = new PlayerPrimaryAttackState(this, stateMachine, "Attack");
        counterAttackState = new PlayerCounterAttackState(this, stateMachine, "CounterAttack");
        aimSwordState = new PlayerAimSwordState(this, stateMachine, "AimSword");
        catchSwordState = new PlayerCatchSwordState(this, stateMachine, "CatchSword");
        blackholeState = new PlayerBlackholeState(this, stateMachine, "Jump");
        deadState = new PlayerDeadState(this, stateMachine, "Die");
    }

    protected override void Start()
    {

        base.Start();

        //玩家开始面向右边，怪物开始面向左边
        facingDir = 1;
        facingRight = true;

        stateMachine.Initailize(idleState);
        skill = SkillManager.instance;

        defaultDashSpeed = dashSpeed;
        defaultJumpForce = jumpForce;
        defaultMoveSpeed = moveSpeed;

        fx = GetComponent<PlayerFX>();
    }

    protected override void Update()
    {

        //如果游戏暂停，停止接收任何输入指令
        if (Time.timeScale == 0)
            return;
        //Debug.Log(Screen.width + " , " + Screen.height);

        base.Update();
        stateMachine.currentState.Update();
        CheckForDashInput();
        //if (IsGroundDetected())
        //    Debug.Log("检测到地面！");
        //if (IsWallDetected())
        //    Debug.Log("墙壁");
        if (Input.GetKeyDown(KeyCode.F) && skill.crystal.crystalUnlocked)
            skill.crystal.CanUseSkill();

        if (Input.GetKeyDown(KeyCode.Alpha1))
            Inventory.instance.UseFlask();

    }

    public override void DamageEffect()
    {
        base.DamageEffect();
        fx.StartCoroutine("FlashFX");
    }
    public IEnumerator BusyFor(float _second)
    {
        isBusy = true;

        yield return new WaitForSeconds(_second);

        isBusy = false;
    }

    public void AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger();

    private void CheckForDashInput()
    {
        if (IsWallDetected())
            return;

        if (skill.dash.dashUnlocked == false)
            return;

        if (Input.GetKeyDown(KeyCode.LeftShift) && SkillManager.instance.dash.CanUseSkill())
        {
            

            dashDir = Input.GetAxisRaw("Horizontal");

            if (dashDir == 0)
                dashDir = facingDir;

            stateMachine.ChangeState(dashState);

        }
    }

    public void AssignNewSword(GameObject _newSword)
    {
        sword = _newSword;
    }

    public void CatchTheSword()
    {
        stateMachine.ChangeState(catchSwordState);
        Destroy(sword);
    }

    public void ExitBlackholeAbility()
    {
        stateMachine.ChangeState(airState);
    }

    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState(deadState);
    }

    public override void SlowEntityBy(float _slowPercentage, float _slowSDuraiton)
    {
        moveSpeed = moveSpeed * (1 - _slowPercentage);
        dashSpeed = dashSpeed * (1 - _slowPercentage);
        jumpForce = jumpForce * (1 - _slowPercentage);
        anim.speed = anim.speed * (1 - _slowPercentage);
        Invoke("ReturnDefaultSpeed", _slowSDuraiton);
    }

    public override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();

        moveSpeed = defaultMoveSpeed;
        dashSpeed = defaultDashSpeed;
        jumpForce = defaultJumpForce;
    }

    public override void SetupZeroKnockbackPower()
    {
        knockbackPower = new Vector2(0, 0);
    }
}
