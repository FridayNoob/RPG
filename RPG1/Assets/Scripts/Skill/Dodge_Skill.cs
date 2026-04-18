using UnityEngine;
using UnityEngine.UI;

public class Dodge_Skill : Skill
{

    [Header("Dodge")]
    [SerializeField] private UI_SkillTreeSlot dodgeUnlockButton;
    [SerializeField] private int evasionAmount;
    public bool dodgeUnlocked { get; private set; }

    [Header("Dodge mirage")]
    [SerializeField] private UI_SkillTreeSlot dodgeMirageUnlockButton;
    public bool dodgeMirageUnlocked { get; private set; }


    protected override void Start()
    {
        base.Start();

        dodgeUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockDodge);
        dodgeMirageUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockDodgeMirage);
    }

    protected override void CheckUnlock()
    {
        UnlockDodge();
        UnlockDodgeMirage();
    }
    private void UnlockDodge()
    {
        if (dodgeUnlockButton.unlocked && !dodgeUnlocked)
        {
            dodgeUnlocked = true;
            player.stats.evasion.AddModifier(evasionAmount);
            Inventory.instance.UpdateStatsUI();
        }
    }

    private void UnlockDodgeMirage()
    {
        if (dodgeMirageUnlockButton.unlocked)
            dodgeMirageUnlocked = true;
    }

    public void CreateMirageOnDodge()
    {
        if (dodgeMirageUnlocked)
        {
            SkillManager.instance.clone.CreateClone(player.transform, new Vector3(2 * player.facingDir, 0));
        }
    }
}
