using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal_Skill_Controller : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsEnemy;
    private CircleCollider2D cd => GetComponent<CircleCollider2D>();

    private Animator anim => GetComponent<Animator>();

    public float crystalExistTimer;
    private float moveSpeed;

    private bool canExplode;
    private bool canMove;

    private bool canGrow;
    [SerializeField] private float growSpeed = 5;

    private Transform closestEnemy;

    private Player player;


    public void SetupCrystal(float _crystalDuration, bool _canExplode, bool _canMove, float _moveSpeed, Transform _closestEnemy, Player _player)
    {
        crystalExistTimer = _crystalDuration;
        canExplode = _canExplode;
        canMove = _canMove;
        moveSpeed = _moveSpeed;
        closestEnemy = _closestEnemy;
        player = _player;
    }
    private void Update()
    {
        crystalExistTimer -= Time.deltaTime;

        if(crystalExistTimer < 0)
        {
            FinishCrystal();
        }

        if (canGrow)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(3, 3), growSpeed * Time.deltaTime);
        }

        if (canMove)
        {
            if (closestEnemy == null)
                return;

            transform.position = Vector2.MoveTowards(transform.position, closestEnemy.position, moveSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, closestEnemy.position) < 2)
                FinishCrystal();
        }
    }

    public void FinishCrystal()
    {
        if (canExplode)
        {
            anim.SetTrigger("Explode");
            canGrow = true;
        }
        else
            SelfDestroy();
    }

    public void SelfDestroy()
    {
        Destroy(gameObject);
    }

    private void AnimationExplodeTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, cd.radius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                //设置目标的击退方向
                hit.GetComponent<Entity>().SetKnockbackDirection(transform);
                player.stats.DoMagicDamage(hit.GetComponent<CharacterStats>());

                ItemData_Equipment equippedAmulet = Inventory.instance.GetEquipment(EquipmentType.Amulet);

                equippedAmulet?.Effect(hit.transform);
            }
        }
    }

    public void ChooseRandomEnemy()
    {
        float radius = SkillManager.instance.blackhole.GetBlackholeRadius();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, whatIsEnemy);

        if (colliders.Length > 0)
            closestEnemy = colliders[Random.Range(0, colliders.Length)].transform;
    }
}
