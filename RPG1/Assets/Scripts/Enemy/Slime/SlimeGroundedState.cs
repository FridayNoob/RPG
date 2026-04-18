using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeGroundedState : EnemyState
{
    private Transform player;
    protected Enemy_Slime enemy;
    public SlimeGroundedState(EnemyStateMachine _stateMachine, Enemy _enemyBase, string _animBoolName, Enemy_Slime _enemy) : base(_stateMachine, _enemyBase, _animBoolName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
        player = PlayerManager.instance.player.transform;
    }
    public override void Update()
    {
        base.Update();

        if (enemy.IsPlayerDetected() || Vector2.Distance(player.position, enemy.transform.position) < enemy.agroDistance)
            stateMachine.ChangeState(enemy.battleState);


    }

    public override void Exit()
    {
        base.Exit();
    }
}
