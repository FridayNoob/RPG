using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class Inventory : MonoBehaviour, ISaveManager
{
    //管理仓库
    //仓库实例
    public static Inventory instance;
    //初始持有物品
    public List<ItemData> startingItems;
    //仓库容器
    public List<InventoryItem> inventory;
    public Dictionary<ItemData, InventoryItem> inventoryDictionary;
    //仓库-材料容器
    public List<InventoryItem> stash;
    public Dictionary<ItemData, InventoryItem> stashDictionary;
    //仓库-装备容器
    public List<InventoryItem> equipment;
    public Dictionary<ItemData_Equipment, InventoryItem> equipmentDictionnary;

    [Header("Inventory UI")]
    //仓库、材料库、装备栏UI插槽、玩家统计数据插槽集合的父物体位置
    //方便获取该物体下所有插槽物体
    [SerializeField] private Transform inventorySlotParent;
    [SerializeField] private Transform stashSlotParent;
    [SerializeField] private Transform equipmentParent;
    [SerializeField] private Transform statSlotParent;
    //保存所有插槽
    private UI_ItemSlot[] inventoryItemSlot;
    private UI_ItemSlot[] stashItemSlot;
    private UI_EquipmentSlot[] equipmentSlot;
    private UI_StatSlot[] statSlot;


    [Header("Item cooldown")]
    //记录上一次使用Flask的时刻
    private float lastTimeUsedFlask;
    //记录上一次铠甲被动生效的时刻
    private float lastTimeUsedArmor;
    //Flask使用的冷却时间
    public float flaskCooldown { get; private set; }
    //铠甲被动生效的冷却时间
    private float armorCooldown;

    [Header("Data base")]
    //从GameData中加载的所有Inventory对象，包括ItemData和StackSize
    public List<ItemData> itemDataBase;
    public List<InventoryItem> loadedItems;
    public List<ItemData_Equipment> loadedEquipments;
    private void Awake()
    {
        //保证只有一个Inventory物体，对应一个Inventory脚本
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        //初始化仓库物品：物品列表、物品字典、库存插槽UI列表
        inventory = new List<InventoryItem>();
        inventoryDictionary = new Dictionary<ItemData, InventoryItem>();
        inventoryItemSlot = inventorySlotParent.GetComponentsInChildren<UI_ItemSlot>();
        //初始化材料物品：物品列表、物品字典、材料库存插槽UI列表
        stash = new List<InventoryItem>();
        stashDictionary = new Dictionary<ItemData, InventoryItem>();
        stashItemSlot = stashSlotParent.GetComponentsInChildren<UI_ItemSlot>();
        //初始化装备栏物品：物品列表、物品字典、装备栏插槽UI列表
        equipment = new List<InventoryItem>();
        equipmentDictionnary = new Dictionary<ItemData_Equipment, InventoryItem>();
        equipmentSlot = equipmentParent.GetComponentsInChildren<UI_EquipmentSlot>();
        //初始化玩家统计数据容器：UI列表
        statSlot = statSlotParent.GetComponentsInChildren<UI_StatSlot>();



    }

    public bool CanUseArmor()
    {
        //判断铠甲的被动效果能否生效
        //获取当前已装备的铠甲
        ItemData_Equipment currentArmor = GetEquipment(EquipmentType.Armor);

        if (currentArmor == null)
            return false;
        //判断铠甲被动是否处于冷却阶段
        if (Time.time > lastTimeUsedArmor + armorCooldown)
        {
            armorCooldown = currentArmor.itemCooldown;
            lastTimeUsedArmor = Time.time;
            return true;
        }

        Debug.Log("Armor on cooldown!");
        return false;

    }
    public void UseFlask()
    {
        //使用Flask效果
        //获取当前已经装备的Flask
        ItemData_Equipment currentFlask = GetEquipment(EquipmentType.Flask);

        if (currentFlask == null)
            return;
        //flaskCooldown初始值为0，避免游戏一开始Flask就进入冷却
        bool canUseFlask = Time.time > lastTimeUsedFlask + flaskCooldown;
        //Flask使用未处于冷却阶段，可以使用
        if (canUseFlask)
        {
            flaskCooldown = currentFlask.itemCooldown;
            //Flask效果调用
            currentFlask.Effect(null);
            lastTimeUsedFlask = Time.time;
        }
        else
            Debug.Log("Flask on cooldown!");

    }
    public ItemData_Equipment GetEquipment(EquipmentType _type)
    {
        //获取玩家当前穿戴的指定类型装备
        ItemData_Equipment equipmentItem = null;

        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionnary)
        {
            if (item.Key.equipmentType == _type)
                equipmentItem = item.Key;
        }

        return equipmentItem;
    }
    //获取材料仓库物品列表
    public List<InventoryItem> GetStashList() => stash;
    //获取装备栏物品列表
    public List<InventoryItem> GetEquimentList() => equipment;
    private void AddStartingItems()
    {
        //将初始物品添加到装备仓库
        //加载装备栏
        foreach (ItemData_Equipment item in loadedEquipments)
        {
            EquipItem(item);
        }
        //加载游戏时从GameData对象中加载物品到装备仓库
        if (loadedItems.Count > 0)
        {
            foreach (InventoryItem item in loadedItems)
            {
                for (int i = 0; i < item.stackSize; i++)
                {
                    AddItem(item.data);

                }
            }
            return;
        }
        //从初始背包中加载物品到对应材料库和装备仓库
        for (int i = 0; i < startingItems.Count; i++)
        {
            if (startingItems[i] != null)
                AddItem(startingItems[i]);
        }
    }

    public bool CanCraft(ItemData_Equipment _itemCraft, List<InventoryItem> _requringMaterials)
    {
        //判断并实现装备合成
        //存放合成耗费的所有材料
        List<InventoryItem> materialsToRemove = new List<InventoryItem>();
        //如果合成需要的材料数量不够或者缺乏，就不能合成
        for (int i = 0; i < _requringMaterials.Count; i++)
        {
            if (stashDictionary.TryGetValue(_requringMaterials[i].data, out InventoryItem stashValue))
            {
                if (stashValue.stackSize < _requringMaterials[i].stackSize)
                {
                    Debug.Log("not enough materials!");
                    return false;
                }
                else
                {
                    materialsToRemove.Add(stashValue);
                }
            }
            else
            {
                Debug.Log("not enough materials!");
                return false;
            }

        }
        //将合成耗费的材料从材料仓库中移除
        for (int i = 0; i < materialsToRemove.Count; i++)
        {
            RemoveItem(materialsToRemove[i].data);
        }
        //将合成的装备添加到装备仓库中，这里有个优化点：装备仓库爆仓会导致合成的装备无法入库，但是材料却消耗了
        AddItem(_itemCraft);
        Debug.Log("Here is your item " + _itemCraft.name);

        return true;
    }
    public void EquipItem(ItemData _item)
    {
        //将物品从装备仓库转移到装备栏的具体过程
        //将装备数据转化为库存物品
        ItemData_Equipment newEquipment = _item as ItemData_Equipment;
        InventoryItem newItem = new InventoryItem(_item);
        //存在装备替换情况，用此变量保存旧装备数据
        ItemData_Equipment oldEquipment = null;
        //遍历所有已穿上的装备，看看是否穿戴同种装备
        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionnary)
        {
            if (item.Key.equipmentType == newEquipment.equipmentType)
                oldEquipment = item.Key;
        }
        //如果有同种装备，装备栏卸掉旧，把旧装备重新放回仓库
        if (oldEquipment != null)
        {
            Debug.Log(oldEquipment.name + "被移除到仓库");
            UnequipItem(oldEquipment);
            AddItem(oldEquipment);
        }
        //实现添加新装备
        equipment.Add(newItem);
        equipmentDictionnary.Add(newEquipment, newItem);
        //添加装备效果
        newEquipment.AddModifiers();
        //从仓库中移除该装备
        RemoveItem(_item);
        //更新库存插槽UI
        UpdateSlotUI();
    }
    public void AddItem(ItemData _item)
    {
        //根据物品类型决定将装备放到装备仓库或者材料库
        if (_item.itemType == ItemType.Equipment && CanAddItem(_item))
        {
            //Debug.Log("合成装备：" + _item.itemName);
            AddToInventory(_item);

        }
        else if (_item.itemType == ItemType.Material)
            AddToStash(_item);
        UpdateSlotUI();
    }

    private void AddToInventory(ItemData _item)
    {
        //将物品添加到装备仓库
        if (inventoryDictionary.TryGetValue(_item, out InventoryItem value))
        {
            value.AddStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            inventory.Add(newItem);
            inventoryDictionary.Add(_item, newItem);
        }
    }

    private void AddToStash(ItemData _item)
    {
        //将物品添加到材料库
        if (stashDictionary.TryGetValue(_item, out InventoryItem value))
        {
            value.AddStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            stash.Add(newItem);
            stashDictionary.Add(_item, newItem);
        }
    }

    public void UnequipItem(ItemData_Equipment oldEquipment)
    {
        //卸掉装备，从列表、字典、装备效果三方面来实现
        if (equipmentDictionnary.TryGetValue(oldEquipment, out InventoryItem value))
        {
            equipment.Remove(value);
            equipmentDictionnary.Remove(oldEquipment);
            oldEquipment.RemoveModifiers();
        }
    }
    public void RemoveItem(ItemData _item)
    {
        //移除仓库物品或材料库物品，要分为是否全部移除和移除一部分两种情况
        //全部移除，需要修改字典、列表，部分移除只要修改数量
        //最后还要更新UI显示
        if (inventoryDictionary.TryGetValue(_item, out InventoryItem value))
        {
            if (value.stackSize <= 1)
            {
                inventory.Remove(value);
                inventoryDictionary.Remove(_item);
            }
            else
            {
                value.RemoveStack();
            }
        }
        if (stashDictionary.TryGetValue(_item, out InventoryItem stashValue))
        {
            if (stashValue.stackSize <= 1)
            {
                stash.Remove(stashValue);
                stashDictionary.Remove(_item);
            }
            else
            {
                stashValue.RemoveStack();
            }
        }
        UpdateSlotUI();
    }

    private void Update()
    {

    }

    private void UpdateSlotUI()
    {
        //更新所有仓库的UI显示
        //先清理仓库和材料库所有旧UI，然后用最新的数据更新仓库、材料库和装备栏UI
        for (int i = 0; i < inventoryItemSlot.Length; i++)
        {
            inventoryItemSlot[i].CleanUpSlot();
        }

        for (int i = 0; i < stashItemSlot.Length; i++)
        {
            stashItemSlot[i].CleanUpSlot();
        }

        for (int i = 0; i < inventory.Count; i++)
        {
            inventoryItemSlot[i].UpdateSlot(inventory[i]);
        }
        for (int i = 0; i < stash.Count; i++)
        {
            stashItemSlot[i].UpdateSlot(stash[i]);
        }
        for (int i = 0; i < equipment.Count; i++)
        {
            equipmentSlot[i].UpdateSlot(equipment[i]);
        }

        UpdateStatsUI();
    }

    public void UpdateStatsUI()
    {
        for (int i = 0; i < statSlot.Length; i++)
        {
            statSlot[i].UpdateStatValueUI();
        }
    }

    public bool CanAddItem(ItemData _item)
    {
        if (_item.itemType != ItemType.Equipment)
            return true;
        //判断装备仓库是否已满，满了的话就无法入库
        if (inventory.Count >= inventoryItemSlot.Length && !inventoryDictionary.Keys.Contains(_item))
        {
            Debug.Log("No more space!");
            return false;
        }
        return true;
    }

    public void LoadData(GameData _gameData)
    {
        //根据存档数据的字典中的UID与Assets中的UID进行比较进行ItemData的获取，再绑定存档数据字典中的int数值作为库存数，加入到loadedItems列表中
        foreach (KeyValuePair<string, int> pair in _gameData.inventory)
        {
            foreach (var item in itemDataBase)
            {
                if (item != null && item.itemID == pair.Key)
                {
                    InventoryItem inventoryItem = new InventoryItem(item);
                    inventoryItem.stackSize = pair.Value;
                    loadedItems.Add(inventoryItem);
                }
            }
        }
        //根据UID加载装备栏数据
        foreach (string loadedItemID in _gameData.equipmentID)
        {
            foreach (var item in itemDataBase)
            {
                if (item != null && item.itemID == loadedItemID)
                {
                    loadedEquipments.Add(item as ItemData_Equipment);
                }
            }
        }

        //添加初始背包物品：包括材料和装备
        AddStartingItems();
    }

    public void SaveData(ref GameData _gameData)
    {
        //保存库存物品的UID和库存数
        _gameData.inventory.Clear();
        _gameData.equipmentID.Clear();
        //装备仓库
        foreach (KeyValuePair<ItemData, InventoryItem> pair in inventoryDictionary)
        {
            _gameData.inventory.Add(pair.Key.itemID, pair.Value.stackSize);
        }
        //材料仓库
        foreach (KeyValuePair<ItemData, InventoryItem> pair in stashDictionary)
        {
            _gameData.inventory.Add(pair.Key.itemID, pair.Value.stackSize);
        }
        //存储装备栏
        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> pair in equipmentDictionnary)
        {
            _gameData.equipmentID.Add(pair.Key.itemID);
        }
    }
    //当Build Game时，下面这段代码不会被考虑
    #if UNITY_EDITOR

    [ContextMenu("Fill up item data base")]
    private void FillUpItemDataBase() => itemDataBase = new List<ItemData>(GetItemDataBase());

    private List<ItemData> GetItemDataBase()
    {
        //把资产数据读到列表中并进行返回
        List<ItemData> tempItemDataBase = new List<ItemData>();
        string[] assetNames = AssetDatabase.FindAssets("", new[] { "Assets/Data/Items" });

        foreach (string SOName in assetNames)
        {
            var SOPath = AssetDatabase.GUIDToAssetPath(SOName);
            var itemData = AssetDatabase.LoadAssetAtPath<ItemData>(SOPath);
            tempItemDataBase.Add(itemData);
        }

        return tempItemDataBase;
    }

    #endif

}
