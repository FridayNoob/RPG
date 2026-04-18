using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimSwordState : PlayerState
{
    public PlayerAimSwordState(Player _player, PlayerStateMachine _playerStateMachine, string _aninmBoolName) : base(_player, _playerStateMachine, _aninmBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.skill.sword.DotsActive(true);
    }

    public override void Exit()
    {
        base.Exit();
        player.StartCoroutine("BusyFor", .2f);
    }

    public override void Update()
    {
        base.Update();

        player.SetZeroVelocity();

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (mousePosition.x > player.transform.position.x && player.facingDir == -1)
            player.Flip();
        else if (mousePosition.x < player.transform.position.x && player.facingDir == 1)
            player.Flip();

        if (Input.GetKeyUp(KeyCode.Mouse1))
            stateMachine.ChangeState(player.idleState);
    }
}
