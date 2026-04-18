using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blackhole_Skill : Skill
{
    [Header("Black hole")]
    [SerializeField] private UI_SkillTreeSlot blackholeUnlockButton;
    public bool blackholeUnlocked;

    [SerializeField] private int cloneAttackAmount;
    [SerializeField] private float cloneAttackCooldown;
    [Space]
    [SerializeField] private GameObject blackholePrefab;
    [SerializeField] private float maxSize;
    [SerializeField] private float growSpeed;
    [SerializeField] private float shrinkSpeed;
    [SerializeField] private float blackholeDuration;

    public Blackhole_Skill_Controller currentBlackhole;


    private void UnlockBlackhole()
    {
        if (blackholeUnlockButton.unlocked)
            blackholeUnlocked = true;
    }

    public override bool CanUseSkill()
    {
        return base.CanUseSkill();
    }

    public override void UseSkill()
    {
        base.UseSkill();
        GameObject newBlackhole = Instantiate(blackholePrefab, player.transform.position,Quaternion.identity);
        currentBlackhole = newBlackhole.GetComponent<Blackhole_Skill_Controller>();

        currentBlackhole.SetupBlackhole(maxSize, growSpeed, shrinkSpeed, cloneAttackAmount, cloneAttackCooldown, blackholeDuration);
        AudioManager.instance.PlaySFX(10, player.transform);
    }

    protected override void Start()
    {
        base.Start();

        blackholeUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockBlackhole);
    }

    protected override void Update()
    {
        base.Update();
    }

    public bool SkillCompleted()
    {
        if (!currentBlackhole)
            return false;
        if (currentBlackhole.playerCanExitState)
        {
            currentBlackhole = null;
            return true;
        }
        return false;
    }
    public float GetBlackholeRadius()
    {
        return maxSize / 2;
    }

    protected override void CheckUnlock()
    {
        UnlockBlackhole();
    }
}
