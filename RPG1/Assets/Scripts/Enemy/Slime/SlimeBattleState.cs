using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBattleState : EnemyState
{
    private Enemy_Slime enemy;

    private Transform player;
    private int moveDir;

    public SlimeBattleState(EnemyStateMachine _stateMachine, Enemy _enemyBase, string _animBoolName, Enemy_Slime _enemy) : base(_stateMachine, _enemyBase, _animBoolName)
    {
        enemy = _enemy;
    }

    // Start is called before the first frame update
    public override void Enter()
    {
        base.Enter();
        player = PlayerManager.instance.player.transform;

        if (player.GetComponent<PlayerStats>().isDead)
            stateMachine.ChangeState(enemy.moveState);
    }
    public override void Update()
    {
        base.Update();
        //Debug.Log("ATK");

        if (enemy.IsPlayerDetected())
        {
            stateTimer = enemy.battleTime;
            if (enemy.IsPlayerDetected().distance < enemy.attackDistance)
            {
                if (CanAttack())
                    stateMachine.ChangeState(enemy.attackState);
            }

        }
        else
        {
            if (stateTimer < 0 || Vector2.Distance(player.transform.position, enemy.transform.position) > 10)
                stateMachine.ChangeState(enemy.idleState);
        }

        if (player.position.x > enemy.transform.position.x)
            moveDir = 1;
        else if (player.position.x < enemy.transform.position.x)
            moveDir = -1;
        //当敌人攻击玩家时，敌人停止移动
        if (enemy.IsPlayerDetected() && enemy.IsPlayerDetected().distance < enemy.attackDistance - .1f)
            return;

        enemy.SetVelocity(enemy.moveSpeed * moveDir, rb.velocity.y);
    }

    public override void Exit()
    {
        base.Exit();
    }

    private bool CanAttack()
    {
        if (Time.time > enemy.lastAttacked + enemy.attackCooldown)
        {
            //快慢刀
            enemy.attackCooldown = Random.Range(enemy.minAttackCooldown, enemy.maxAttackCooldown);
            enemy.lastAttacked = Time.time;
            return true;
        }
        return false;
    }
}
