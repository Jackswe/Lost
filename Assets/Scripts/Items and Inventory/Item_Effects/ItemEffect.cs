using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 描述道具效果
/// </summary>
public class ItemEffect : ScriptableObject
{

    public bool effectUsed { get; set; }
    public float effectLastUseTime { get; set; }  // 最后使用时间
    public float effectCooldown;  // 效果冷却时间

    [TextArea]
    public string effectDescription;  // 效果描述

    [TextArea]
    public string effectDescription_Chinese; // 效果描述中文

    public virtual void ExecuteEffect(Transform _spawnTransform)
    {
        //Debug.Log("Effect Executed");
    }

    //public virtual void ExecuteEffect_NoHitNeeded()
    //{

    //}

    public virtual void ReleaseSwordArcane()
    {

    }

}
