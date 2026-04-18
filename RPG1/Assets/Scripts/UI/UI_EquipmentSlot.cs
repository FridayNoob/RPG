using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EquipmentSlot : UI_ItemSlot
{
    //装备槽
    public EquipmentType slotType;

    private void OnValidate()
    {
        //只要生效就会显示装备的物品
        gameObject.name = "Equipment slot - " + slotType.ToString();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (item == null || item.data == null)
        {
            Debug.Log("装备栏为空！");
            return;
        }
        //装备栏脱下装备，并且仓库添加装备，然后清空装备栏UI
        Inventory.instance.UnequipItem(item.data as ItemData_Equipment);
        Inventory.instance.AddItem(item.data as ItemData_Equipment);

        ui.itemTooltip.HideTooltip();
        CleanUpSlot();

    }
}
