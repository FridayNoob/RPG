using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerGroundedState
{
    public PlayerIdleState(Player _player, PlayerStateMachine _playerStateMachine, string _aninmBoolName) : base(_player, _playerStateMachine, _aninmBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.SetZeroVelocity();
    }

    public override void Exit()
    {
        base.Exit();
    }
    
    public override void Update()
    {
        
        base.Update();

        if (xInput != 0 && !player.isBusy)
        {
            if (player.IsWallDetected() && xInput == player.facingDir)
                return;
            stateMachine.ChangeState(player.moveState);

        }

    }
}
