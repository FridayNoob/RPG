using UnityEngine;

public class PlayerCatchSwordState : PlayerState
{
    private Transform sword;
    public PlayerCatchSwordState(Player _player, PlayerStateMachine _playerStateMachine, string _aninmBoolName) : base(_player, _playerStateMachine, _aninmBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        sword = player.sword.transform;

        //玩家接住飞剑后会触发灰尘特效
        player.fx.PlayerDashFX();
        //接住剑后会产生屏幕抖动特效
        player.fx.ScreenShake(player.fx.shakeSwordImpact);

        if (sword.position.x > player.transform.position.x && player.facingDir == -1)
            player.Flip();
        else if (sword.position.x < player.transform.position.x && player.facingDir == 1)
            player.Flip();
    }

    public override void Exit()
    {
        base.Exit();
        player.StartCoroutine("BusyFor", .2f);
    }

    public override void Update()
    {
        base.Update();

        if (triggerCalled)
            player.stateMachine.ChangeState(player.idleState);

        rb.velocity = new Vector2(player.swordReturnImpact * -player.facingDir, rb.velocity.y);
    }
}
