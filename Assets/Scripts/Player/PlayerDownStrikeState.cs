using UnityEngine;

public class PlayerDownStrikeState : PlayerState
{
    public bool fallingStrikeTrigger { get; private set; } = false;
    public bool animStopTrigger { get; private set; } = false;

    private bool animStopTriggerHasBeenSet = false;
    private bool screenShakeHasBeenSet = false;
    public PlayerDownStrikeState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.SetVelocity(0, -10);

        fallingStrikeTrigger = false;
        animStopTrigger = false;
        animStopTriggerHasBeenSet = false;
        screenShakeHasBeenSet = false;

        stateTimer = Mathf.Infinity;
    }

    public override void Exit()
    {
        base.Exit();

        player.anim.speed = 1;
    }

    public override void Update()
    {
        base.Update();

        if (stateMachine.currentState != this)
        {
            return;
        }

        if (!fallingStrikeTrigger)
        {
            player.SetVelocity(0, 1.2f);
        }
        else
        {
            player.SetVelocity(0, -17);

            if (animStopTrigger && !animStopTriggerHasBeenSet)
            {
                player.anim.speed = 0;
                animStopTriggerHasBeenSet = true;
            }

            if (player.IsGroundDetected())
            {
                player.anim.speed = 1;

                if (!screenShakeHasBeenSet)
                {
                    player.fx.ScreenShake(player.fx.shakeDirection_medium);
                    player.fx.PlayDownStrikeDustFX();
                    screenShakeHasBeenSet = true;
                }
            }

            if (triggerCalled)
            {
                stateMachine.ChangeState(player.idleState);
            }

        }
    }

    public void SetFallingStrikeTrigger()
    {
        fallingStrikeTrigger = true;
    }

    public void SetAnimStopTrigger()
    {
        animStopTrigger = true;
    }

}