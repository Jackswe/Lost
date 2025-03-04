using System.Collections.Generic;
using UnityEngine;

public enum EquipmentType
{
    Weapon,     // 武器
    Armor,      // 护甲
    Charm,      // 饰品
    Flask       // 药水
}

[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Equipment")]
public class ItemData_Equipment : ItemData
{
    public EquipmentType equipmentType;

    [Header("装备道具效果")]
    //public float itemCooldown;
    //public bool itemUsed { get; set; }
    //public float itemLastUseTime { get; set; }
    public ItemEffect[] itemEffects;

    [Header("主属性")]
    public int strength;  //damage + 1; crit_power + 1%
    public int agility;  //evasion + 1%; crit_chance + 1%
    public int intelligence; //magic_damage + 1; magic_resistance + 3
    public int vitaliy; //maxHP + 5

    [Header("防御属性")]
    public int maxHP;
    public int armor;
    public int evasion;
    public int magicResistance;

    [Header("进攻属性")]
    public int damage;
    public int critChance;
    public int critPower; 

    [Header("魔法属性")]
    public int fireDamage;
    public int iceDamage;
    public int lightningDamage;

    [Header("制作材料")]
    public List<InventorySlot> requiredCraftMaterials;

    private int statInfoLength;
    //private int minStatInfoLength = 5;

    public void AddModifiers()
    {
        // 装备的时候修改属性
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        playerStats.strength.AddModifier(strength);
        playerStats.agility.AddModifier(agility);
        playerStats.intelligence.AddModifier(intelligence);
        playerStats.vitality.AddModifier(vitaliy);

        playerStats.maxHP.AddModifier(maxHP);
        playerStats.armor.AddModifier(armor);
        playerStats.evasion.AddModifier(evasion);
        playerStats.magicResistance.AddModifier(magicResistance);

        playerStats.damage.AddModifier(damage);
        playerStats.critChance.AddModifier(critChance);
        playerStats.critPower.AddModifier(critPower);

        playerStats.fireDamage.AddModifier(fireDamage);
        playerStats.iceDamage.AddModifier(iceDamage);
        playerStats.lightningDamage.AddModifier(lightningDamage);
    }

    public void RemoveModifiers()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        playerStats.strength.RemoveModifier(strength);
        playerStats.agility.RemoveModifier(agility);
        playerStats.intelligence.RemoveModifier(intelligence);
        playerStats.vitality.RemoveModifier(vitaliy);

        playerStats.maxHP.RemoveModifier(maxHP);
        playerStats.armor.RemoveModifier(armor);
        playerStats.evasion.RemoveModifier(evasion);
        playerStats.magicResistance.RemoveModifier(magicResistance);

        playerStats.damage.RemoveModifier(damage);
        playerStats.critChance.RemoveModifier(critChance);
        playerStats.critPower.RemoveModifier(critPower);

        playerStats.fireDamage.RemoveModifier(fireDamage);
        playerStats.iceDamage.RemoveModifier(iceDamage);
        playerStats.lightningDamage.RemoveModifier(lightningDamage);
    }

    //private void ExecuteItemEffect(Transform _spawnTransform)
    //{
    //    foreach (var effect in itemEffects)
    //    {
    //        effect.ExecuteEffect(_spawnTransform);
    //    }
    //}

    //public void ExecuteItemEffect_NoHitNeeded()
    //{
    //    foreach (var effect in itemEffects)
    //    {
    //        effect.ExecuteEffect_NoHitNeeded();
    //    }
    //}

    //private void ReleaseSwordArcane()
    //{
    //    foreach (var effect in itemEffects)
    //    {
    //        effect.ReleaseSwordArcane();
    //    }
    //}

    /// <summary>
    /// 更新道具使用状态
    /// </summary>
    public void RefreshUseState()
    {
        //itemUsed = false;
        //itemLastUseTime = 0;

        foreach (var effect in itemEffects)
        {
            effect.effectUsed = false;
            effect.effectLastUseTime = 0;
        }
    }

    public void ExecuteItemEffect_ConsiderCooldown(Transform _spawnTransform)
    {
        // 触发道具效果之前 检查道具效果冷却时间 有多个效果的话需要每个单独检测 只有冷却的效果才能够进行触发

        foreach (var effect in itemEffects)
        {
            bool canUseEffect = Time.time >= effect.effectLastUseTime + effect.effectCooldown;

            if (canUseEffect || !effect.effectUsed)
            {
                //ExecuteItemEffect(_spawnTransform);
                effect.ExecuteEffect(_spawnTransform);
                effect.effectLastUseTime = Time.time;
                effect.effectUsed = true;
                Inventory.instance.UpdateStatUI();
            }
            else
            {
                Debug.Log("道具效果正在冷却！！！");
            }
        }
    }

    public void ReleaseSwordArcane_ConsiderCooldown()
    {
       
        foreach (var effect in itemEffects)
        {
            bool canUseEffect = Time.time >= effect.effectLastUseTime + effect.effectCooldown;

            if (canUseEffect || !effect.effectUsed)
            {
                effect.ReleaseSwordArcane();
                effect.effectLastUseTime = Time.time;
                effect.effectUsed = true;
                Inventory.instance.UpdateStatUI();
            }
            else
            {
                Debug.Log("道具效果正在冷却！！！");
            }
        }
    }

    public override string GetItemStatInfoAndEffectDescription()
    {
        sb.Length = 0;
        statInfoLength = 0;

        AddItemStatInfo(strength, "力量");
        AddItemStatInfo(agility, "敏捷");
        AddItemStatInfo(intelligence, "智力");
        AddItemStatInfo(vitaliy, "耐力");

        AddItemStatInfo(damage, "伤害");
        AddItemStatInfo(critChance, "暴击率");
        AddItemStatInfo(critPower, "暴击伤害");

        AddItemStatInfo(maxHP, "HP");
        AddItemStatInfo(evasion, "闪避率");
        AddItemStatInfo(armor, "护甲");
        AddItemStatInfo(magicResistance, "魔法抗性");

        AddItemStatInfo(fireDamage, "火焰伤害");
        AddItemStatInfo(iceDamage, "冰冻伤害");
        AddItemStatInfo(lightningDamage, "雷电伤害");

        if (itemEffects.Length > 0 && statInfoLength > 0)
        {
            if (itemEffects[0].effectDescription.Length > 0)
            {
                sb.AppendLine();
            }
        }

        for (int i = 0; i < itemEffects.Length; i++)
        {
            sb.AppendLine();

            if (LanguageManager.instance.localeID == 0)
            {
                if (itemEffects[i].effectDescription.Length > 0)
                {
                    sb.Append($"[unique effect]\n{itemEffects[i].effectDescription}\n");
                }
            }
            else if (LanguageManager.instance.localeID == 1)
            {
                if (itemEffects[i].effectDescription_Chinese.Length > 0)
                {
                    sb.Append($"[固有效果]\n{itemEffects[i].effectDescription_Chinese}\n");
                }
            }

            statInfoLength++;
        }

        if (sb.ToString()[sb.Length - 1] == '\n')
        {
            sb.Remove(sb.Length - 1, 1);
        }


        //if (statInfoLength < minStatInfoLength)
        //{
        //    int _numberOfLinesToApped = minStatInfoLength - statInfoLength;
        //    if (statInfoLength == 0)
        //    {
        //        _numberOfLinesToApped--;
        //    }

        //    for (int i = 0; i < _numberOfLinesToApped; i++)
        //    {
        //        sb.AppendLine();
        //        sb.Append("");
        //    }
        //}

        sb.AppendLine();
        sb.Append("");
        sb.AppendLine();
        sb.Append("");

        return sb.ToString();
    }

    private void AddItemStatInfo(int _statValue, string _statName)
    {
        if (_statValue != 0)
        {
            if (sb.Length > 0)
            {
                sb.AppendLine();
            }

            if (_statValue > 0)
            {
                sb.Append($"+ {_statValue} {_statName}");
                statInfoLength++;
            }
        }
    }
}
