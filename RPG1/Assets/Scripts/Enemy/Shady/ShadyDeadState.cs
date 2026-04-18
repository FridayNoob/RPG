using UnityEngine;


public class ShadyDeadState : EnemyState
{
    private Enemy_Shady enemy;
    public ShadyDeadState(EnemyStateMachine _stateMachine, Enemy _enemyBase, string _animBoolName, Enemy_Shady _enemy) : base(_stateMachine, _enemyBase, _animBoolName)
    {
        this.enemy = _enemy;
    }
    public override void Enter()
    {
        base.Enter();
        //Debug.Log("Enemy Dead");
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (triggerCalled)
            enemy.SelfDestroy();
    }
}
