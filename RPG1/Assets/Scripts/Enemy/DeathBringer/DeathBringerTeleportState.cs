using UnityEngine;


public class DeathBringerTeleportState : EnemyState
{
    private Enemy_DeathBringer enemy;
    public DeathBringerTeleportState(EnemyStateMachine _stateMachine, Enemy _enemyBase, string _animBoolName, Enemy_DeathBringer _enemy) : base(_stateMachine, _enemyBase, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.stats.MakeInvincible(true);
    }

    public override void Exit()
    {
        base.Exit();

        enemy.stats.MakeInvincible(false);

        
    }

    public override void Update()
    {
        base.Update();

        if (triggerCalled)
        {
            if (enemy.CanSpellCast())
                stateMachine.ChangeState(enemy.spellCastState);
            else
                stateMachine.ChangeState(enemy.battleState);
        }
    }
}
