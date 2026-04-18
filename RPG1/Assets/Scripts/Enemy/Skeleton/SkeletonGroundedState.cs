using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonGroundedState : EnemyState
{

    private Transform player;
    protected Enemy_Skeleton enemy;
    public SkeletonGroundedState(EnemyStateMachine _stateMachine, Enemy _enemyBase, string _animBoolName, Enemy_Skeleton _enemy) : base(_stateMachine, _enemyBase, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
        player = PlayerManager.instance.player.transform;
    }
    public override void Update()
    {
        base.Update();

        if (enemy.IsPlayerDetected() || Vector2.Distance(player.position, enemy.transform.position )< enemy.agroDistance)
            stateMachine.ChangeState(enemy.battleState);
            

    }

    public override void Exit()
    {
        base.Exit();
    }

}
