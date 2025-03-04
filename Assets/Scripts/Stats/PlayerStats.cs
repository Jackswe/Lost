using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    private Player player;
    private PlayerFX playerFX;

    private float playerDefaultMoveSpeed;

    protected override void Awake()
    {
        base.Awake();
        playerFX = GetComponent<PlayerFX>();
    }

    protected override void Start()
    {
        base.Start();

        player = GetComponent<Player>();
        playerDefaultMoveSpeed = player.moveSpeed;
    }

    public override void DoDamge(CharacterStats _targetStats)
    {
        base.DoDamge(_targetStats);

        if (_targetStats.GetComponent<Enemy>() != null)
        {
            Player player = PlayerManager.instance.player;
            int playerComboCounter = player.primaryAttackState.comboCounter;

            if (player.stateMachine.currentState == player.primaryAttackState)
            {
                if (playerComboCounter <= 1)
                {
                    playerFX.ScreenShake(playerFX.shakeDirection_light);
                }
                else
                {
                    playerFX.ScreenShake(playerFX.shakeDirection_medium);
                }
            }
            //else 
            //{
            //    fx.ScreenShake(fx.shakeDirection_medium);
            //}

        }
    }

    public override void TakeDamage(int _damage, Transform _attacker, Transform _attackee, bool _isCrit)
    {
        //if (isInvincible)
        //{
        //    return;
        //}

        int takenDamage = DecreaseHPBy(_damage, _isCrit);


        _attackee.GetComponent<Entity>()?.DamageFlashEffect();

        SlowerPlayerMoveSpeedForTime(0.2f);

        //当玩家收到的伤害大于最大血量的30% 则造成击退效果
        if (takenDamage >= player.stats.getMaxHP() * 0.3f)
        {
            _attackee.GetComponent<Entity>()?.DamageKnockbackEffect(_attacker, _attackee);
            player.fx.ScreenShake(player.fx.shakeDirection_heavy);
        }

        if (currentHP <= 0 && !isDead)
        {
            Die();
        }
    }

    private void SlowerPlayerMoveSpeedForTime(float _duration)
    {
        float defaultMoveSpeed = player.moveSpeed;

        player.moveSpeed = player.moveSpeed * _duration;

        Invoke("ReturnToDefaultMoveSpeed", _duration);
    }

    private void ReturnToDefaultMoveSpeed()
    {
        player.moveSpeed = playerDefaultMoveSpeed;
    }

    protected override void Die()
    {
        base.Die();

        player.Die();
        // 掉落所有金币
        GameManager.instance.droppedCurrencyAmount = PlayerManager.instance.GetCurrentCurrency();
        PlayerManager.instance.currency = 0;

        GetComponent<PlayerItemDrop>()?.GenrateDrop();
    }

    /// <summary>
    /// 减少血量
    /// </summary>
    /// <param name="_takenDamage"></param>
    /// <param name="_isCrit"> s是否暴击</param>
    /// <returns></returns>
    public override int DecreaseHPBy(int _takenDamage, bool _isCrit)
    {
        base.DecreaseHPBy(_takenDamage, _isCrit);

        int randomIndex = Random.Range(34, 36);
        AudioManager.instance.PlaySFX(randomIndex, player.transform);

        ItemData_Equipment currentArmor = Inventory.instance.GetEquippedEquipmentByType(EquipmentType.Armor);

        if (currentArmor != null)
        {
            //currentArmor.ExecuteItemEffect(player.transform);
            Inventory.instance.UseArmorEffect_ConsiderCooldown(player.transform);
        }

        return _takenDamage;
    }

    public override void OnEvasion()
    {
        player.skill.dodge.CreateMirageOnDodge();
    }

    public void CloneDoDamage(CharacterStats _targetStats, float _cloneAttackDamageMultipler, Transform _cloneTransform)
    {
        if (TargetCanEvadeThisAttack(_targetStats))
        {
            return;
        }

        int _totalDamage = damage.GetValue() + strength.GetValue();

        bool crit = CanCrit();

        if (crit)
        {
            _totalDamage = CalculatCritDamage(_totalDamage);
        }

        fx.CreateHitFX(_targetStats.transform, crit);

        if (_cloneAttackDamageMultipler > 0)
        {
            _totalDamage = Mathf.RoundToInt(_totalDamage * _cloneAttackDamageMultipler);
        }

        _totalDamage = CheckTargetArmor(_targetStats, _totalDamage);
        _totalDamage = CheckTargetVulnerability(_targetStats, _totalDamage);

        _targetStats.TakeDamage(_totalDamage, _cloneTransform, _targetStats.transform, crit);
    }
}
