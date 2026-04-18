using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Sword_Skill_Controller : MonoBehaviour
{

    private float freezeTimeDuration;

    [Header("Spin info")]
    private float maxTravelDistance;
    private float spinDuration;
    private float spinTimer;
    private bool wasStopped;
    private bool isSpinning;

    private float hitCooldown;
    private float hitTimer;

    private float spinDirection;

    [Header("Pierce info")]
    [SerializeField] private int pierceAmount;

    private float bounceSpeed;
    public bool isBouncing = false;
    public int bounceAmount = 4;
    private List<Transform> enemyTarget;
    private int targetIndex;

    private bool isReturnning;
    private float returnSpeed = 20;

    private bool canRotate = true;

    private Animator anim;
    private Rigidbody2D rb;
    private CircleCollider2D cd;
    private Player player;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CircleCollider2D>();
    }
    private void Update()
    {
        if (canRotate)
            transform.right = rb.velocity;

        if (isReturnning)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, returnSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, player.transform.position) < 1)
                player.CatchTheSword();
        }

        BounceLogic();
        SpinLogic();
    }

    private void SpinLogic()
    {
        if (isSpinning)
        {
            if (Vector2.Distance(player.transform.position, transform.position) > maxTravelDistance && !wasStopped)
            {
                StopWhenSpinning();
            }
            if (wasStopped)
            {
                spinTimer -= Time.deltaTime;

                //transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x + spinDirection, transform.position.y), 1.5f * Time.deltaTime);

                if (spinTimer < 0)
                {
                    isReturnning = true;
                    isSpinning = false;
                }

                hitTimer -= Time.deltaTime;

                if (hitTimer < 0)
                {
                    hitTimer = hitCooldown;

                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1);

                    foreach (var hit in colliders)
                    {
                        if(hit.GetComponent<Enemy>() != null)
                            SwordSkillDamage(hit.GetComponent<Enemy>());
                    }
                }
            }
        }
    }

    private void StopWhenSpinning()
    {
        wasStopped = true;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        spinTimer = spinDuration;
    }

    private void BounceLogic()
    {
        //isBouncing = false;
        if (isBouncing && enemyTarget.Count > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, enemyTarget[targetIndex].position, bounceSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, enemyTarget[targetIndex].position) < 0.1f)
            {
                SwordSkillDamage(enemyTarget[targetIndex].GetComponent<Enemy>());

                targetIndex++;
                bounceAmount--;

                if (bounceAmount <= 0)
                {
                    isBouncing = false;
                    isReturnning = true;
                }

                if (targetIndex >= enemyTarget.Count)
                    targetIndex = 0;
            }
        }
    }

    public void SetupSword(Vector2 _dir, float _gravityScale, Player _player, float _freezeTimeDuration, float _returnSpeed)
    {
        if(pierceAmount <= 0)
            anim.SetBool("Rotation", true);
        player = _player;
        freezeTimeDuration = _freezeTimeDuration;
        returnSpeed = _returnSpeed;
        //rb = GetComponent<Rigidbody2D>();
        //if (rb == null)
        //    Debug.Log("rb为空");
        rb.velocity = _dir;
        rb.gravityScale = _gravityScale;

        spinDirection = Mathf.Clamp(rb.velocity.x, -1, 1);

        Invoke("DestroyMe", 7);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isReturnning)
            return;

        if(collision.GetComponent<Enemy>() != null)
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            SwordSkillDamage(enemy);
        }
        SetupTargetsForBounce(collision);

        StuckInto(collision);
    }

    private void SwordSkillDamage(Enemy enemy)
    {
        EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
        player.stats.DoDamage(enemyStats);

        if(player.skill.sword.swordTImeStopUnlocked)
            enemy.StartCoroutine("FreezeTimeCoroutine", freezeTimeDuration);

        if (player.skill.sword.swordVulnerableUnlocked)
            enemyStats.MakeVulnerableFor(freezeTimeDuration + 1);

        ItemData_Equipment equippedAmulet = Inventory.instance.GetEquipment(EquipmentType.Amulet);

        equippedAmulet?.Effect(enemy.transform);
    }

    private void SetupTargetsForBounce(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            if (isBouncing && enemyTarget.Count <= 0)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);

                foreach (var hit in colliders)
                {
                    if (hit.GetComponent<Enemy>() != null)
                        enemyTarget.Add(hit.transform);
                }
            }
        }
    }

    private void StuckInto(Collider2D collision)
    {
        if (isSpinning)
        {
            StopWhenSpinning();
            return;
        }

        if(pierceAmount > 0 && collision.GetComponent<Enemy>() != null)
        {
            pierceAmount--;
            return;
        }

        canRotate = false;
        cd.enabled = false;

        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        //播放特效
        GetComponentInChildren<ParticleSystem>().Play();

        if (isBouncing && enemyTarget.Count > 0)
            return;

        anim.SetBool("Rotation", false);
        transform.parent = collision.transform;
    }

    public void ReturnSword()
    {
        //rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        transform.parent = null;
        isReturnning = true;
    }

    public void SetupBounce(bool _isBouncing, int _amountOfBounce, float _bounceSpeed)
    {
        //Debug.Log("Diaoyong1");
        isBouncing = _isBouncing;
        bounceAmount = _amountOfBounce;
        bounceSpeed = _bounceSpeed;

        enemyTarget = new List<Transform>();
    }
    public void SetupPierce(int _pierceAmount)
    {
        pierceAmount = _pierceAmount;
    }
    public void SetupSpin(bool _isSpinning, float _maxTravelDistance, float _spinDuration)
    {
        isSpinning = _isSpinning;
        maxTravelDistance = _maxTravelDistance;
        spinDuration = _spinDuration;
    }

    private void DestroyMe()
    {
        Destroy(gameObject);
    }
}
