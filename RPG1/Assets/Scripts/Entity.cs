using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public SpriteRenderer sr { get; private set; }

    

    [Header("Knock back info")]
    [SerializeField] protected Vector2 knockbackPower = new Vector2(7, 12);
    //每次击退时产生随机偏移
    [SerializeField] private Vector2 knockbackOffset = new Vector2(0.5f, 2);
    [SerializeField] protected float knockBackDuration = 0.07f;
    protected bool isKnocked;

    #region Componets
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public CharacterStats stats { get; private set; }
    #endregion

    [Header("Collision info")]
    public Transform attackCheck;
    public float attackCheckRadius = 1.2f;
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDistance = 0.8f;
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckDistance = 0.75f;
    [SerializeField] protected LayerMask whatIsGround;
    //保存玩家或敌人的受击方向
    public int knockbackDir { get; private set; }
    public  int facingDir { get;  set; } = -1;
    protected  bool facingRight = false;

    public System.Action onFlipped;


    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
        anim = GetComponentInChildren<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        stats = GetComponent<CharacterStats>();
    }

    protected virtual void Update()
    {

    }
    #region Velocity
    public void SetZeroVelocity()
    {
        if (isKnocked)
            return;
        rb.velocity = new Vector2(0, 0);
    }

    public void SetVelocity(float _xVelocity, float _yVelocity)
    {
        if (isKnocked)
            return;
        FlipController(_xVelocity);
        rb.velocity = new Vector2(_xVelocity, _yVelocity);

    }

    #endregion
    #region Collision
    public virtual bool IsGroundDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
    public virtual bool IsWallDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);//TODO:修改2 *facingDir

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance * facingDir, wallCheck.position.y));//TODO：修改3 *facingDir
        Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
    }
    #endregion
    #region Flip
    public virtual void Flip()
    {
        facingDir *= -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
        if (onFlipped != null)
            onFlipped();
    }

    public virtual void FlipController(float _x)
    {
        if (_x > 0 && !facingRight)
            Flip();
        else if (_x < 0 && facingRight)
            Flip();
    }

    #endregion

    public virtual void DamageEffect()
    {
        //Debug.Log(gameObject.name + "  was damaged!");


        StartCoroutine("HitKnockBack");
    }

    public virtual void SetKnockbackDirection(Transform _damageDirection)
    {
        if (_damageDirection.position.x > transform.position.x)
            knockbackDir = -1;
        else if (_damageDirection.position.x < transform.position.x)
            knockbackDir = 1;
    }

    public void SetupKnockbackPower(Vector2 _knockbackPower)
    {
        knockbackPower = _knockbackPower;
    }
    protected virtual IEnumerator HitKnockBack()
    {
        isKnocked = true;
        //水平方向产生一个随机偏移量
        float xOffset = Random.Range(knockbackOffset.x, knockbackOffset.y);
        rb.velocity = new Vector2((knockbackPower.x + xOffset) * knockbackDir, knockbackPower.y);

        yield return new WaitForSeconds(knockBackDuration);
        isKnocked = false;
        //无法连续击退
        SetupZeroKnockbackPower();
    }

    public virtual void SetDefaultFacingDir(int _direction)
    {
        facingDir = _direction;

        if (facingDir == -1)
            facingRight = false;
    }

    public virtual void Die()
    {

    }

    public virtual void SlowEntityBy(float _slowPercentage, float _slowSDuraiton)
    {

    }

    public virtual void ReturnDefaultSpeed()
    {
        anim.speed = 1;
    }

    public virtual void SetupZeroKnockbackPower()
    {

    }
}
