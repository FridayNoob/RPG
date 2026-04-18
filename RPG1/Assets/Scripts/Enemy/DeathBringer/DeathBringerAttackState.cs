using UnityEngine;


public class DeathBringerAttackState : EnemyState
{
    private Enemy_DeathBringer enemy;
    public DeathBringerAttackState(EnemyStateMachine _stateMachine, Enemy _enemyBase, string _animBoolName, Enemy_DeathBringer _enemy) : base(_stateMachine, _enemyBase, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.chanceToTeleport += 5;
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
        {
            if (enemy.CanTeleport())
                stateMachine.ChangeState(enemy.teleportState);
            else
                stateMachine.ChangeState(enemy.battleState);

        }
    }
}
