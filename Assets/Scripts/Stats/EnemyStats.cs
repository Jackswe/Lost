using UnityEngine;

public class EnemyStats : CharacterStats
{
    private Enemy enemy;
    private ItemDrop itemDropSystem;

    public Stat currencyDropAmount;

    [Header("���˵ȼ�")]
    [SerializeField] private int enemyLevel = 1;

    [Header("�������������ٷֱ�")]
    [Range(0f, 1f)]
    [SerializeField] private float percentageModifier = 0.4f;

    protected override void Start()
    {
        //currencyDropAmount.SetDefaultValue(100);

        ModifyAllStatsAccordingToEnemyLevel();

        base.Start();

        enemy = GetComponent<Enemy>();
        itemDropSystem = GetComponent<ItemDrop>();

    }


    public override void TakeDamage(int _damage, Transform _attacker, Transform _attackee, bool _isCrit)
    {
        base.TakeDamage(_damage, _attacker, _attackee, _isCrit);

        // ��δ�����Ϊ���ڵ����ܵ��˺���ʱ��������ϵ��˵�ǰ��״̬����ս��״̬
        enemy.GetIntoBattleState();
    }

    protected override void Die()
    {
        base.Die();

        enemy.Die();

        itemDropSystem.GenrateDrop();

        PlayerManager.instance.currency += currencyDropAmount.GetValue();

        Destroy(gameObject, 3f);
    }

    public void ZeroHP()
    {
        currentHP = 0;

        base.Die();

        if (onHealthChanged != null)
        {
            onHealthChanged();
        }
    }

    public void DropCurrencyAndItem()
    {
        itemDropSystem.GenrateDrop();

        PlayerManager.instance.currency += currencyDropAmount.GetValue();
    }

    private void ModifyAllStatsAccordingToEnemyLevel()
    {
        ModifyStatAccordingToEnemyLevel(strength);
        ModifyStatAccordingToEnemyLevel(agility);
        ModifyStatAccordingToEnemyLevel(intelligence);
        ModifyStatAccordingToEnemyLevel(vitality);

        ModifyStatAccordingToEnemyLevel(damage);
        ModifyStatAccordingToEnemyLevel(critChance);
        ModifyStatAccordingToEnemyLevel(critPower);

        ModifyStatAccordingToEnemyLevel(maxHP);
        ModifyStatAccordingToEnemyLevel(armor);
        ModifyStatAccordingToEnemyLevel(evasion);
        ModifyStatAccordingToEnemyLevel(magicResistance);

        ModifyStatAccordingToEnemyLevel(fireDamage);
        ModifyStatAccordingToEnemyLevel(iceDamage);
        ModifyStatAccordingToEnemyLevel(lightningDamage);

        ModifyStatAccordingToEnemyLevel(currencyDropAmount);
    }

    private void ModifyStatAccordingToEnemyLevel(Stat _stat)
    {
        for (int i = 1; i < enemyLevel; i++)
        {
            float _modifier = _stat.GetValue() * percentageModifier;
            _stat.AddModifier(Mathf.RoundToInt(_modifier));
        }
    }
}
