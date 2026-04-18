using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
    protected Rigidbody2D rb;

    protected Player player;
    protected PlayerStateMachine stateMachine;

    protected float xInput;
    protected float yInput;
    private string aninmBoolName;

    protected float stateTimer;
    protected bool triggerCalled;

    public PlayerState(Player _player, PlayerStateMachine _playerStateMachine, string _aninmBoolName)
    {
        this.player = _player;
        this.stateMachine = _playerStateMachine;
        this.aninmBoolName = _aninmBoolName;
    }

    public virtual void Enter()
    {
        player.anim.SetBool(aninmBoolName, true);
        rb = player.rb;
        triggerCalled = false;
    }

    public virtual void Update()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        player.anim.SetFloat("yVelocity", rb.velocity.y);

        

        stateTimer -= Time.deltaTime;
    }

    public virtual void Exit()
    {

        player.anim.SetBool(aninmBoolName, false);
    }

    public virtual void AnimationFinishTrigger()
    {
        triggerCalled = true;
    }
}
