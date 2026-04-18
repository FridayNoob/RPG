using UnityEngine;


public class ArcherStunnedState : EnemyState
{

    private Enemy_Archer enemy;

    public ArcherStunnedState(EnemyStateMachine _stateMachine, Enemy _enemyBase, string _animBoolName, Enemy_Archer _enemy) : base(_stateMachine, _enemyBase, _animBoolName)
    {
        this.enemy = _enemy;
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

        if (stateTimer < 0)
            stateMachine.ChangeState(enemy.idleState);


    }

    public override void Exit()
    {
        base.Exit();
        enemy.fx.Invoke("CancelColorChange", 0);
    }
}
