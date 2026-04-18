using System.Collections.Generic;
using UnityEngine;

public enum EquipmentType
{
    Weapon,
    Armor,
    Amulet,
    Flask
}

[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Equipment")]
public class ItemData_Equipment : ItemData
{
    private int descriptionLength;
    public EquipmentType equipmentType;

    [Header("Unique effect")]
    public float itemCooldown;
    public ItemEffect[] itemEffects;



    //装备物品数据，在仓库物品数据的基础上增加了装备属性数据和属性方法
    [Header("Major stats")]
    public int strength;
    public int agility;
    public int intelligence;
    public int vitality;

    [Header("Defensive stats")]
    public int maxHealth;
    public int armor;
    public int evasion;
    public int magicRsistance;

    [Header("Offensive stats")]
    public int damage;
    public int critChance;
    public int critPower;

    [Header("Magic Stats")]
    public int fireDamage;
    public int iceDamage;
    public int lightningDamage;
    //public int igniteDamage;

    [Header("Craft requirements")]
    public List<InventoryItem> craftingMaterials;

    public void Effect(Transform _enemyPosition)
    {
        //调用装备的所有效果
        foreach(var item in itemEffects)
        {
            item.ExecuteEffect(_enemyPosition);
        }
    }
    public void AddModifiers()
    {
        //将装备属性加成应用到玩家上
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        playerStats.strength.AddModifier(strength);
        playerStats.agility.AddModifier(agility);
        playerStats.intelligence.AddModifier(intelligence);
        playerStats.vitality.AddModifier(vitality);

        playerStats.maxHealth.AddModifier(maxHealth);
        playerStats.armor.AddModifier(armor);
        playerStats.evasion.AddModifier(evasion);
        playerStats.magicRsistance.AddModifier(magicRsistance);

        playerStats.damage.AddModifier(damage);
        playerStats.critChance.AddModifier(critChance);
        playerStats.critPower.AddModifier(critPower);

        playerStats.fireDamage.AddModifier(fireDamage);
        playerStats.iceDamage.AddModifier(iceDamage);
        playerStats.lightningDamage.AddModifier(lightningDamage);
    }

    public void RemoveModifiers()
    {
        //移除玩家的装备属性加成
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        playerStats.strength.RemoveModifier(strength);
        playerStats.agility.RemoveModifier(agility);
        playerStats.intelligence.RemoveModifier(intelligence);
        playerStats.vitality.RemoveModifier(vitality);

        playerStats.maxHealth.RemoveModifier(maxHealth);
        playerStats.armor.RemoveModifier(armor);
        playerStats.evasion.RemoveModifier(evasion);
        playerStats.magicRsistance.RemoveModifier(magicRsistance);

        playerStats.damage.RemoveModifier(damage);
        playerStats.critChance.RemoveModifier(critChance);
        playerStats.critPower.RemoveModifier(critPower);

        playerStats.fireDamage.RemoveModifier(fireDamage);
        playerStats.iceDamage.RemoveModifier(iceDamage);
        playerStats.lightningDamage.RemoveModifier(lightningDamage);
    }

    public override string GetDescription()
    {
        sb.Length = 0;
        descriptionLength = 0;

        AddItemDescription(strength, "strength");
        AddItemDescription(agility, "agility");
        AddItemDescription(intelligence, "intelligence");
        AddItemDescription(vitality, "vitality");

        AddItemDescription(damage, "damage");
        AddItemDescription(critChance, "critChance");
        AddItemDescription(critPower, "critPower");

        AddItemDescription(maxHealth, "health");
        AddItemDescription(armor, "armor");
        AddItemDescription(evasion, "evasion");
        AddItemDescription(magicRsistance, "magicRes");

        AddItemDescription(fireDamage, "fireDamage");
        AddItemDescription(iceDamage, "iceDamage");
        AddItemDescription(lightningDamage, "lightingDamage");

        for(int i = 0; i < itemEffects.Length; i++)
        {
            if (itemEffects[i].effectDescription.Length > 0)
            {
                sb.AppendLine();
                sb.AppendLine(itemEffects[i].effectDescription);
                descriptionLength++;
            }
        }

        if(descriptionLength < 5)
        {
            for(int i = 0; i < 5 - descriptionLength; i++)
            {
                sb.AppendLine();
                sb.Append("");
            }
        }


        

        return sb.ToString();
    }

    private void AddItemDescription(int _value, string _name)
    {
        if(_value != 0)
        {
            if (sb.Length > 0)
            {
                sb.AppendLine();
                descriptionLength++;
            }

            if (_value > 0)
                sb.Append("+ " + _value + "  " + _name);
        }
    }
}
