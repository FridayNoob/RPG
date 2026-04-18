using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeAttackState : EnemyState
{
    private Enemy_Slime enemy;

    public SlimeAttackState(EnemyStateMachine _stateMachine, Enemy _enemyBase, string _animBoolName, Enemy_Slime _enemy) : base(_stateMachine, _enemyBase, _animBoolName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();

        enemy.lastAttacked = Time.time;
    }

    public override void Update()
    {
        base.Update();

        enemy.SetZeroVelocity();
        //TODO:当玩家还处于敌人攻击范围内，敌人站在原地进行新一轮攻击
        if (triggerCalled)
            stateMachine.ChangeState(enemy.battleState);
    }
}
