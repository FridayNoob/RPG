using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrimaryAttackState : PlayerState
{
    public int comboCounter { get; private set; }

    private float lastTimeAttacked;
    private float comboWindown = 2;
    public PlayerPrimaryAttackState(Player _player, PlayerStateMachine _playerStateMachine, string _aninmBoolName) : base(_player, _playerStateMachine, _aninmBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        ////播放攻击音效
        //AudioManager.instance.PlaySFX(4);

        xInput = 0; //解决xInput不为0导致攻击方向出问题的BUG

        stateTimer = .1f;

        if (comboCounter > 2 || Time.time > lastTimeAttacked + comboWindown)
            comboCounter = 0;

        player.anim.SetInteger("ComboCounter", comboCounter);

        #region Choose attack direction

        float attackDir = player.facingDir;

        if (xInput != 0)
            attackDir = xInput;

        #endregion

        player.SetVelocity(player.attackMovement[comboCounter].x * attackDir, player.attackMovement[comboCounter].y);
    }

    public override void Exit()
    {
        base.Exit();

        comboCounter++;
        lastTimeAttacked = Time.time;

        player.StartCoroutine("BusyFor", .15f);
    }

    public override void Update()
    {
        base.Update();

        if (triggerCalled)
            stateMachine.ChangeState(player.idleState);

        if (stateTimer < 0)
            player.SetZeroVelocity();
    }
}
