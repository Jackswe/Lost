using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerAimSwordState : PlayerState
{
    public PlayerAimSwordState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = 0.1f;

        player.skill.sword.ShowDots(true);
    }

    public override void Exit()
    {
        base.Exit();

        player.StartCoroutine(player.BusyFor(0.1f));
    }

    public override void Update()
    {
        base.Update();

        if (stateMachine.currentState != player.aimSwordState)
        {
            return;
        }

        if (stateTimer < 0)
        {
            player.SetZeroVelocity();
        }

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (mousePosition.x < player.transform.position.x && player.facingDirection == 1)
        {
            player.Flip();
        }
        else if (mousePosition.x > player.transform.position.x && player.facingDirection == -1)
        {
            player.Flip();
        }


        if (Input.GetKeyUp(/*KeyCode.Mouse1*/ KeyBindManager.instance.keybindsDictionary["Aim"]))
        {
            player.skill.sword.ShowDots(false);
            stateMachine.ChangeState(player.idleState);
        }

        if (Input.GetKeyDown(/*KeyCode.Mouse0*/ KeyBindManager.instance.keybindsDictionary["Attack"]))
        {
            stateMachine.ChangeState(player.throwSwordState);
        }
    }
}
