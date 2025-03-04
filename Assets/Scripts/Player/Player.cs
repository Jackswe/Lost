using System.Collections;
using UnityEngine;

public class Player : Entity
{
    public SkillManager skill { get; private set; }
    public GameObject sword { get; private set; }

    [Header("移动信息")]
    public float moveSpeed;  // 移速
    public float jumpForce;     // 跳跃力
    public float wallJumpXSpeed;    // 蹬墙跳横向速度
    public float wallJumpDuration;  // 蹬墙跳持续时间
    private float defaultMoveSpeed; // 默认移速
    private float defaultJumpForce; // 默认跳跃力

    [Header("攻击信息")]
    public Vector2[] attackMovement;
    public float counterAttackDuration = 0.2f;

    [Header("冲刺信息")]
    public float dashSpeed;
    public float dashDuration;
    public float dashDirection { get; private set; }
    private float defaultDashSpeed;

    [Header("环境检测")]
    [SerializeField] private BoxCollider2D pitCheck;
    [SerializeField] private BoxCollider2D downablePlatformCheck;

    // 是否在坑附近
    public bool isNearPit { get; set; }
    public DownablePlatform lastPlatform { get; set; }
    public bool isOnPlatform { get; set; } = false;

    // 标志玩家是否正处于繁忙状态（如攻击、冲刺等），用于限制其他输入。
    public bool isBusy { get; private set; }
    public PlayerFX fx { get; private set; }

    #region States and Statemachine
    public PlayerStateMachine stateMachine { get; private set; }

    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerWallSlideState wallSlideState { get; private set; }
    public PlayerWallJumpState wallJumpState { get; private set; }
    public PlayerPrimaryAttackState primaryAttackState { get; private set; }
    public PlayerAirLaunchAttackState airLaunchAttackState { get; private set; }
    public PlayerDownStrikeState downStrikeState { get; private set; }
    public PlayerCounterAttackState counterAttackState { get; private set; }
    public PlayerAimSwordState aimSwordState { get; private set; }
    public PlayerThrowSwordState throwSwordState { get; private set; }
    public PlayerCatchSwordState catchSwordState { get; private set; }
    public PlayerReleaseBlackholeSkillState blackholeSkillState { get; private set; }
    public PlayerDeathState deathState { get; private set; }
    #endregion

    protected override void Awake()
    {
        base.Awake();

        stateMachine = new PlayerStateMachine();
        fx = GetComponent<PlayerFX>();

        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState = new PlayerAirState(this, stateMachine, "Jump");
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        wallSlideState = new PlayerWallSlideState(this, stateMachine, "WallSlide");
        wallJumpState = new PlayerWallJumpState(this, stateMachine, "Jump");
        primaryAttackState = new PlayerPrimaryAttackState(this, stateMachine, "Attack");
        airLaunchAttackState = new PlayerAirLaunchAttackState(this, stateMachine, "AirLaunchAttack");
        downStrikeState = new PlayerDownStrikeState(this, stateMachine, "DownStrike");
        counterAttackState = new PlayerCounterAttackState(this, stateMachine, "CounterAttack");
        aimSwordState = new PlayerAimSwordState(this, stateMachine, "AimSword");
        throwSwordState = new PlayerThrowSwordState(this, stateMachine, "ThrowSword");
        catchSwordState = new PlayerCatchSwordState(this, stateMachine, "CatchSword");
        blackholeSkillState = new PlayerReleaseBlackholeSkillState(this, stateMachine, "Jump");
        deathState = new PlayerDeathState(this, stateMachine, "Death");
    }

    protected override void Start()
    {
        base.Start();

        skill = SkillManager.instance;

        stateMachine.Initialize(idleState);

        defaultMoveSpeed = moveSpeed;
        defaultJumpForce = jumpForce;
        defaultDashSpeed = dashSpeed;
    }

    protected override void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }

        base.Update();

        stateMachine.currentState.Update();

        if (stats.isDead)
        {
            return;
        }

        CheckForDashInput();

        if (Input.GetKeyDown(/*KeyCode.F*/ KeyBindManager.instance.keybindsDictionary["Crystal"]) && skill.crystal.crystalUnlocked)
        {
            skill.crystal.UseSkillIfAvailable();
        }

        if (Input.GetKeyDown(/*KeyCode.Alpha1*/ KeyBindManager.instance.keybindsDictionary["Flask"]))
        {
            Inventory.instance.UseFlask_ConsiderCooldown(null);
        }

    }

    private void CheckForDashInput()
    {
        if (skill.dash.dashUnlocked == false)
        {
            return;
        }

        if (IsWallDetected())
        {
            return;
        }

        if (Input.GetKeyDown(/*KeyCode.LeftShift*/ KeyBindManager.instance.keybindsDictionary["Dash"]) && SkillManager.instance.dash.UseSkillIfAvailable())
        {
            if (stateMachine.currentState == aimSwordState || stateMachine.currentState == throwSwordState)
            {
                skill.sword.ShowDots(false);
            }

            dashDirection = Input.GetAxisRaw("Horizontal");

            if (dashDirection == 0)
            {
                dashDirection = facingDirection;
            }

            stateMachine.ChangeState(dashState);
        }
    }

    public void AnimationTrigger()
    {
        stateMachine.currentState.AnimationFinishTrigger();
    }

    public void AirLaunchJumpTrigger()
    {
        airLaunchAttackState.SetAirLaunchJumpTrigger();
    }

    public void DownStrikeTrigger()
    {
        downStrikeState.SetFallingStrikeTrigger();
    }

    public void DownStrikeAnimStopTrigger()
    {
        downStrikeState.SetAnimStopTrigger();
    }

    public IEnumerator BusyFor(float _seconds)
    {
        isBusy = true;
        //Debug.Log("Is busy");
        yield return new WaitForSeconds(_seconds);
        isBusy = false;
        //Debug.Log("not busy");
    }

    public void AssignNewSword(GameObject _newSword)
    {
        sword = _newSword;
    }

    public void CatchSword()
    {
        stateMachine.ChangeState(catchSwordState);
        Destroy(sword);
    }

    public bool HasNoSword()
    {
        if (!sword)
        {
            return true;
        }

        sword.GetComponent<SwordSkillController>().ReturnSword();
        return false;
    }


    public override void Die()
    {
        base.Die();

        stateMachine.ChangeState(deathState);
    }

    public override void SlowSpeedBy(float _percentage, float _duration)
    {
        moveSpeed = moveSpeed * (1 - _percentage);
        jumpForce = jumpForce * (1 - _percentage);
        dashSpeed = dashSpeed * (1 - _percentage);
        anim.speed = anim.speed * (1 - _percentage);

        Invoke("ReturnDefaultSpeed", _duration);
    }

    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();

        moveSpeed = defaultMoveSpeed;
        jumpForce = defaultJumpForce;
        dashSpeed = defaultDashSpeed;
    }


    public override void DamageFlashEffect()
    {
        fx.StartCoroutine("FlashFX");
    }

    public void JumpOffPlatform()
    {
        if (isOnPlatform)
        {
            lastPlatform.TurnOffPlatformColliderForTime(0.5f);
        }
    }
}
