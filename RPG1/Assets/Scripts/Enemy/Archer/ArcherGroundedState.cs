using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherGroundedState : EnemyState
{
    protected Transform player;
    protected Enemy_Archer enemy;

    public ArcherGroundedState(EnemyStateMachine _stateMachine, Enemy _enemyBase, string _animBoolName, Enemy_Archer _enemy) : base(_stateMachine, _enemyBase, _animBoolName)
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

        if (enemy.IsPlayerDetected() || Vector2.Distance(player.position, enemy.transform.position) < enemy.agroDistance)
            stateMachine.ChangeState(enemy.battleState);


    }

    public override void Exit()
    {
        base.Exit();
    }
}
