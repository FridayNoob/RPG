using UnityEngine;


public class ShadyIdleState : ShadyGroundedState
{
    public ShadyIdleState(EnemyStateMachine _stateMachine, Enemy _enemyBase, string _animBoolName, Enemy_Shady _enemy) : base(_stateMachine, _enemyBase, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = enemy.idleTime;
    }

    public override void Exit()
    {
        base.Exit();

        
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
            stateMachine.ChangeState(enemy.moveState);
    }
}
