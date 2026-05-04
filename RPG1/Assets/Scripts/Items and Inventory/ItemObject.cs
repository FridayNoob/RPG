using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    //管理野生的掉落物品
    [SerializeField] private ItemData itemData;

    [SerializeField] private Rigidbody2D rb;

    private SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = itemData.icon;
    }

    private void OnValidate()
    {
        if (itemData == null)
            return;

        gameObject.GetComponent<SpriteRenderer>().sprite = itemData.icon;
        gameObject.name = "Item Object -- " + itemData.name;
    }
    private void SetupVisuals()
    {
        if (itemData == null)
        {
            return;
        }
        GetComponent<SpriteRenderer>().sprite = itemData.icon;
        gameObject.name = "Item Object - " + itemData.itemName;
    }

    public void SetupItem(ItemData _itemdata, Vector2 _velocity)
    {
        itemData = _itemdata;
        rb.velocity = _velocity;
        SetupVisuals();
    }


    public void PickUpItem()
    {
        if(!Inventory.instance.CanAddItem(itemData) && itemData.itemType == ItemType.Equipment)
        {
            rb.velocity = new Vector2(0, 7);
            //PlayerManager.instance.player.fx.CreatePopUpText("仓库已满！");
            return;
        }
        AudioManager.instance.PlaySFX(9, transform);
        Inventory.instance.AddItem(itemData);
        Destroy(gameObject);
    }
}
