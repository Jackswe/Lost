using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    public PlayerDashState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        // 冲刺技能释放
        player.skill.dash.CloneOnDashStart(player.transform.position);

        stateTimer = player.dashDuration;
        
        
        player.stats.BecomeInvincible(true);
    }

    public override void Exit()
    {
        base.Exit();
        // 重置速度
        player.SetVelocity(0, rb.velocity.y);

        player.skill.dash.CloneOnDashEnd(player.transform.position);

        
        player.stats.BecomeInvincible(false);
    }

    public override void Update()
    {
        base.Update();
        // 如果当前角色状态是冲刺状态则直接返回
        if (stateMachine.currentState != player.dashState)
        {
            return;
        }

        if (!player.IsGroundDetected() && player.IsWallDetected())
        {
            stateMachine.ChangeState(player.wallSlideState);
            return;
        }

        player.SetVelocity(player.dashSpeed * player.dashDirection, 0);

        player.fx.CreateAfterimage();

        if (stateTimer < 0)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
