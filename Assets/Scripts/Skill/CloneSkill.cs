using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;


// 克隆攻击技能分支实现
public class CloneSkill : Skill
{
    private float currentCloneAttackDamageMultipler;

    [Header("克隆属性")]
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration;
    [SerializeField] private float colorLosingSpeed;


    [Header("幻影攻击属性")] 
    [SerializeField] private SkillTreeSlot_UI mirageAttackUnlockButton;
    [Range(0f, 1f)]
    [SerializeField] private float cloneAttackDamageMultiplier;  
    public bool mirageAttackUnlocked { get; private set; }


    [Header("斗争幻影解锁属性")] 
    [SerializeField] private SkillTreeSlot_UI aggressiveMirageUnlockButton;
    [Range(0f, 1f)]
    [SerializeField] private float aggressiveCloneAttackDamageMultiplier;
    public bool aggressiveMirageUnlocked { get; private set; }
    public bool aggressiveCloneCanApplyOnHitEffect { get; private set; }


    [Header("多重幻影解锁属性")] 
    [SerializeField] private SkillTreeSlot_UI multipleMirageUnlockButton;
    [Range(0f, 1f)]
    [SerializeField] private float duplicateCloneAttackDamageMultiplier;  
    public bool multipleMirageUnlocked { get; private set; }
    [SerializeField] private float duplicatePossibility;
    public int maxDuplicateCloneAmount; 
    [HideInInspector] public int currentDuplicateCloneAmount;


    [Header("水晶幻影解锁属性")]
    [SerializeField] private SkillTreeSlot_UI crystalMirageUnlockButton;
    public bool crystalMirageUnlocked { get; private set; }


    protected override void Start()
    {
        base.Start();

        mirageAttackUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockMirageAttack);
        aggressiveMirageUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockAggressiveMirage);
        multipleMirageUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockMultipleMirage);
        crystalMirageUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockCrystalMirage);
    }


    public void RefreshCurrentDuplicateCloneAmount()
    {
        currentDuplicateCloneAmount = 0;
    }

    public void CreateClone(Vector3 _position)
    {
        if (crystalMirageUnlocked)
        {
            //if (SkillManager.instance.crystal.crystalMirageUnlocked)
            //{
            //    SkillManager.instance.crystal.crystalMirageUnlocked = false;
            //    Debug.Log("Clone_Mirage in Crystal Skill is DISABLED" +
            //        "\nBecase Replace_Clone_By_Crystal in Clone Skill is ENABLED");
            //}

            //SkillManager.instance.crystal.DestroyCurrentCrystal_InCrystalMirageOnly();

            if (SkillManager.instance.crystal.SkillIsReadyToUse())
            {
                SkillManager.instance.crystal.UseSkillIfAvailable();
            }
            return;
        }

        RefreshCurrentDuplicateCloneAmount();

        GameObject newClone = Instantiate(clonePrefab, _position, Quaternion.identity);
        CloneSkillController newCloneScript = newClone.GetComponent<CloneSkillController>();

        newCloneScript.SetupClone(cloneDuration, colorLosingSpeed, mirageAttackUnlocked, FindClosestEnemy(newClone.transform), multipleMirageUnlocked, duplicatePossibility, currentCloneAttackDamageMultipler);
    }

    public void CreateDuplicateClone(Vector3 _position)
    {
        GameObject newClone = Instantiate(clonePrefab, _position, Quaternion.identity);
        CloneSkillController newCloneScript = newClone.GetComponent<CloneSkillController>();

        newCloneScript.SetupClone(cloneDuration, colorLosingSpeed, mirageAttackUnlocked, FindClosestEnemy(newClone.transform), multipleMirageUnlocked, duplicatePossibility, currentCloneAttackDamageMultipler);

        currentDuplicateCloneAmount++;
    }


    public void CreateCloneWithDelay(Vector3 _position, float _delay)
    {
        StartCoroutine(CreateCloneWithDelay_Coroutine(_position, _delay));
    }

    private IEnumerator CreateCloneWithDelay_Coroutine(Vector3 _position, float _seconds)
    {
        yield return new WaitForSeconds(_seconds);

        CreateClone(_position);
    }


    protected override void CheckUnlockFromSave()
    {
        UnlockMirageAttack();
        UnlockAggressiveMirage();
        UnlockCrystalMirage();
        UnlockMultipleMirage();
    }

    #region Unlock Skill
    private void UnlockMirageAttack()
    {
        if (mirageAttackUnlocked)
        {
            return;
        }

        if (mirageAttackUnlockButton.unlocked)
        {
            mirageAttackUnlocked = true;
            currentCloneAttackDamageMultipler = cloneAttackDamageMultiplier;
        }
    }

    private void UnlockAggressiveMirage()
    {
        if (aggressiveMirageUnlocked)
        {
            return;
        }

        if (aggressiveMirageUnlockButton.unlocked)
        {
            aggressiveMirageUnlocked = true;
            aggressiveCloneCanApplyOnHitEffect = true;
            currentCloneAttackDamageMultipler = aggressiveCloneAttackDamageMultiplier;
        }
    }

    private void UnlockMultipleMirage()
    {
        if (multipleMirageUnlocked)
        {
            return;
        }

        if (multipleMirageUnlockButton.unlocked)
        {
            multipleMirageUnlocked = true;
            currentCloneAttackDamageMultipler = duplicateCloneAttackDamageMultiplier;
        }
    }

    private void UnlockCrystalMirage()
    {
        if (crystalMirageUnlocked)
        {
            return;
        }

        if (crystalMirageUnlockButton.unlocked)
        {
            crystalMirageUnlocked = true;
        }
    }
    #endregion
}
