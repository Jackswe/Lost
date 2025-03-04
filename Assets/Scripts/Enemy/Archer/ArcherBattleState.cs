using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ArcherBattleState : ArcherState
{
    private Transform player;

    public ArcherBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Archer _enemy) : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = enemy.aggressiveTime;

        player = PlayerManager.instance.player.transform;

        FacePlayer();

        if (player.GetComponent<PlayerStats>().isDead)
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }

    public override void Exit()
    {
        base.Exit();

        //AudioManager.instance.StopSFX(24);
    }

    public override void Update()
    {
        base.Update();

        if (enemy.stateMachine.currentState != this)
        {
            return;
        }


        //AudioManager.instance.PlaySFX(24, enemy.transform);

        //AudioManager.instance.PlaySFX(14, enemy.transform);

        FacePlayer();

        if (enemy.IsPlayerDetected())
        {
            stateTimer = enemy.aggressiveTime;
            //Debug.Log("I see player");

            if (Vector2.Distance(player.transform.position, enemy.transform.position) < enemy.jumpJudgeDistance)
            {
                if (CanJump())
                {
                    anim.SetBool("Idle", false);
                    anim.SetBool("Move", false);
                    stateMachine.ChangeState(enemy.jumpState);
                    return;
                }
            }

            if (enemy.IsPlayerDetected() && Vector2.Distance(player.transform.position, enemy.transform.position) < enemy.attackDistance)
            {

                if (CanAttack())
                {
                    anim.SetBool("Idle", false);
                    anim.SetBool("Move", false);
                    stateMachine.ChangeState(enemy.attackState);
                    return;
                }
            }
        }
        else  // 看不到玩家的时候
        {
            
            if (stateTimer < 0 || Vector2.Distance(player.transform.position, enemy.transform.position) > enemy.playerScanDistance)
            {
                stateMachine.ChangeState(enemy.idleState);
                return;
            }
        }

        if (enemy.IsPlayerDetected() && Vector2.Distance(enemy.transform.position, player.transform.position) < enemy.attackDistance)
        {
            ChangeToIdleAnimation();
            return;
        }

    }

    private bool CanAttack()
    {
        if (Time.time - enemy.lastTimeAttacked >= enemy.attackCooldown && !enemy.isKnockbacked)
        {
            enemy.attackCooldown = Random.Range(enemy.minAttackCooldown, enemy.maxAttackCooldown);
            return true;
        }

        return false;
    }

    private bool CanJump()
    {
        if (!enemy.GroundBehindCheck() || enemy.WallBehindCheck())
        {
            return false;
        }

        if (Time.time - enemy.lastTimeJumped >= enemy.jumpCooldown && !enemy.isKnockbacked)
        {
            return true;
        }

        return false;
    }

    private void ChangeToIdleAnimation()
    {
        anim.SetBool("Move", false);
        anim.SetBool("Idle", true);
    }

    private void FacePlayer()
    {
        if (player.transform.position.x < enemy.transform.position.x)
        {
            if (enemy.facingDirection != -1)
            {
                enemy.Flip();
            }
        }

        if (player.transform.position.x > enemy.transform.position.x)
        {
            if (enemy.facingDirection != 1)
            {
                enemy.Flip();
            }
        }
    }
}
