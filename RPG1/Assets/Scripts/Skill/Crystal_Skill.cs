using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crystal_Skill : Skill
{
    [Header("Crystal simple")]
    [SerializeField] private UI_SkillTreeSlot crystalUnlockButton;
    public bool crystalUnlocked { get; private set; }

    [Header("Cyrstal mirrage")]
    [SerializeField] private UI_SkillTreeSlot crystalMirageUnlockButton;
    [SerializeField] private bool crystalMirageUnlocked;

    [SerializeField] float crystalDuration;
    [SerializeField] private GameObject crystalPrefab;
    private GameObject currentCrystal;

    [Header("Explosive crystak")]
    [SerializeField] private UI_SkillTreeSlot crystalExplosiveUnlockButton;
    [SerializeField] private bool canExplode;

    [Header("Moving crystal")]
    [SerializeField] private UI_SkillTreeSlot crystalMovingUnlockButton;
    [SerializeField] private bool canMoveToEnemy;
    [SerializeField] private float moveSpeed;

    [Header("Mutiple crystal")]
    [SerializeField] private UI_SkillTreeSlot crystalMultipleUnlockButton;
    [SerializeField] private bool canUseMultiStacks;
    [SerializeField] private int stackAmount;
    [SerializeField] private float multiStackCooldown;
    [SerializeField] private List<GameObject> crystalLeft = new List<GameObject>();
    [SerializeField] private float useTimeWindow;

    protected override void Start()
    {
        base.Start();

        crystalUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCrystal);
        crystalMirageUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCrystalMirage);
        crystalExplosiveUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockExplosiveCrystal);
        crystalMovingUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockMovingCrystal);
        crystalMultipleUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockMultipleCrystal);
    }



    //解锁技能的函数
    #region Unlock skill region
    private void UnlockCrystal()
    {
        if (crystalUnlockButton.unlocked)
            crystalUnlocked = true;
    }

    private void UnlockCrystalMirage()
    {
        if (crystalMirageUnlockButton.unlocked)
            crystalMirageUnlocked = true;
    }

    private void UnlockExplosiveCrystal()
    {
        if (crystalExplosiveUnlockButton.unlocked)
            canExplode = true;
    }

    private void UnlockMovingCrystal()
    {
        if (crystalMovingUnlockButton.unlocked)
            canMoveToEnemy = true;
    }

    private void UnlockMultipleCrystal()
    {
        if (crystalMultipleUnlockButton.unlocked)
            canUseMultiStacks = true;
    }
    #endregion

    protected override void CheckUnlock()
    {
        UnlockCrystal();
        UnlockCrystalMirage();
        UnlockExplosiveCrystal();
        UnlockMovingCrystal();
        UnlockMultipleCrystal();
    }
    private bool CanUseMultiCrystal()
    {
        if (canUseMultiStacks)
        {
            if (crystalLeft.Count > 0)
            {
                if (crystalLeft.Count == stackAmount)
                    Invoke("ResetAbility", useTimeWindow);

                cooldown = 0;
                int crystalCount = crystalLeft.Count;
                GameObject crystalToSpawn = crystalLeft[crystalCount - 1];
                crystalLeft.Remove(crystalToSpawn);
                
                GameObject newCrystal = Instantiate(crystalToSpawn, player.transform.position, Quaternion.identity);


                newCrystal.GetComponent<Crystal_Skill_Controller>().
                    SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindClosestEnemy(newCrystal.transform), player);


                return true;
            }
            if (crystalLeft.Count <= 0)
            {
                cooldown = multiStackCooldown;
                //更新技能冷却图标
                RefilCrystal();
            }
        }
        return false;
    }

    public override void UseSkill()
    {
        base.UseSkill();

        if (CanUseMultiCrystal() || canUseMultiStacks)
            return;



        if (currentCrystal != null)
        {
            if (canMoveToEnemy)
                return;

            Vector2 playerPos = player.transform.position;
            player.transform.position = currentCrystal.transform.position;
            currentCrystal.transform.position = playerPos;



            if (crystalMirageUnlocked)
            {
                SkillManager.instance.clone.CreateClone(currentCrystal.transform, Vector3.zero);
                Destroy(currentCrystal);
            }
            else
            {
                currentCrystal.GetComponent<Crystal_Skill_Controller>().FinishCrystal();
            }
        }
        else
        {
            CreateCrystal();
        }

    }

    public void CreateCrystal()
    {
        currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity);
        Crystal_Skill_Controller currentCrystalScript = currentCrystal.GetComponent<Crystal_Skill_Controller>();
        currentCrystalScript.SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindClosestEnemy(currentCrystal.transform), player);
    }

    private void RefilCrystal()
    {
        int amountToAdd = stackAmount - crystalLeft.Count;

        for (int i = 0; i < amountToAdd; i++)
        {
            crystalLeft.Add(crystalPrefab);
        }
    }

    private void ResetAbility()
    {
        if (cooldownTimer > 0)
            return;

        cooldownTimer = multiStackCooldown;

        RefilCrystal();
    }

    public void CurrentCrystalChooseRandomEnemy() => currentCrystal.GetComponent<Crystal_Skill_Controller>().ChooseRandomEnemy();
}
