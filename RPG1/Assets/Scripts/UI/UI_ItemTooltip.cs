using TMPro;
using UnityEngine;

public class UI_ItemTooltip : UI_Tooltip
{
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemTypeText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;

    public void ShowTooltip(ItemData_Equipment item)
    {
        if (item == null)
            return;

        itemNameText.text = item.itemName;
        itemTypeText.text = item.itemType.ToString();
        itemDescriptionText.text = item.GetDescription();

        AdjustPosition();
        

        gameObject.SetActive(true);
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }
}
