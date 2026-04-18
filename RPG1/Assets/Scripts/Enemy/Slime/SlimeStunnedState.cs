using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeStunnedState : EnemyState
{
    private Enemy_Slime enemy;

    public SlimeStunnedState(EnemyStateMachine _stateMachine, Enemy _enemyBase, string _animBoolName, Enemy_Slime _enemy) : base(_stateMachine, _enemyBase, _animBoolName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = enemy.stunnedDuration;
        rb.velocity = new Vector2(enemy.stunnedDirection.x * -enemy.facingDir, enemy.stunnedDirection.y);

        enemy.fx.InvokeRepeating("RedColorBlink", 0, .1f);
    }
    public override void Update()
    {
        base.Update();

        //ÊÜ»÷ºóÎÞµÐ
        if (rb.velocity.y < .1f && enemy.IsGroundDetected())
        {
            enemy.anim.SetTrigger("StunFold");
            enemy.stats.MakeInvincible(true);
            enemy.fx.Invoke("CancelColorChange", 0);
        }

        if (stateTimer < 0)
            stateMachine.ChangeState(enemy.idleState);


    }

    public override void Exit()
    {
        base.Exit();
        enemy.stats.MakeInvincible(false);
    }
}
