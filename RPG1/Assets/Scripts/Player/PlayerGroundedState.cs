using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    public PlayerGroundedState(Player _player, PlayerStateMachine _playerStateMachine, string _aninmBoolName) : base(_player, _playerStateMachine, _aninmBoolName)
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

        if (Input.GetKeyDown(KeyCode.R) && player.skill.blackhole.blackholeUnlocked)
        {
            if (player.skill.blackhole.cooldownTimer > 0)
            {
                player.fx.CreatePopUpText("Cooldown");
                return;
            }
            stateMachine.ChangeState(player.blackholeState);
        }

        if (Input.GetKeyDown(KeyCode.Mouse1) && HasNoSword() && player.skill.sword.swordUnlocked)
            stateMachine.ChangeState(player.aimSwordState);

        if (Input.GetKeyDown(KeyCode.Q) && player.skill.parry.parryUnlocked)
            stateMachine.ChangeState(player.counterAttackState);

        if (Input.GetKeyDown(KeyCode.Mouse0))
            stateMachine.ChangeState(player.primaryAttackState);

        if (Input.GetKeyDown(KeyCode.Space) && player.IsGroundDetected()) 
            stateMachine.ChangeState(player.jumpState);

        if (!player.IsGroundDetected())
            stateMachine.ChangeState(player.airState);
    }

    private bool HasNoSword()
    {
        if(player.sword == null)
        {
            return true;
        }

        player.sword.GetComponent<Sword_Skill_Controller>().ReturnSword();
        return false;
    }
}
