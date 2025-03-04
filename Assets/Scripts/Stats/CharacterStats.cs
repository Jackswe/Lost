using System.Collections;
using TMPro;
using UnityEngine;

public enum StatType
{
    strength,   // ����
    agility,// ����
    intelligence,// ����
    vitality,// ����
    damage,// �˺�
    critChance,// ������
    critPower,// �����˺�
    maxHP,// �������ֵ
    armor,// ����
    evasion,// ������
    magicResistance,// ħ������
    fireDamage,// �����˺�
    iceDamage,// �����˺�
    lightningDamage// �׵��˺�
}

public class CharacterStats : MonoBehaviour
{
    [Header("��Ҫ����")]
    public Stat strength;  
    public Stat agility;  
    public Stat intelligence; 
    public Stat vitality;

    [Header("�������ԣ����Ѫ�������ף��￹���������� ������")]
    public Stat maxHP;
    public Stat armor;
    public Stat evasion;
    public Stat magicResistance;

    [Header("��������")]
    public Stat damage;
    public Stat critChance;
    public Stat critPower;

    [Header("ħ������")]
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightningDamage;

    [Header("�쳣״̬")]
    public bool isIgnited; 
    public bool isChilled; 
    public bool isShocked; 
    public Stat igniteDuration;
    public Stat chillDuration;
    public Stat shockDuration;

    // ���缼��
    [SerializeField] private GameObject thunderStrikePrefab;
    private int thunderStrikeDamage;

    
    public bool isInvincible { get; private set; }

    [Space]
    public int currentHP;
    // �쳣״̬��ʱ��
    private float ignitedAilmentTimer;
    private float chilledAilmentTimer;
    private float shockedAilmentTimer;

    private float ignitedDamageCooldown = 0.3f;
    private float ignitedDamageTimer;
    private int igniteDamage; 

    public bool isVulnerable { get; private set; }
    public bool isDead { get; private set; }

    // �������Լ���
    //total_evasion = evasion + agility + [attacker shock effect];   ���� + ���� +�����˵����״̬Ч����
    //total_damage = (damage + strength) * [total_crit_power] - target.armor * [target chill effect]; ���˺� + ������* [�����˺��ٷֱ�] - Ŀ�껤�� * [�Է�����״̬Ч��]
    //total_crit_chance = critChance + agility;  �ܱ����� = ������ + ����
    //total_crit_power = critPower + strength;  �ܱ��� = ���� + ����
    //total_magic_damage = (fireDamage + iceDamage + lightningDamage + intelligence) - (target.magicResistance + 3 * target.intelligence);  ���������˺� + ������- �����˷��� + 3 * ���������� 


    public System.Action onHealthChanged;


    protected EntityFX fx;

    protected virtual void Awake()
    {
        fx = GetComponent<EntityFX>();
    }

    protected virtual void Start()
    {
        currentHP = getMaxHP();
        critPower.SetDefaultValue(150);

        //HPBarCanBeInitialized = true;
    }

    protected virtual void Update()
    {
        ignitedAilmentTimer -= Time.deltaTime;
        chilledAilmentTimer -= Time.deltaTime;
        shockedAilmentTimer -= Time.deltaTime;

        ignitedDamageTimer -= Time.deltaTime;

        if (ignitedAilmentTimer < 0)
        {
            isIgnited = false;
        }

        if (chilledAilmentTimer < 0)
        {
            isChilled = false;
        }

        if (shockedAilmentTimer < 0)
        {
            isShocked = false;
        }

        if (isIgnited)
        {
            DealIgniteDamage();
        }

    }



    public virtual void DoDamge(CharacterStats _targetStats)
    {
        if (_targetStats.isInvincible)
        {
            return;
        }

        if (TargetCanEvadeThisAttack(_targetStats))
        {
            return;
        }

        int _totalDamage = damage.GetValue() + strength.GetValue();

        bool crit = CanCrit();

        if (crit)
        {
            Debug.Log("Critical Attack!");
            _totalDamage = CalculatCritDamage(_totalDamage);
        }

        fx.CreateHitFX(_targetStats.transform, crit);

        _totalDamage = CheckTargetArmor(_targetStats, _totalDamage);

        _totalDamage = CheckTargetVulnerability(_targetStats, _totalDamage);

        _targetStats.TakeDamage(_totalDamage, transform, _targetStats.transform, crit);

    }



    public virtual void TakeDamage(int _damage, Transform _attacker, Transform _attackee, bool _isCrit)
    {
        if (isInvincible)
        {
            return;
        }

        DecreaseHPBy(_damage, _isCrit);


        _attackee.GetComponent<Entity>()?.DamageFlashEffect();
        _attackee.GetComponent<Entity>()?.DamageKnockbackEffect(_attacker, _attackee);

        if (currentHP <= 0 && !isDead)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        Debug.Log($"{gameObject.name} is Dead");
    }

    public virtual void DieFromFalling()
    {
        if (isDead)
        {
            return;
        }

        currentHP = 0;

        if (onHealthChanged != null)
        {
            onHealthChanged();
        }

        Die();
    }

    public void BecomeInvincible(bool _invincible)
    {
        isInvincible = _invincible;
    }




    #region Magic and Ailments
    public virtual void DoMagicDamage(CharacterStats _targetStats, Transform _attacker)
    {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightningDamage = lightningDamage.GetValue();

        int _totalMagicDamage = _fireDamage + _iceDamage + _lightningDamage + intelligence.GetValue();
        _totalMagicDamage = CheckTargetMagicResistance(_targetStats, _totalMagicDamage);

        _targetStats.TakeDamage(_totalMagicDamage, _attacker, _targetStats.transform, false);

        // ������һ�������˺�����0���ų������쳣
        if (Mathf.Max(_fireDamage, _iceDamage, _lightningDamage) <= 0)
        {
            return;
        }

        AttemptToApplyAilments(_targetStats, _fireDamage, _iceDamage, _lightningDamage);
    }

    private void AttemptToApplyAilments(CharacterStats _targetStats, int _fireDamage, int _iceDamage, int _lightningDamage)
    {
        // �����˺������쳣״̬
        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightningDamage;
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _lightningDamage;
        bool canApplyShock = _lightningDamage > _fireDamage && _lightningDamage > _iceDamage;

        // ���ȫ��һ�� ���������һ��
        if (!canApplyIgnite && !canApplyChill && !canApplyShock)
        {
            if (Random.value < 0.3f && _fireDamage > 0)
            {
                canApplyIgnite = true;

                if (canApplyIgnite)
                {
                    _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * 0.2f));
                }

                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }

            if (Random.value < 0.5f && _iceDamage > 0)
            {
                canApplyChill = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }

            if (Random.value < 0.5f && _lightningDamage > 0)
            {
                canApplyShock = true;

                if (canApplyShock)
                {
                    _targetStats.SetupThunderStrikeDamage(Mathf.RoundToInt(_lightningDamage * 0.2f));
                }

                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }
        }

        // ���Ƶ�ȼ�˺�
        //ignite_damage = fire_damage * 0.2
        if (canApplyIgnite)
        {
            _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * 0.2f));
        }
        // �������
        if (canApplyShock)
        {
            _targetStats.SetupThunderStrikeDamage(Mathf.RoundToInt(_lightningDamage * 0.2f));
        }

        _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
    }

    public void ApplyAilments(bool _ignite, bool _chill, bool _shock)
    {
        bool canApplyIgnite = !isIgnited && !isChilled && !isShocked;
        bool canApplyChill = !isIgnited && !isChilled && !isShocked;
        bool canApplyShock = !isIgnited && !isChilled;

        if (_ignite && canApplyIgnite)
        {
            isIgnited = _ignite;
            ignitedAilmentTimer = igniteDuration.GetValue();

            //fx.EnableIgniteFXForTime(ignitedAilmentTimer);
            StartCoroutine(fx.EnableIgniteFXForTime_Coroutine(ignitedAilmentTimer));
        }

        if (_chill && canApplyChill)
        {
            isChilled = _chill;
            chilledAilmentTimer = chillDuration.GetValue();

            //fx.EnableChillFXForTime(chilledAilmentTimer);
            StartCoroutine(fx.EnableChillFXForTime_Coroutine(chilledAilmentTimer));

            float _slowPercentage = 0.2f;
            GetComponent<Entity>()?.SlowSpeedBy(_slowPercentage, chillDuration.GetValue());
        }

        if (_shock && canApplyShock)
        {
            if (!isShocked)
            {
                ApplyShockAilment(_shock);
            }
            else 
            {
                if (GetComponent<Player>() != null)
                {
                    return;
                }

                GenerateThunderStrikeAndHitClosestEnemy(7.5f);

            }
        }

        if (isIgnited)
        {
            Debug.Log($"{gameObject.name} is Ignited");
        }
        else if (isChilled)
        {
            Debug.Log($"{gameObject.name} is Chilled");
        }
        else if (isShocked)
        {
            Debug.Log($"{gameObject.name} is Shocked");
        }
    }

    public void ApplyShockAilment(bool _shock)
    {
        // �Ѿ��ܵ��׻�Ч�����ܵ���
        if (isShocked)
        {
            return;
        }

        isShocked = _shock;
        shockedAilmentTimer = shockDuration.GetValue();

        //fx.EnableShockFXForTime(shockedAilmentTimer);
        StartCoroutine(fx.EnableShockFXForTime_Coroutine(shockedAilmentTimer));
    }

    private void GenerateThunderStrikeAndHitClosestEnemy(float _targetScanRadius)
    {
        // �ҵ���Χ�������Ŀ��
        Transform closestEnemy = null;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _targetScanRadius);

        float closestDistanceToEnemy = Mathf.Infinity;

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && Vector2.Distance(transform.position, hit.transform.position) > 1)
            {
                float currentDistanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

                if (currentDistanceToEnemy < closestDistanceToEnemy)
                {
                    closestDistanceToEnemy = currentDistanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }
        }

        //���û���ҵ�
        //�򽫼����ͷŵ���Ϊ�Լ�
        if (closestEnemy == null)
        {
            closestEnemy = transform;
        }

        if (closestEnemy != null)
        {
            GameObject newThunderStrike = Instantiate(thunderStrikePrefab, transform.position, Quaternion.identity);

            newThunderStrike.GetComponent<Skill_ThunderStrikeController>()?.Setup(thunderStrikeDamage, closestEnemy.GetComponent<CharacterStats>());
        }
    }

    private void DealIgniteDamage()
    {
        if (ignitedDamageTimer < 0)
        {
            Debug.Log($"Take burn damage {igniteDamage}");
            DecreaseHPBy(igniteDamage, false);

            if (currentHP <= 0 && !isDead)
            {
                Die();
            }

            ignitedDamageTimer = ignitedDamageCooldown;
        }
    }

    public void SetupIgniteDamage(int _igniteDamage)
    {
        igniteDamage = _igniteDamage;
    }

    public void SetupThunderStrikeDamage(int _thunderStrikeDamage)
    {
        thunderStrikeDamage = _thunderStrikeDamage;
    }
    #endregion



    #region 
    protected int CheckTargetArmor(CharacterStats _targetStats, int _totalDamage)
    {
        if (_targetStats.isChilled)
        {
            _totalDamage -= Mathf.RoundToInt(_targetStats.armor.GetValue() * 0.8f);
        }
        else
        {
            _totalDamage -= _targetStats.armor.GetValue();
        }


        _totalDamage = Mathf.Clamp(_totalDamage, 0, int.MaxValue);
        return _totalDamage;
    }

    private int CheckTargetMagicResistance(CharacterStats _targetStats, int _totalMagicDamage)
    {
        _totalMagicDamage -= _targetStats.magicResistance.GetValue() + (3 * _targetStats.intelligence.GetValue());

        _totalMagicDamage = Mathf.Clamp(_totalMagicDamage, 0, int.MaxValue);
        return _totalMagicDamage;
    }

    protected bool TargetCanEvadeThisAttack(CharacterStats _targetStats)
    {
        int _totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();

        if (isShocked)
        {
            _totalEvasion += 20;
        }

        if (Random.Range(0, 100) < _totalEvasion)
        {
            _targetStats.OnEvasion();
            return true;
        }

        return false;
    }

    protected bool CanCrit()
    {
        int _totalCritChance = critChance.GetValue() + agility.GetValue();

        if (Random.Range(0, 100) <= _totalCritChance)
        {
            return true;
        }

        return false;
    }

    protected int CalculatCritDamage(int _damage)
    {
        float _totalCritPower = (critPower.GetValue() + strength.GetValue()) * 0.01f;

        float critDamage = _damage * _totalCritPower;

        return Mathf.RoundToInt(critDamage);
    }

    public int CheckTargetVulnerability(CharacterStats _targetStats, int _totalDamage)
    {
        if (_targetStats.isVulnerable)
        {
            _totalDamage = Mathf.RoundToInt(_totalDamage * 1.1f);
        }

        return _totalDamage;
    }

    public virtual void OnEvasion()
    {

    }


    #region HP
    public virtual int DecreaseHPBy(int _takenDamage, bool _isCrit)
    {
        currentHP -= _takenDamage;

        if (_takenDamage > 0)
        {
            GameObject popUpText = fx.CreatePopUpText(_takenDamage.ToString());

            if (_isCrit)
            {
                popUpText.GetComponent<TextMeshPro>().color = Color.yellow;
                popUpText.GetComponent<TextMeshPro>().fontSize = 12;
            }
        }

        Debug.Log($"{gameObject.name} takes {_takenDamage} damage");

        if (onHealthChanged != null)
        {
            onHealthChanged();
        }

        return _takenDamage;
    }

    public virtual void IncreaseHPBy(int _HP)
    {
        currentHP += _HP;

        if (currentHP > getMaxHP())
        {
            currentHP = getMaxHP();
        }

        if (onHealthChanged != null)
        {
            onHealthChanged();
        }
    }
    #endregion

    #endregion

    public void BecomeVulnerableForTime(float _seconds)
    {
        StartCoroutine(BecomeVulnerableForTime_Coroutine(_seconds));
    }

    private IEnumerator BecomeVulnerableForTime_Coroutine(float _duration)
    {
        isVulnerable = true;
        yield return new WaitForSeconds(_duration);
        isVulnerable = false;
    }

    public virtual void IncreaseStatByTime(Stat _statToModify, int _modifier, float _duration)
    {
        StartCoroutine(StatModify_Coroutine(_statToModify, _modifier, _duration));
    }

    private IEnumerator StatModify_Coroutine(Stat _statToModify, int _modifier, float _duration)
    {
        _statToModify.AddModifier(_modifier);
        Inventory.instance.UpdateStatUI();

        yield return new WaitForSeconds(_duration);

        _statToModify.RemoveModifier(_modifier);
        Inventory.instance.UpdateStatUI();
    }

    public Stat GetStatByType(StatType _statType)
    {
        Stat stat = null;

        switch (_statType)
        {
            case StatType.strength: stat = strength; break;
            case StatType.agility: stat = agility; break;
            case StatType.intelligence: stat = intelligence; break;
            case StatType.vitality: stat = vitality; break;
            case StatType.damage: stat = damage; break;
            case StatType.critChance: stat = critChance; break;
            case StatType.critPower: stat = critPower; break;
            case StatType.maxHP: stat = maxHP; break;
            case StatType.armor: stat = armor; break;
            case StatType.evasion: stat = evasion; break;
            case StatType.magicResistance: stat = magicResistance; break;
            case StatType.fireDamage: stat = fireDamage; break;
            case StatType.iceDamage: stat = iceDamage; break;
            case StatType.lightningDamage: stat = lightningDamage; break;
        }

        return stat;
    }


    #region 
    public int getMaxHP()
    {
        return maxHP.GetValue() + vitality.GetValue() * 5;
    }

    public int GetDamage()
    {
        return damage.GetValue() + strength.GetValue();
    }

    public int GetCritPower()
    {
        return critPower.GetValue() + strength.GetValue();
    }

    public int GetCritChance()
    {
        return critChance.GetValue() + agility.GetValue();
    }

    public int GetEvasion()
    {
        return evasion.GetValue() + agility.GetValue();
    }

    public int GetMagicResistance()
    {
        return magicResistance.GetValue() + intelligence.GetValue() * 3;
    }

    public int GetFireDamage()
    {
        return fireDamage.GetValue() + intelligence.GetValue();
    }

    public int GetIceDamage()
    {
        return iceDamage.GetValue() + intelligence.GetValue();
    }

    public int GetLightningDamage()
    {
        return lightningDamage.GetValue() + intelligence.GetValue();
    }
    #endregion
}
