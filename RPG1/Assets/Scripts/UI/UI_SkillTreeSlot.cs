using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SkillTreeSlot : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,ISaveManager
{
    private UI ui;
    private Image skiilImage;

    [SerializeField] private int skillCost;
    [SerializeField] private string skillName;
    [TextArea]
    [SerializeField] private string skillDescreption;
    [SerializeField] private Color lockedSkillColor;

    public bool unlocked;

    public TextMeshProUGUI slotText;

    [SerializeField] private UI_SkillTreeSlot[] shouldBeUnlocked;
    [SerializeField] private UI_SkillTreeSlot[] shouldBeLocked;


    private void OnValidate()
    {
        gameObject.name = "SkillTreeSlot_UI - " + skillName;
        slotText.text = skillName;
    }

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => { UnlockSkillSlot(); });
        
    }
    private void Start()
    {
        ui = GetComponentInParent<UI>();

        skiilImage = GetComponent<Image>();
        skiilImage.color = lockedSkillColor;
        if (unlocked)
            skiilImage.color = Color.white;

    }

    public void UnlockSkillSlot()
    {
        for(int i = 0; i < shouldBeUnlocked.Length; i++)
        {
            if (!shouldBeUnlocked[i].unlocked)
            {
                Debug.Log("Can not unlock skill!--1");
                return;
            }
        }

        for(int i = 0; i < shouldBeLocked.Length; i++)
        {
            if (shouldBeLocked[i].unlocked)
            {
                Debug.Log("Can not unlock skill!--2");
                return;
            }
        }

        if(PlayerManager.instance.HaveEnoughMoney(skillCost) == false)
        {
            return;
        }

        unlocked = true;
        skiilImage.color = Color.white;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.skillTooltip.ShowTooltip(skillName,skillDescreption, skillCost);

    }
    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillTooltip.HideTooltip();
    }

    public void LoadData(GameData _gameData)
    {
        if(_gameData.skillTree.TryGetValue(skillName, out bool value))
        {
            unlocked = value;
        }


    }

    public void SaveData(ref GameData _gameData)
    {
        if(_gameData.skillTree.TryGetValue(skillName, out bool value))
        {
            _gameData.skillTree.Remove(skillName);
            _gameData.skillTree.Add(skillName, unlocked);
        }
        else
        {
            _gameData.skillTree.Add(skillName, unlocked);
        }
    }
}
