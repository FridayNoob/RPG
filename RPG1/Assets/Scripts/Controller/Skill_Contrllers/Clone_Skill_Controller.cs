using UnityEngine;

public class Clone_Skill_Controller : MonoBehaviour
{
    private Transform closestEnemy;
    [SerializeField] private Transform attackCheck;
    [SerializeField] private float attackCheckRadius = 1f;

    private Animator anim;
    [SerializeField] private float colorLosingSpeed;
    private SpriteRenderer sr;
    private float cloneTimer;
    private float attackMultiplier;
    private bool canDuplicateClone;
    private float chanceToDuplicate;

    private int facingDir = 1;

    private Player player;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

    }
    private void Update()
    {
        cloneTimer -= Time.deltaTime;
        if (cloneTimer < 0)
        {
            sr.color = new Color(1, 1, 1, sr.color.a - (Time.deltaTime * colorLosingSpeed));

            if (sr.color.a < 0)
                Destroy(gameObject);
        }
    }
    public void SetupClone(Transform _newTransform, float _cloneDuration, bool _canAttack, Vector3 _offset, Transform _closestEnemy, bool _canDuplicateClone, float _chanceToDuplicate, Player _player, float _attackMultiplier)
    {
        closestEnemy = _closestEnemy;
        if (_canAttack)
            anim.SetInteger("AttackNumber", Random.Range(1, 4));
        transform.position = _newTransform.position + _offset;
        cloneTimer = _cloneDuration;
        canDuplicateClone = _canDuplicateClone;
        chanceToDuplicate = _chanceToDuplicate;
        player = _player;
        attackMultiplier = _attackMultiplier;
        FacingClosestTarget();
    }

    private void AnimationTrigger()
    {
        cloneTimer = -.1f;
    }

    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                //设置目标的击退方向
                hit.GetComponent<Entity>().SetKnockbackDirection(transform);

                //player.stats.DoDamage(hit.GetComponent<CharacterStats>());
                PlayerStats playerStats = player.GetComponent<PlayerStats>();
                EnemyStats enemyStats = hit.GetComponent<EnemyStats>();

                playerStats.CloneDoDamage(enemyStats, attackMultiplier);

                //应用受到打击的技能效果
                if (player.skill.clone.canApplyHitEffect)
                {
                    Inventory.instance.GetEquipment(EquipmentType.Weapon)?.Effect(hit.transform);
                }

                if (canDuplicateClone)
                {
                    if (Random.Range(0, 100) < chanceToDuplicate)
                    {
                        SkillManager.instance.clone.CreateClone(hit.transform, new Vector3(.5f * facingDir, 0));
                    }
                }
            }

        }
    }

    private void FacingClosestTarget()
    {
        if (closestEnemy != null)
        {
            if (transform.position.x > closestEnemy.position.x)
            {
                facingDir = -1;
                transform.Rotate(0, 180, 0);
            }
        }
    }
}
