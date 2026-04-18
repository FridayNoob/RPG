using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shadyExplosive_Controller : MonoBehaviour
{
    private Animator anim;

    private CharacterStats myStates;
    private float growSpeed = 15;
    private float maxSize = 6;
    private float explosiveRadius;

    private bool canGrow = true;
    private void Update()
    {
        if (canGrow)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);
        }

        if(maxSize - transform.localScale.x < .5f)
        {
            canGrow = false;
            anim.SetTrigger("Explode");
        }
    }
    public void SetUpExplosive(CharacterStats _myStats, float _growSpeed, float _maxSize, float _explosiveRadius)
    {
        anim = GetComponent<Animator>();

        myStates = _myStats;
        growSpeed = _growSpeed;
        maxSize = _maxSize;
        explosiveRadius = _explosiveRadius;
    }
    private void AnimationExplodeTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosiveRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<CharacterStats>() != null)
            {
                //设置目标的击退方向
                hit.GetComponent<Entity>().SetKnockbackDirection(transform);
                myStates.DoDamage(hit.GetComponent<CharacterStats>());

            }
        }
    }

    private void SelfDestroy() => Destroy(gameObject);
}
