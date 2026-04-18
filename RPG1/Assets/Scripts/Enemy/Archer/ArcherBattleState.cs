using UnityEngine;



public class ArcherBattleState : EnemyState
{

    private Enemy_Archer enemy;
    private Transform player;

    public ArcherBattleState(EnemyStateMachine _stateMachine, Enemy _enemyBase, string _animBoolName, Enemy_Archer _enemy) : base(_stateMachine, _enemyBase, _animBoolName)
    {
        this.enemy = _enemy;
    }


    public override void Enter()
    {
        base.Enter();
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
            //当与玩家距离小于安全距离时，敌人会往后跳
            if (enemy.IsPlayerDetected().distance < enemy.safeDistance)
            {
                if (CanJump())
                    stateMachine.ChangeState(enemy.jumpState);
            }

            if (enemy.IsPlayerDetected().distance < enemy.attackDistance)
            {
                if (CanAttack())
                    stateMachine.ChangeState(enemy.attackState);
            }

        }
        else
        {
            if (stateTimer < 0 || Vector2.Distance(player.transform.position, enemy.transform.position) > 10)
                stateMachine.ChangeState(enemy.idleState);
        }

        BattleStateFlipControl();
    }

    private void BattleStateFlipControl()
    {
        if (player.position.x > enemy.transform.position.x && enemy.facingDir == -1)
            enemy.Flip();
        else if (player.position.x < enemy.transform.position.x && enemy.facingDir == 1)
            enemy.Flip();
    }

    public override void Exit()
    {
        base.Exit();
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

    private bool CanJump()
    {
        if(enemy.GroundBehindCheck() == false || enemy.WallBehindCheck() == true)
            return false;

        if(Time.time > enemy.lasttimeJump + enemy.jumpCooldown)
        {
            enemy.lasttimeJump = Time.time;
            return true;
        }
        return false;
    }
}
