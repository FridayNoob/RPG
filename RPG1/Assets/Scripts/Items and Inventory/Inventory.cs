using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;


public class Inventory : MonoBehaviour, ISaveManager
{
    //用一个结构体来存储仓库的属性，方便管理和传递参数
    struct InventoryProp
    {
        public RectTransform itemSlotRect;
        public float itemSlotWidth;//库存插槽宽度
        public float itemSlotHeight;//库存插槽高度
        public int column;//单页显示列数
        public int row;//单页显示行数
        public int totalColumn;
        public int totalRow;//总列数
        public int maxShownItems;//单页显示库存数
        public int startShowItemIndex;//单页显示的第一个库存物品索引
    }
    private InventoryProp stashProp, inventoryProp;
    //管理仓库
    //仓库实例
    public static Inventory instance;
    //初始持有物品
    public List<ItemData> startingItems;
    //仓库容器
    public List<InventoryItem> inventory;
    public Dictionary<ItemData, InventoryItem> inventoryDictionary;
    //用于三个仓库库存计算的共享变量
    

    [Header("库存插槽偏移Padding")]
    public Vector2 offset = new Vector2(0, 0);
    //仓库-材料容器
    public List<InventoryItem> stash;
    public Dictionary<ItemData, InventoryItem> stashDictionary;
    public GameObject stashSlotPrefab;
    public GameObject stashScrollViewObj;
    
    
    //仓库-装备容器
    public List<InventoryItem> equipment;
    public Dictionary<ItemData_Equipment, InventoryItem> equipmentDictionnary;
    public GameObject inventorySlotPrefab;
    public GameObject inventoryScrollViewObj;
    

    [Header("Inventory UI")]
    //仓库、材料库、装备栏UI插槽、玩家统计数据插槽集合的父物体位置
    //方便获取该物体下所有插槽物体
    [SerializeField] private Transform inventorySlotParent;
    [SerializeField] private Transform stashSlotParent;
    [SerializeField] private Transform equipmentParent;
    [SerializeField] private Transform statSlotParent;
    //保存所有插槽
    private List<UI_ItemSlot> inventoryItemSlot;
    private List<UI_ItemSlot> stashItemSlot;
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
        inventoryItemSlot = new List<UI_ItemSlot>(inventorySlotParent.GetComponentsInChildren<UI_ItemSlot>());
        //初始化材料物品：物品列表、物品字典、材料库存插槽UI列表
        stash = new List<InventoryItem>();
        stashDictionary = new Dictionary<ItemData, InventoryItem>();
        stashItemSlot = new List<UI_ItemSlot>(stashSlotParent.GetComponentsInChildren<UI_ItemSlot>());
        //初始化装备栏物品：物品列表、物品字典、装备栏插槽UI列表
        equipment = new List<InventoryItem>();
        equipmentDictionnary = new Dictionary<ItemData_Equipment, InventoryItem>();
        equipmentSlot = equipmentParent.GetComponentsInChildren<UI_EquipmentSlot>();
        //初始化玩家统计数据容器：UI列表
        statSlot = statSlotParent.GetComponentsInChildren<UI_StatSlot>();

        //添加stash仓库滚动监听事件
        var scrollRect = stashScrollViewObj.GetComponent<ScrollRect>();
        scrollRect.onValueChanged.AddListener(delegate { StashScrollViewOnValueChanged(); });

        //添加inventory仓库滚动监听事件
        var inventoryScrollRect = inventoryScrollViewObj.GetComponent<ScrollRect>();
        inventoryScrollRect.onValueChanged.AddListener(delegate { InventoryScrollViewOnValueChanged(); });


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
        if (_item.itemType == ItemType.Equipment)
        {
            //Debug.Log("合成装备：" + _item.itemName);
            AddToInventory(_item);

        }
        else if (_item.itemType == ItemType.Material)
            AddToStash(_item);
        UpdateSlotUI();
    }

    public void AddItemsBatch(IEnumerable<ItemData> items)
    {
        //批量添加，避免频繁刷新UI
        foreach (var _item in items)
        {
            if (_item == null) continue;
            if (_item.itemType == ItemType.Equipment)
                AddToInventory(_item);
            else if (_item.itemType == ItemType.Material)
                AddToStash(_item);
        }
        // 批量添加后一次性刷新 UI
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
        //更新所有仓库库存UI
        //计算插槽位置参数
        //更新stash仓库
        UpdateStashSlotUI();

        //更新inventory仓库
        UpdateInventorySlotUI();

        //更新装备栏
        UpdateEquipmentSlotUI();

        //更新玩家统计数据UI
        UpdateStatsUI();
    }


    private void UpdateEquipmentSlotUI()
    {
        for(int i = 0; i < equipmentSlot.Length; i++)
        {
            if (i < equipment.Count)
            {
                equipmentSlot[i].UpdateSlot(equipment[i]);
            }
            else
            {
                equipmentSlot[i].CleanUpSlot();
            }
        }
    }
    private void UpdateInventorySlotUI()
    {
        //统计物品仓库物体总数
        int inventoryCount = inventory.Count;
        //Debug.Log("Total stash items: " + stashCount);
        //根据ScrollView窗口大小计算行列数
        //一页的宽高
        var inventoryScrollViewRect = inventoryScrollViewObj.GetComponent<RectTransform>();
        float scrollViewWidth = inventoryScrollViewRect.rect.width;
        float scrollViewHeight = inventoryScrollViewRect.rect.height;
        //单个库存插槽的宽高
        inventoryProp.itemSlotRect = inventorySlotPrefab.GetComponent<RectTransform>();
        inventoryProp.itemSlotWidth = inventoryProp.itemSlotRect.rect.width;
        inventoryProp.itemSlotHeight = inventoryProp.itemSlotRect.rect.height;
        //每页能够显示的行列数
        inventoryProp.column = Mathf.FloorToInt(scrollViewWidth / (inventoryProp.itemSlotWidth + offset.y));
        inventoryProp.row = Mathf.FloorToInt(scrollViewHeight / (inventoryProp.itemSlotHeight + offset.x)) + 1;//这里加1是为了衔接滚动的连续效果较好

        inventoryProp.totalRow = Mathf.CeilToInt((float)inventoryCount / inventoryProp.column) + 1;//这里加1是为了保证当物品数量正好是列数的整数倍时，最后一行能被完整显示出来，而不是只显示最后一行的一部分
        //计算当前ScrollView窗口能显示的最大物品数量
        inventoryProp.maxShownItems = inventoryProp.column * inventoryProp.row;
        //Debug.Log("Item slot width: " + itemSlotWidth + ", height: " + itemSlotHeight);
        //Debug.Log("ScrollView width: " + scrollViewWidth + ", height: " + scrollViewHeight);
        //Debug.Log("Columns: " + columns + ", Rows: " + rows + ", Max shown items: " + maxShownItems);
        //设置content大小
        var contentRect = inventorySlotParent.GetComponent<RectTransform>();
        float contentWidth = contentRect.sizeDelta.x; 
        float contentHeight = inventoryProp.totalRow * (inventoryProp.itemSlotHeight + offset.y);
        Vector2 contentSize = new Vector2(contentWidth, contentHeight);
        contentRect.sizeDelta = contentSize;

        // 创建前先清理掉之前的插槽UI
        if (inventorySlotParent != null)
        {
            // 先把所有子物体拷贝到临时列表，避免在遍历时修改 Transform 的子集合（Reparent 会改变集合）
            var children = new List<Transform>();
            foreach (Transform child in inventorySlotParent)
                children.Add(child);

            // 逐一处理
            foreach (var child in children)
            {
                if (child == null) continue;
                    var slot = child.GetComponent<UI_ItemSlot>();
                if (slot != null)
                {
                    // 正常回收到池里（会 SetActive(false) 并 SetParent(poolParent)）
                    ItemSlotPool.Instance.Release(slot);
                }
                else
                {
                    // 如果发现非 UI_ItemSlot 的子物体，记录日志并清理，防止意外累积
                    Debug.LogWarning($"Found non-UI_ItemSlot child under {stashSlotParent.name}: {child.name}. Destroying to avoid accumulation.");
                    Destroy(child.gameObject);
                }
            }
        }

        inventoryItemSlot.Clear();


        CreateInventoryItemSlotNow(inventoryProp.maxShownItems);
        //开启协程创建对应数量的材料仓库插槽UI

        //StartCoroutine(CreateStashItemSlot(maxShownItems));
    }

    private void UpdateStashSlotUI()
    {
        //统计材料仓库物体总数
        int stashCount = stash.Count;
        //Debug.Log("Total stash items: " + stashCount);
        //根据ScrollView窗口大小计算行列数
        var stashScrollViewRect = stashScrollViewObj.GetComponent<RectTransform>();
        float scrollViewWidth = stashScrollViewRect.rect.width;
        float scrollViewHeight = stashScrollViewRect.rect.height;

        stashProp.itemSlotRect = stashSlotPrefab.GetComponent<RectTransform>();
        stashProp.itemSlotWidth = stashProp.itemSlotRect.rect.width;
        stashProp.itemSlotHeight = stashProp.itemSlotRect.rect.height;

        stashProp.column = Mathf.FloorToInt(scrollViewWidth / (stashProp.itemSlotWidth + offset.y));
        stashProp.row = Mathf.FloorToInt(scrollViewHeight / (stashProp.itemSlotHeight + offset.x));

        stashProp.totalColumn = Mathf.CeilToInt((float)stashCount / stashProp.row);
        //计算当前ScrollView窗口能显示的最大物品数量
        stashProp.maxShownItems = stashProp.column * stashProp.row + 1;
        //Debug.Log("Item slot width: " + itemSlotWidth + ", height: " + itemSlotHeight);
        //Debug.Log("ScrollView width: " + scrollViewWidth + ", height: " + scrollViewHeight);
        //Debug.Log("Columns: " + columns + ", Rows: " + rows + ", Max shown items: " + maxShownItems);
        //设置content大小
        var contentRect = stashSlotParent.GetComponent<RectTransform>();
        float contentWidth = stashProp.totalColumn * (stashProp.itemSlotWidth + offset.x);
        float contentHeight = contentRect.sizeDelta.y;
        Vector2 contentSize = new Vector2(contentWidth, contentHeight);
        contentRect.sizeDelta = contentSize;

        // 创建前先清理掉之前的插槽UI
        if (stashSlotParent != null)
        {
            // 先把所有子物体拷贝到临时列表，避免在遍历时修改 Transform 的子集合（Reparent 会改变集合）
            var children = new List<Transform>();
            foreach (Transform child in stashSlotParent)
                children.Add(child);

            // 逐一处理
            foreach (var child in children)
            {
                if (child == null) continue;
                var slot = child.GetComponent<UI_ItemSlot>();
                if (slot != null)
                {
                    // 正常回收到池里（会 SetActive(false) 并 SetParent(poolParent)）
                    ItemSlotPool.Instance.Release(slot);
                }
                else
                {
                    // 如果发现非 UI_ItemSlot 的子物体，记录日志并清理，防止意外累积
                    Debug.LogWarning($"Found non-UI_ItemSlot child under {stashSlotParent.name}: {child.name}. Destroying to avoid accumulation.");
                    Destroy(child.gameObject);
                }
            }
        }

        stashItemSlot.Clear();


        CreateStashItemSlotNow(stashProp.maxShownItems);
        //开启协程创建对应数量的材料仓库插槽UI

        //StartCoroutine(CreateStashItemSlot(maxShownItems));

    }

    private  void UpdatePositionHorizontal(float offsetX, float offsetY, float itemSlotWidth, float itemSlotHeight, int index, UI_ItemSlot stashSlot)
    {
        //根据索引计算物品UI在content的实际位置(横向滚动列表)
        int currentColumn = index;
        float posX = currentColumn * (itemSlotWidth + offsetX);
        stashSlot.GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, 0);
    }
    private  void UpdatePositionVertical(float offsetX, float offsetY, float itemSlotWidth, float itemSlotHeight, int index, UI_ItemSlot stashSlot)
    {
        //根据索引计算物品UI在content中的实际位置（纵向滚动列表）
        int currentColumn = index % inventoryProp.column;
        int currentRow = index / inventoryProp.column;
        float posX = currentColumn * (itemSlotWidth + offsetX);
        float posY = -currentRow * (itemSlotHeight + offsetY);
        stashSlot.GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, posY);
    }



    void StashScrollViewOnValueChanged()
    {
        //当ScrollView滚动时，更新材料仓库UI显示
        var contentRect = stashSlotParent.GetComponent<RectTransform>();
        UpdateStashScroviewShowInfo(contentRect);
    }


    void InventoryScrollViewOnValueChanged()
    {
        //当ScrollView滚动时，更新inventory仓库UI显示
        var contentRect = inventorySlotParent.GetComponent<RectTransform>();
        UpdateInventoryScroviewShowInfo(contentRect);
    }

    //当Stash仓库的ScrollView滚动时，更新材料仓库UI显示
    public void UpdateStashScroviewShowInfo(RectTransform contentRect)
    {
        float x = contentRect.anchoredPosition.x;
        x = Mathf.Min(x, 0);
        int newStartShowItemIndex = Mathf.FloorToInt(-x / (stashProp.itemSlotWidth + offset.x));
        if (newStartShowItemIndex != stashProp.startShowItemIndex)
        {
            Debug.Log("ScrollView position changed. Updating shown items. New start index: " + newStartShowItemIndex);
            //新索引满足条件：不能小于0，不能大于等于stash.Count - maxShownItems
            stashProp.startShowItemIndex = Mathf.Min(newStartShowItemIndex, stash.Count- stashProp.maxShownItems);
            //补充条件 新索引不能大于仓库数量
            stashProp.startShowItemIndex = Mathf.Min(stashProp.startShowItemIndex, stash.Count - 1);
            stashProp.startShowItemIndex = Mathf.Max(stashProp.startShowItemIndex, 0);
            //只更新UI显示，不会增删插槽
            //UpdateSlotUI();
            UpdateStashSlotShow();
        }
    }

    //当Inventory仓库的ScrollView滚动时，更新仓库UI显示
    public void UpdateInventoryScroviewShowInfo(RectTransform contentRect)
    {
        float y = contentRect.anchoredPosition.y;
        y = Mathf.Max(y, 0);
        int newStartShowRowIndex = Mathf.FloorToInt(y / (inventoryProp.itemSlotHeight + offset.y)); //  从0开始的行索引
        int newStartShowItemIndex = newStartShowRowIndex * inventoryProp.column; // 计算对应的物品索引
        if (newStartShowItemIndex != inventoryProp.startShowItemIndex)
        {
            Debug.Log("ScrollView position changed. Updating shown items. New start index: " + newStartShowItemIndex);
            //新索引满足条件：不能小于0，一页显示的最后一个物品索引不能大于等于库存物品总数
            inventoryProp.startShowItemIndex = Mathf.Min(newStartShowItemIndex, inventory.Count - inventoryProp.maxShownItems + inventoryProp.column);//这里加上column是为了保证当滚动到最后一行时，最后一行能被完整显示出来，而不是只显示最后一行的一部分
            inventoryProp.startShowItemIndex = Mathf.Max(inventoryProp.startShowItemIndex, 0);
            //只更新UI显示，不会增删插槽
            //UpdateSlotUI();
            UpdateInventorySlotShow();
        }
    }

    //IEnumerator CreateStashItemSlot(int maxShownItems)
    //{
    //    //Debug.Log("Creating stash item slots...");
    //    //Debug.Log("stash items count : " + stash.Count);
    //    //根据当前ScrollView窗口能显示的最大物品数量来创建对应数量的材料仓库插槽UI
    //    int showItems = Mathf.Min(stash.Count, maxShownItems);



    //    for (int i = 0; i < showItems; i++)
    //    {
    //        UI_ItemSlot slotObject = ItemSlotPool.Instance.Get(stashSlotParent,false);
            
    //        stashItemSlot.Add(slotObject);

    //        if (stashProp.startShowItemIndex + i >= stash.Count)
    //        {
    //            Debug.LogWarning("Attempting to access stash item at index " + (stashProp.startShowItemIndex + i) + " but stash count is only " + stash.Count);
    //            Debug.Log("I = " + i + "             startShowItemIndex = " + stashProp.startShowItemIndex);
    //        }
    //        slotObject.UpdateSlot(stash[stashProp.startShowItemIndex + i]);
    //        //根据索引设置插槽位置，按照行列来设置
    //        UpdatePositionHorizontal(offset.x, offset.y, stashProp.itemSlotWidth, stashProp.itemSlotHeight, stashProp.startShowItemIndex + i, slotObject);
    //        yield return null;

    //    }

    //}

    void CreateInventoryItemSlotNow(int maxShownItems)
    {
        //Debug.Log("Creating stash item slots...");
        //Debug.Log("stash items count : " + stash.Count);
        //根据当前ScrollView窗口能显示的最大物品数量来创建对应数量的材料仓库插槽UI
        int showItems = Mathf.Min(inventory.Count, maxShownItems);



        for (int i = 0; i < showItems; i++)
        {
            UI_ItemSlot slotObject = ItemSlotPool.Instance.Get(inventorySlotParent, false);
            inventoryItemSlot.Add(slotObject);

            if (inventoryProp.startShowItemIndex + i >= inventory.Count)
            {
                Debug.LogWarning("Attempting to access stash item at index " + (inventoryProp.startShowItemIndex + i) + " but stash count is only " + stash.Count);
                Debug.Log("I = " + i + "             startShowItemIndex = " + inventoryProp.startShowItemIndex);
            }
            slotObject.UpdateSlot(inventory[inventoryProp.startShowItemIndex + i]);
            //根据索引设置插槽位置，按照行列来设置
            UpdatePositionVertical(offset.x, offset.y, inventoryProp.itemSlotWidth, inventoryProp.itemSlotHeight, inventoryProp.startShowItemIndex + i, slotObject);

        }

    }



    void CreateStashItemSlotNow(int maxShownItems)
    {
        //Debug.Log("Creating stash item slots...");
        //Debug.Log("stash items count : " + stash.Count);
        //根据当前ScrollView窗口能显示的最大物品数量来创建对应数量的材料仓库插槽UI
        int showItems = Mathf.Min(stash.Count, maxShownItems);



        for (int i = 0; i < showItems; i++)
        {
            UI_ItemSlot slotObject = ItemSlotPool.Instance.Get(stashSlotParent, false);
            
            stashItemSlot.Add(slotObject);
            
            if(stashProp.startShowItemIndex + i >= stash.Count)
            {
                Debug.LogWarning("Attempting to access stash item at index " + (stashProp.startShowItemIndex + i) + " but stash count is only " + stash.Count);
                Debug.Log("I = " + i + "             startShowItemIndex = " + stashProp.startShowItemIndex);
            }
            slotObject.UpdateSlot(stash[stashProp.startShowItemIndex + i]);
            //根据索引设置插槽位置，按照行列来设置
            UpdatePositionHorizontal(offset.x, offset.y, stashProp.itemSlotWidth, stashProp.itemSlotHeight, stashProp.startShowItemIndex + i, slotObject);
            
        }

    }

    private void UpdateStashSlotShow()
    {
        for(int i = 0; i < stashItemSlot.Count; i++)
        {
            if (stashProp.startShowItemIndex + i < stash.Count)
            {
                if(stashItemSlot[i].gameObject.activeSelf == false)
                {
                    stashItemSlot[i].gameObject.SetActive(true);
                }
                stashItemSlot[i].UpdateSlot(stash[stashProp.startShowItemIndex + i]);
            }
            else
            {
                //如果没有足够的物品来填充所有插槽，隐藏多余的插槽
                stashItemSlot[i].gameObject.SetActive(false);
            }
            UpdatePositionHorizontal(offset.x, offset.y, stashProp.itemSlotWidth, stashProp.itemSlotHeight, stashProp.startShowItemIndex + i, stashItemSlot[i]);
        }

    }

    private void UpdateInventorySlotShow()
    {
        for (int i = 0; i < inventoryItemSlot.Count; i++)
        {
            if (inventoryProp.startShowItemIndex + i < inventory.Count)
            {
                if (inventoryItemSlot[i].gameObject.activeSelf == false)
                {
                    inventoryItemSlot[i].gameObject.SetActive(true);
                }
                inventoryItemSlot[i].UpdateSlot(inventory[inventoryProp.startShowItemIndex + i]);
            }
            else
            {
                //如果没有足够的物品来填充所有插槽，隐藏多余的插槽
                inventoryItemSlot[i].gameObject.SetActive(false);
            }
            UpdatePositionVertical(offset.x, offset.y, inventoryProp.itemSlotWidth, inventoryProp.itemSlotHeight, inventoryProp.startShowItemIndex + i, inventoryItemSlot[i]);
        }

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
        ////判断装备仓库是否已满，满了的话就无法入库
        //if (inventory.Count >= inventoryItemSlot.Count && !inventoryDictionary.Keys.Contains(_item))
        //{
        //    Debug.Log("No more space!");
        //    return false;
        //}
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
