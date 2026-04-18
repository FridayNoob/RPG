using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_SkillTooltip : UI_Tooltip
{
    [SerializeField] private TextMeshProUGUI skillDescription;
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI skillCost;

    public void ShowTooltip(string _skillName, string _skillDescription, int _price)
    {
        skillName.text = _skillName;

        skillDescription.text = _skillDescription;
        skillCost.text = "Cost:" + _price.ToString();

        //让技能描述框跟随鼠标位置显示
        AdjustPosition();
        gameObject.SetActive(true);
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }
}
