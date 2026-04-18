using UnityEngine;

public class DeathBringerSpell_Controller : MonoBehaviour
{
    [SerializeField] private Transform check;
    [SerializeField] private Vector2 boxSize;
    [SerializeField] private LayerMask whatIsPlayer;

    private CharacterStats myStats;

    public void SetUpSpell(CharacterStats _stats) => myStats = _stats;

    private void AnimationTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(check.position, boxSize, whatIsPlayer);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Player>() != null)
            {
                //设置目标的击退方向
                hit.GetComponent<Entity>().SetKnockbackDirection(transform);
                myStats.DoDamage(hit.GetComponent<CharacterStats>());

            }
        }
    }
    private void OnDrawGizmos() => Gizmos.DrawWireCube(check.position, boxSize);

    private void SelfDestroy() => Destroy(gameObject);
}
