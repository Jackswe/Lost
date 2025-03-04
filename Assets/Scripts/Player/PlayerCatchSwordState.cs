using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCatchSwordState : PlayerState
{
    private Transform sword;
    public PlayerCatchSwordState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        sword = player.sword.transform;

        player.fx.PlayDustFX();
        player.fx.ScreenShake(player.fx.shakeDirection_medium);

        if (sword.position.x < player.transform.position.x && player.facingDirection == 1)
        {
            player.Flip();
        }
        else if (sword.position.x > player.transform.position.x && player.facingDirection == -1)
        {
            player.Flip();
        }

        stateTimer = 0.1f;
        rb.velocity = new Vector2(player.moveSpeed * -player.facingDirection, rb.velocity.y);
    }

    public override void Exit()
    {
        base.Exit();

        player.StartCoroutine(player.BusyFor(0.1f));
    }

    public override void Update()
    {
        base.Update();

        if (stateMachine.currentState != player.catchSwordState)
        {
            return;
        }

        if (stateTimer < 0)
        {
            player.SetVelocity(0, rb.velocity.y);
        }

        if (triggerCalled)
        {
            player.stateMachine.ChangeState(player.idleState);
        }
    }
}
