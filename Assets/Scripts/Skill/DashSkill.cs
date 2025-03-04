using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashSkill : Skill
{
    [Header("冲刺技能")]
    public bool dashUnlocked;
    [SerializeField] private SkillTreeSlot_UI dashUnlockButton;

    [Header("冲刺幻影 开始")]
    public bool cloneOnDashStartUnlocked;
    [SerializeField] private SkillTreeSlot_UI cloneOnDashStartUnlockButton;

    [Header("冲刺幻影 结束")]
    public bool cloneOnDashEndUnlocked;
    [SerializeField] private SkillTreeSlot_UI cloneOnDashEndUnlockButton;

    protected override void Start()
    {
        base.Start();

        dashUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockDash);
        cloneOnDashStartUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockCloneOnDashStart);
        cloneOnDashEndUnlockButton.GetComponent<Button>()?.onClick.AddListener(UnlockCloneOnDashEnd);
    }


    public override void UseSkill()
    {
        base.UseSkill();
    }

    public void CloneOnDashStart(Vector3 _position)
    {
        if (cloneOnDashStartUnlocked)
        {
            SkillManager.instance.clone.CreateClone(_position);
        }
    }

    public void CloneOnDashEnd(Vector3 _position)
    {
        if (cloneOnDashEndUnlocked)
        {
            SkillManager.instance.clone.CreateClone(_position);
        }
    }

    protected override void CheckUnlockFromSave()
    {
        UnlockDash();
        UnlockCloneOnDashStart();
        UnlockCloneOnDashEnd();
    }

    #region Unlock Skill
    private void UnlockDash()
    {
        if (dashUnlocked)
        {
            return;
        }

        if (dashUnlockButton.unlocked)
        {
            dashUnlocked = true;
        }
    }

    private void UnlockCloneOnDashStart()
    {
        if (cloneOnDashStartUnlocked)
        {
            return;
        }

        if (cloneOnDashStartUnlockButton.unlocked)
        {
            cloneOnDashStartUnlocked = true;
        }
    }
    private void UnlockCloneOnDashEnd()
    {
        if (cloneOnDashEndUnlocked)
        {
            return;
        }

        if (cloneOnDashEndUnlockButton.unlocked)
        {
            cloneOnDashEndUnlocked = true;
        }
    }
    #endregion
}
