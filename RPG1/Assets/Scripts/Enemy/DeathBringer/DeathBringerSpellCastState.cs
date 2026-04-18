using System.Collections;
using UnityEngine;


public class DeathBringerSpellCastState : EnemyState
{
    private Enemy_DeathBringer enemy;

    private int spellAmount;
    private float spellTimer;
    public DeathBringerSpellCastState(EnemyStateMachine _stateMachine, Enemy _enemyBase, string _animBoolName, Enemy_DeathBringer _enemy) : base(_stateMachine, _enemyBase, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        spellAmount = enemy.spellAmount;
        spellTimer = .5f;
    }

    public override void Update()
    {
        base.Update();

        spellTimer -= Time.deltaTime;

        if (CanCast())
        {
            enemy.CastSpell();
        }
        
        if(spellAmount <= 0)
            stateMachine.ChangeState(enemy.teleportState);
    }

    public override void Exit()
    {
        base.Exit();

        enemy.lastTimeSpellCast = Time.time;
    }

    private bool CanCast()
    {
        if(spellAmount > 0 && spellTimer < 0)
        {
            spellTimer = enemy.spellCooldown;
            spellAmount--;
            return true;
        }
        return false;
    }
}
