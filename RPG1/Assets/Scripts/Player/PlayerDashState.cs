using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerState
{

    public PlayerDashState(Player _player, PlayerStateMachine _playerStateMachine, string _aninmBoolName) : base(_player, _playerStateMachine, _aninmBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = player.dashDuration;

        //player.skill.clone.CreateClone(player.transform, Vector3.zero);
        player.skill.dash.CloneOnDash();

        //Íæ¼Ò³å´ÌÊ±ÎÞµÐ
        player.stats.MakeInvincible(true);
    }

    public override void Exit()
    {
        base.Exit();

        player.SetVelocity(0, rb.velocity.y);
        player.skill.dash.CloneOnArrival();

        player.stats.MakeInvincible(false);
    }

    public override void Update()
    {
        base.Update();

        player.SetVelocity(player.dashSpeed * player.dashDir, 0);

        if (stateTimer < 0)
            stateMachine.ChangeState(player.idleState);

        if (!player.IsGroundDetected() && player.IsWallDetected())
            stateMachine.ChangeState(player.wallSlideState);

        player.fx.CreateAfterImage();
    }
}
