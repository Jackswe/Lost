using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadyIdleState : ShadyGroundedState
{
    public ShadyIdleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Shady _enemy) : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //AudioManager.instance.StopSFX(14);
        stateTimer = enemy.patrolStayTime;
    }

    public override void Exit()
    {
        base.Exit();

        //AudioManager.instance.PlaySFX(14, enemy.transform);
    }

    public override void Update()
    {
        base.Update();

        if (stateMachine.currentState != enemy.idleState)
        {
            return;
        }

        enemy.SetVelocity(0, rb.velocity.y);

        if (stateTimer < 0)
        {
            enemy.Flip();
            stateMachine.ChangeState(enemy.moveState);
        }
    }
}
