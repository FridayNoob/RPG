using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> : Singeton<ObjectPool<T>> where T : Component
{
    [Header("对象池预制体")]
    public T prefab;
    [Header("对象池最大容量")]
    public int maxCapacity = 99999;
    [Header("对象池初始容量")]
    public int preloadCount = 100;
    [Header("对象池每帧创建数量")]
    public int createPerFrame = 3;
    [Header("对象超时时间")]
    public float idleDestroyTime = 30f;

    // 底层存储：队列 存储闲置对象
    private Queue<T> _idleQueue = new Queue<T>();
    // 记录对象的闲置开始时间（用于超时判断）
    private Dictionary<T, float> _idleTimeDict = new Dictionary<T, float>();
    // 正在使用的对象数量
    private int _activeCount = 0;
    // 对象池父物体（规范层级）
    private Transform _poolParent;

    protected override void Awake()
    {
        base.Awake();
        InitPoolParent();
        // 启动协程：分步预加载对象
        StartCoroutine(PreloadObjectsAsync());
        // 启动协程：定时检测超时对象
        StartCoroutine(CheckTimeoutObjectsCoroutine());
    }

    #region 初始化
    /// <summary>
    /// 创建对象池父物体
    /// </summary>
    private void InitPoolParent()
    {
        GameObject parent = new GameObject($"Pool_{typeof(T).Name}");
        parent.transform.SetParent(transform);
        _poolParent = parent.transform;
    }
    #endregion

    #region 协程：分步预加载对象（核心：防卡顿）
    private IEnumerator PreloadObjectsAsync()
    {
        for (int i = 0; i < preloadCount; i++)
        {
            // 达到容量上限，停止创建
            if (GetTotalCount() >= maxCapacity) break;

            CreateNewObject();

            // 每创建 N 个，等待一帧（分步创建）
            if ((i + 1) % createPerFrame == 0)
                yield return null;
        }
    }
    #endregion

    #region 核心：获取对象
    /// <summary>
    /// 从对象池获取对象
    /// </summary>
    public T Get()
    {
        // 无闲置对象 → 新建对象（不超容量）
        if (_idleQueue.Count == 0)
        {
            if (GetTotalCount() >= maxCapacity)
            {
                Debug.LogWarning($"对象池 {typeof(T).Name} 已达最大容量 {maxCapacity}，无法创建新对象！");
                return null;
            }
            CreateNewObject();
        }

        // 从队列取出闲置对象
        T obj = _idleQueue.Dequeue();
        _idleTimeDict.Remove(obj);
        _activeCount++;

        obj.gameObject.SetActive(true);
        return obj;
    }
    #endregion

    public T Get(Transform parent, bool worldPositionStays = false)
    {
        // 无闲置对象 → 新建对象（不超容量）
        if (_idleQueue.Count == 0)
        {
            if (GetTotalCount() >= maxCapacity)
            {
                Debug.LogWarning($"对象池 {typeof(T).Name} 已达最大容量 {maxCapacity}，无法创建新对象！");
                return null;
            }
            CreateNewObject();
        }

        // 从队列取出闲置对象
        T obj = _idleQueue.Dequeue();
        _idleTimeDict.Remove(obj);
        _activeCount++;

        // 先设父级，再激活，避免在错误父级或 poolParent 下造成短暂可见或 Layout 重建
        obj.transform.SetParent(parent, worldPositionStays);
        obj.transform.localScale = Vector3.one; // 确保缩放正确（如果需要）
        obj.gameObject.SetActive(true);
        return obj;
    }

    #region 核心：回收对象
    /// <summary>
    /// 回收对象到对象池
    /// </summary>
    public void Release(T obj)
    {
        if (obj == null) return;

        obj.gameObject.SetActive(false);
        obj.transform.SetParent(_poolParent);

        // 加入闲置队列 + 记录回收时间
        _idleQueue.Enqueue(obj);
        _idleTimeDict[obj] = Time.time;
        _activeCount--;
    }
    #endregion

    #region 协程：超时销毁闲置对象（核心：省内存）
    private IEnumerator CheckTimeoutObjectsCoroutine()
    {
        WaitForSeconds wait = new WaitForSeconds(5f); // 每5秒检测一次
        while (true)
        {
            yield return wait;
            DestroyTimeoutObjects();
        }
    }

    /// <summary>
    /// 销毁超时闲置的对象
    /// </summary>
    private void DestroyTimeoutObjects()
    {
        List<T> timeoutList = new List<T>();

        // 遍历找出超时对象
        foreach (var kvp in _idleTimeDict)
        {
            if (Time.time - kvp.Value > idleDestroyTime)
            {
                timeoutList.Add(kvp.Key);
            }
        }

        // 销毁并移除超时对象
        foreach (var obj in timeoutList)
        {
            if (_idleQueue.Count > 0 && _idleQueue.Peek() == obj)
            {
                _idleQueue.Dequeue();
            }
            _idleTimeDict.Remove(obj);
            Destroy(obj.gameObject);
        }
    }
    #endregion

    #region 工具方法
    /// <summary>
    /// 创建新对象并直接回收
    /// </summary>
    private void CreateNewObject()
    {
        T obj = Instantiate(prefab, _poolParent);
        obj.gameObject.SetActive(false);
        _idleQueue.Enqueue(obj);
        _idleTimeDict[obj] = Time.time;
    }

    /// <summary>
    /// 获取总对象数（闲置+使用）
    /// </summary>
    public int GetTotalCount() => _idleQueue.Count + _activeCount;

    /// <summary>
    /// 清空对象池
    /// </summary>
    public void ClearPool()
    {
        while (_idleQueue.Count > 0)
        {
            T obj = _idleQueue.Dequeue();
            Destroy(obj.gameObject);
        }
        _idleTimeDict.Clear();
        _activeCount = 0;
    }
    #endregion

    private void OnDestroy()
    {
        StopAllCoroutines();
        ClearPool();
    }
}
