using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBringerAttackState : DeathBringerState
{
    public DeathBringerAttackState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, DeathBringer _enemy) : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = 0.1f;
    }

    public override void Exit()
    {
        base.Exit();

        enemy.lastTimeAttacked = Time.time;
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer > 0)
        {
            if (enemy.isKnockbacked)
            {
                stateTimer = 0;
                return;
            }

        }
        else
        {
            enemy.SetVelocity(0, rb.velocity.y);
        }

        if (triggerCalled)
        {
            if (enemy.CanTeleport())
            {
                stateMachine.ChangeState(enemy.teleportState);
            }
            else
            {
                enemy.chanceToTeleport += 10;
                stateMachine.ChangeState(enemy.battleState);
            }
        }

    }
}
