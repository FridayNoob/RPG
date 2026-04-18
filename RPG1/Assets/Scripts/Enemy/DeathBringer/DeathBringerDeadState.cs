using System.Collections;
using UnityEngine;


public class DeathBringerDeadState : EnemyState
{
    private Enemy_DeathBringer enemy;
    public DeathBringerDeadState(EnemyStateMachine _stateMachine, Enemy _enemyBase, string _animBoolName, Enemy_DeathBringer _enemy) : base(_stateMachine, _enemyBase, _animBoolName)
    {
        this.enemy = _enemy;
    }
    public override void Enter()
    {
        base.Enter();
        enemy.anim.SetBool(enemy.lastAnimBoolName, true);
        enemy.anim.speed = 0;
        enemy.cd.enabled = false;

        stateTimer = .2f;
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer > 0)
        {
            rb.velocity = new Vector2(0, 10);
        }
    }
}
