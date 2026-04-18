using UnityEngine;

//杀死大史莱姆后会分裂为若干中史莱姆，杀死中史莱姆会分裂为若干小史莱姆
public enum SlimeType { big, medium, small }

public class Enemy_Slime : Enemy
{

    [Header("Slime specific")]
    [SerializeField] private SlimeType slimeType;
    //史莱姆分裂数量
    [SerializeField] private int slimeDivisionAmount;
    [SerializeField] private GameObject slimePrefab;
    //史莱姆分裂时，新生的史莱姆会有一个速度
    [SerializeField] private Vector2 minCreationVelocity;
    [SerializeField] private Vector2 maxCreationVelocity;
    #region States

    public SlimeIdleState idleState { get; private set; }
    public SlimeMoveState moveState { get; private set; }
    public SlimeBattleState battleState { get; private set; }

    public SlimeAttackState attackState { get; private set; }
    public SlimeStunnedState stunnedState { get; private set; }
    public SlimeDeadState deadState { get; private set; }

    #endregion

    protected override void Awake()
    {
        base.Awake();

        //史莱姆开始时面朝左边
        SetDefaultFacingDir(-1);
        idleState = new SlimeIdleState(stateMachine, this, "Idle", this);
        moveState = new SlimeMoveState(stateMachine, this, "Move", this);
        battleState = new SlimeBattleState(stateMachine, this, "Move", this);
        attackState = new SlimeAttackState(stateMachine, this, "Attack", this);
        stunnedState = new SlimeStunnedState(stateMachine, this, "Stunned", this);
        deadState = new SlimeDeadState(stateMachine, this, "Idle", this);
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initailize(idleState);
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.U))
            stateMachine.ChangeState(stunnedState);
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

    private void CreateSlimes(int _slimeDivisionAmount, GameObject _slimePrefab)
    {
        for(int i = 0; i < _slimeDivisionAmount; i++)
        {
            GameObject newSlime = Instantiate(_slimePrefab, transform.position, Quaternion.identity);
            Debug.Log("Slime create-----");
            newSlime.GetComponent<Enemy_Slime>().SetUpSlime(facingDir);
        }
    }

    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState(deadState);

        Debug.Log("Slime die");
        //最小的史莱姆死后不再分裂
        if (slimeType == SlimeType.small)
            return;
        Debug.Log("Slime create" + slimeDivisionAmount);
        CreateSlimes(slimeDivisionAmount, slimePrefab);
    }

    public void SetUpSlime(int _facingDir)
    {

        if (_facingDir != facingDir)
            Flip();

        float xVelocity = Random.Range(minCreationVelocity.x, maxCreationVelocity.x);
        float yVelocity = Random.Range(minCreationVelocity.y, maxCreationVelocity.y);
        //禁止干扰
        isKnocked = true;

        GetComponent<Rigidbody2D>().velocity = new Vector2(xVelocity * -facingDir, yVelocity);
        //分裂时间为1.5s
        Invoke("CancelKnockBack", 1.5f);
    }

    private void CancelKnockBack() => isKnocked = false;
}
