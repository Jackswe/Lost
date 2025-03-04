using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// ��������Ч��
/// </summary>
public class ItemEffect : ScriptableObject
{

    public bool effectUsed { get; set; }
    public float effectLastUseTime { get; set; }  // ���ʹ��ʱ��
    public float effectCooldown;  // Ч����ȴʱ��

    [TextArea]
    public string effectDescription;  // Ч������

    [TextArea]
    public string effectDescription_Chinese; // Ч����������

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
