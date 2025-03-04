using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBringerCastState : DeathBringerState
{
    private int castAmount;
    private float castTimer;

    public DeathBringerCastState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, DeathBringer _enemy) : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        castAmount = enemy.castAmount;
        castTimer = 0.5f;
    }

    public override void Exit()
    {
        base.Exit();

        enemy.lastTimeEnterSpellCastState = Time.time;
    }

    public override void Update()
    {
        base.Update();

        castTimer -= Time.deltaTime;

        if (CanCast())
        {
            enemy.CastSpell();
        }
        
        if (castAmount <= 0)
        {
            stateMachine.ChangeState(enemy.teleportState);
        }
    }

    private bool CanCast()
    {
        if (castAmount >= 0 && castTimer < 0)
        {
            castAmount--;
            castTimer = enemy.castCooldown;
            return true;
        }

        return false;
    }
}
