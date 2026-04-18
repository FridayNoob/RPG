using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clone_Skill : Skill
{
    [Header("Clone info")]
    [SerializeField] private float attackMultiplier;
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration;
    [Space]

    [Header("Clone attack")]
    [SerializeField] private UI_SkillTreeSlot cloneAttackUnlockButton;
    [SerializeField] private float cloneAttackMultiplier;
    [SerializeField] private bool canAttack;

    [Header("Clone Aggressive")]
    [SerializeField] private UI_SkillTreeSlot cloneAggressiveUnlockButton;
    [SerializeField] private float cloneAgressiveMultiplier;
    public bool canApplyHitEffect { get; private set; }

    [Header("Clone can duplicate")]
    [SerializeField] private UI_SkillTreeSlot cloneMultipleUnlockButton;
    [SerializeField] private float cloneMultipleMultiplier;
    [SerializeField] private bool canDuplicateClone;
    [SerializeField] private float chanceToDuplicate;

    [Header("Crystal instesd of Clone")]
    [SerializeField] private UI_SkillTreeSlot cloneCrystalInsteadUnlockButton;
    public bool crystalInsteadOfClone;

    protected override void Start()
    {
        base.Start();

        cloneAttackUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneAttack);
        cloneAggressiveUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneAggressive);
        cloneMultipleUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneMultiple);
        cloneCrystalInsteadUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneCrystalInstead);
    }

    #region Unlock

    protected override void CheckUnlock()
    {
        UnlockCloneAttack();
        UnlockCloneAggressive();
        UnlockCloneMultiple();
        UnlockCloneCrystalInstead();
    }
    private void UnlockCloneAttack()
    {
        if (cloneAttackUnlockButton.unlocked)
        {
            canAttack = true;
            attackMultiplier = cloneAttackMultiplier;
        }
    }

    private void UnlockCloneAggressive()
    {
        if (cloneAggressiveUnlockButton.unlocked)
        {
            canApplyHitEffect = true;
            attackMultiplier = cloneAgressiveMultiplier;
        }
    }

    private void UnlockCloneMultiple()
    {
        if(cloneMultipleUnlockButton.unlocked)
        {
            canDuplicateClone = true;
            attackMultiplier = cloneMultipleMultiplier;
        }
    }

    private void UnlockCloneCrystalInstead()
    {
        if (cloneCrystalInsteadUnlockButton.unlocked)
        {
            crystalInsteadOfClone = true;
        }
    }

    #endregion
    public void CreateClone(Transform _newTransform, Vector3 _offset)
    {
        if (crystalInsteadOfClone)
        {
            SkillManager.instance.crystal.CreateCrystal();
            return;
        }

        GameObject newClone = Instantiate(clonePrefab);
        newClone.GetComponent<Clone_Skill_Controller>().SetupClone(_newTransform, cloneDuration, canAttack, _offset,
            FindClosestEnemy(newClone.transform), canDuplicateClone, chanceToDuplicate, player, attackMultiplier);
    }



    public void CreateCloneWithDelay(Transform _enemyTransform)
    {
            StartCoroutine(CloneDelayCoroutine(_enemyTransform, new Vector3(1 * player.facingDir, 0)));
    }

    private IEnumerator CloneDelayCoroutine(Transform _transform, Vector3 _offset)
    {
        yield return new WaitForSeconds(.4f);
        CreateClone(_transform, _offset);
    }
}
