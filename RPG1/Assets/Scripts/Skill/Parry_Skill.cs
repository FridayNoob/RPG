using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Parry_Skill : Skill
{
    [Header("Parry")]
    [SerializeField] private UI_SkillTreeSlot parryUnlockButton;
    public bool parryUnlocked { get; private set; }

    [Header("Parry restore")]
    [SerializeField] private UI_SkillTreeSlot parryRestoreUnlockedButton;
    public bool parryRestoreUnlocked { get; private set; }
    [Range(0f, 1f)]
    [SerializeField] private float restoreHealthPercentage;

    [Header("Parry mirage")]
    [SerializeField] private UI_SkillTreeSlot parryMirageUnlockButton;
    public bool parryMirageUnlocked { get; private set; }


    public override void UseSkill()
    {
        base.UseSkill();

        if (parryRestoreUnlocked)
        {
            int restoreAmount = Mathf.RoundToInt(player.stats.GetMaxHealthValue() * restoreHealthPercentage);
            player.stats.IncreaseHealthBy(restoreAmount);
        }
    }
    protected override void Start()
    {
        base.Start();

        parryUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockParry);
        parryRestoreUnlockedButton.GetComponent<Button>().onClick.AddListener(UnlockParryRestore);
        parryMirageUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockParryMirage);
    }

    #region Unlock

    protected override void CheckUnlock()
    {
        UnlockParry();
        UnlockParryMirage();
        UnlockParryRestore();
    }
    private void UnlockParry()
    {
        if(parryUnlockButton.unlocked)
        {
            parryUnlocked = true;
        }
    }

    private void UnlockParryRestore()
    {
        if(parryRestoreUnlockedButton.unlocked)
        {
            parryRestoreUnlocked = true;
        }
    }

    private void UnlockParryMirage()
    {
        if(parryMirageUnlockButton.unlocked)
        {
            parryMirageUnlocked = true;
        }
    }
    #endregion
    public void MakeMirageOnParry(Transform _respawnTransform)
    {
        if(parryMirageUnlocked)
        {
            SkillManager.instance.clone.CreateCloneWithDelay(_respawnTransform);
        }
    }
}
