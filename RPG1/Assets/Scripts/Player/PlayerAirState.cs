using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirState : PlayerState
{
    public PlayerAirState(Player _player, PlayerStateMachine _playerStateMachine, string _aninmBoolName) : base(_player, _playerStateMachine, _aninmBoolName)
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

        //if (Input.GetKeyDown(KeyCode.Space) && player.IsWallDetected()) //TODO:ÐÞ¸Ä1
        //    stateMachine.ChangeState(player.wallJumpState);

        if (player.IsGroundDetected())
            stateMachine.ChangeState(player.idleState);

        if (xInput != 0)
            player.SetVelocity(player.moveSpeed * 0.8f * xInput, rb.velocity.y);

        if (player.IsWallDetected())
            stateMachine.ChangeState(player.wallSlideState);

    }
}
