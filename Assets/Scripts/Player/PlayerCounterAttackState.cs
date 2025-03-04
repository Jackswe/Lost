using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 角色反击状态
/// <summary>
/// 角色成功反击之后将会进入这个状态
/// </summary>
public class PlayerCounterAttackState : PlayerState
{
    private bool canCreateClone;

    public PlayerCounterAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = player.counterAttackDuration;
        player.anim.SetBool("SuccessfulCounterAttack", false);

        canCreateClone = true;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (stateMachine.currentState != this)
        {
            return;
        }

        player.SetZeroVelocity();
        // 获取角色攻击范围内的碰撞体
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);

        foreach (var hit in colliders)
        {
            if(hit.GetComponent<Arrow_Controller>() != null)
            {
                hit.GetComponent<Arrow_Controller>().FlipArrow();
                SuccessfulCounterAttack();
            }

            if (hit.GetComponent<Enemy>() != null)
            {
                Enemy enemy = hit.GetComponent<Enemy>();

                if (enemy.CanBeStunnedByCounterAttack())
                {
                    SuccessfulCounterAttack();

                    player.skill.parry.RecoverHPFPInSuccessfulParry();

                    if (canCreateClone)
                    {
                        player.skill.parry.MakeMirageInSuccessfulParry(new Vector3(enemy.transform.position.x - 1.5f * enemy.facingDirection, enemy.transform.position.y));
                        canCreateClone = false; 
                    }
                }
            }
        }

        if (stateTimer < 0 || triggerCalled)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }

    private void SuccessfulCounterAttack()
    {
        stateTimer = 10;

        player.anim.SetBool("SuccessfulCounterAttack", true);
        player.fx.ScreenShake(player.fx.shakeDirection_medium);
    }
}
