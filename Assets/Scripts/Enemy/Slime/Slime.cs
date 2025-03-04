using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// 史莱姆类型
public enum SlimeType
{
    big,
    medium,
    small
}

// 史莱姆敌人行为
public class Slime : Enemy
{
    [Header("史莱姆属性")]
    [SerializeField] private SlimeType slimeType;
    [SerializeField] private int amoutOfSlimeToSpawnAfterDeath;
    [SerializeField] private GameObject slimePrefab;
    [SerializeField] private Vector2 minSlimeSpawnSpeed;
    [SerializeField] private Vector2 maxSlimeSpawnSpeed;

    #region States
    public SlimeIdleState idleState {  get; private set; }
    public SlimeMoveState moveState { get; private set; }
    public SlimeBattleState battleState { get; private set; }
    public SlimeAttackState attackState { get; private set; }
    public SlimeStunnedState stunnedState { get; private set; }
    public SlimeDeathState deathState { get; private set; }
    #endregion

    protected override void Awake()
    {
        base.Awake();

        idleState = new SlimeIdleState(this, stateMachine, "Idle", this);
        moveState = new SlimeMoveState(this, stateMachine, "Move", this);
        battleState = new SlimeBattleState(this, stateMachine, "Move", this);
        attackState = new SlimeAttackState(this, stateMachine, "Attack", this);
        stunnedState = new SlimeStunnedState(this, stateMachine, "Stunned", this);
        deathState = new SlimeDeathState(this, stateMachine, "Idle", this);

        SetupDefaultFacingDirection(-1);
    }

    protected override void Start()
    {
        base.Start();

        InitializeLastTimeInfo();
        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();

        if (stateMachine.currentState != attackState)
        {
            CloseCounterAttackWindow();
        }
    }

    public override bool CanBeStunnedByCounterAttack()
    {
        if (base.CanBeStunnedByCounterAttack())
        {
            stateMachine.ChangeState(stunnedState);
            return true;
        }

        return false;
    }

    public override void Die()
    {
        base.Die();

        stateMachine.ChangeState(deathState);

        if (slimeType == SlimeType.small)
        {
            return;
        }

        SpawnSlime(amoutOfSlimeToSpawnAfterDeath, slimePrefab);
    }

    public override void GetIntoBattleState()
    {
        //if (stateMachine.currentState == battleState || stateMachine.currentState == attackState)
        //{
        //    return;
        //}

        if (slimeType == SlimeType.big && (stateMachine.currentState == battleState || stateMachine.currentState == attackState))
        {
            return;
        }

        if (stateMachine.currentState != battleState && stateMachine.currentState != stunnedState && stateMachine.currentState != deathState)
        {
            stateMachine.ChangeState(battleState);
        }
    }

    private void SpawnSlime(int _amountOfSlimeToSpawn, GameObject _slimePrefab)
    {
        for (int i = 0; i < _amountOfSlimeToSpawn; i++)
        {
            GameObject newSlime = Instantiate(_slimePrefab, transform.position, Quaternion.identity);

            newSlime.GetComponent<Slime>()?.SetupSpawnedSlime(facingDirection);
        }
    }

    public void SetupSpawnedSlime(int _facingDirection)
    {
       
        if (facingDirection != _facingDirection)
        {
            Flip();
        }

        float xVelocity = Random.Range(minSlimeSpawnSpeed.x, maxSlimeSpawnSpeed.x);
        float yVelocity = Random.Range(minSlimeSpawnSpeed.y, maxSlimeSpawnSpeed.y);

        isKnockbacked = true;

        GetComponent<Rigidbody2D>().velocity = new Vector2(xVelocity * -facingDirection, yVelocity);

        Invoke("CancelKnockback", 1.5f);
    }

    private void CancelKnockback()
    {
        isKnockbacked = false;
    }

    protected override void InitializeLastTimeInfo()
    {
        lastTimeAttacked = 0;
    }
}
