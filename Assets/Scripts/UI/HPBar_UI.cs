using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HPBar_UI : MonoBehaviour
{
    private Entity entity;
    private RectTransform barTransform;
    private Slider slider;

    private CharacterStats myStats;

    private void Awake()
    {
        entity = GetComponentInParent<Entity>();
        barTransform = GetComponent<RectTransform>();
        slider = GetComponentInChildren<Slider>();
        myStats = GetComponentInParent<CharacterStats>();
    }

    private void OnEnable()
    {
        entity.onFlipped += FlipUI;
        myStats.onHealthChanged += UpdateHPUI;
    }

    private void OnDisable()
    {
        if (entity != null)
        {
            entity.onFlipped -= FlipUI;
        }

        if (myStats != null)
        {
            myStats.onHealthChanged -= UpdateHPUI;
        }
    }

    private void Start()
    {
        

        UpdateHPUI();
    }

    private void UpdateHPUI()
    {
        //total_maxHP = maxHP + vitality * 5
        slider.maxValue = myStats.getMaxHP();
        slider.value = myStats.currentHP;
    }

    private void FlipUI()
    {
        barTransform.Rotate(0, 180, 0);
    }

   
}
