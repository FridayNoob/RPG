using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallSlideState : PlayerState
{
    public PlayerWallSlideState(Player _player, PlayerStateMachine _playerStateMachine, string _aninmBoolName) : base(_player, _playerStateMachine, _aninmBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        //从墙上滑下来时，进入空中状态
        if (!player.IsWallDetected())
            stateMachine.ChangeState(player.airState);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            stateMachine.ChangeState(player.wallJumpState);
            return;

        }
        
        if (xInput != 0 && player.facingDir != xInput)
            stateMachine.ChangeState(player.airState);  //TODO:修改4 idleState修改为airState

        if (yInput < 0)
            player.SetVelocity(0, rb.velocity.y);
        else
            player.SetVelocity(0, rb.velocity.y * .7f);

        if (player.IsGroundDetected())
            stateMachine.ChangeState(player.idleState);
    }
}
