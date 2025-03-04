using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerIdleState : PlayerGroundedState
{
    public PlayerIdleState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        AudioManager.instance.StopSFX(14);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (stateMachine.currentState != player.idleState)
        {
            return;
        }

        player.SetVelocity(0, rb.velocity.y);

        if (xInput != 0 && !player.isBusy)
        {
            if (!(player.IsWallDetected() && xInput == player.facingDirection))
            {
                stateMachine.ChangeState(player.moveState);
            }
        }

    }
}
