using UnityEngine;


public class ShadyBattleState : EnemyState
{
    private Enemy_Shady enemy;
    private int moveDir;
    private Transform player;

    private float defaultSpeed;

    public ShadyBattleState(EnemyStateMachine _stateMachine, Enemy _enemyBase, string _animBoolName, Enemy_Shady _enemy) : base(_stateMachine, _enemyBase, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        defaultSpeed = enemy.moveSpeed;
        enemy.moveSpeed = enemy.battleStateMoveSpeed;

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

            //Debug.Log(enemy.IsPlayerDetected().distance);

            if (enemy.IsPlayerDetected().distance < enemy.attackDistance)
                 enemy.stats.KillEntity();
        }
        else
        {
            if (stateTimer < 0 || Vector2.Distance(player.transform.position, enemy.transform.position) > 10)
                stateMachine.ChangeState(enemy.idleState);
        }

        BattleStateMoveControl();
    }

    private void BattleStateMoveControl()
    {
        if (player.position.x > enemy.transform.position.x)
            moveDir = 1;
        else if (player.position.x < enemy.transform.position.x)
            moveDir = -1;
        if (enemy.IsGroundDetected())
            enemy.SetVelocity(enemy.moveSpeed * moveDir, rb.velocity.y);
        else
            stateMachine.ChangeState(enemy.idleState);
    }

    public override void Exit()
    {
        base.Exit();

        enemy.moveSpeed = defaultSpeed;
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
