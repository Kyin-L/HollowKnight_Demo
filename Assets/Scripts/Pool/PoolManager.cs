using System.Collections.Generic;
using UnityEngine;

public class PoolManager : SingletonBase<PoolManager>, IPoolManager
{
    private Dictionary<string, PoolData> m_poolDic = new Dictionary<string, PoolData>();

    private GameObject m_pool;

    private IResourceManager m_resourceManager;

    private PoolManager() 
    {
        m_resourceManager = ManagerLocator.Get<IResourceManager>();
    }

    public GameObject Get(string path)
    {
        string name = Tools.Path.GetName(path);
        GameObject obj;

        if (m_poolDic.ContainsKey(name) && m_poolDic[name].poolQueue.Count > 0)
        {
            obj = m_poolDic[name].Get();
        }
        else
        {
            obj = GameObject.Instantiate(m_resourceManager.Load<GameObject>(path));
            obj.name = name;
        }

        return obj;
    }

    public GameObject Get(string directory, string name)
    {
        return Get(directory + "/" + name);
    }

    public GameObject Get(GameObject prefab)
    {
        GameObject obj;
        string name = prefab.name;

        if (m_poolDic.ContainsKey(name) && m_poolDic[name].poolQueue.Count > 0)
        {
            obj = m_poolDic[name].Get();
        }
        else
        {
            obj = GameObject.Instantiate(prefab);
            obj.name = name;
        }

        return obj;
    }

    public void Push(GameObject obj)
    {
        if (m_pool == null)
            m_pool = new GameObject("Pool");

        if (m_poolDic.ContainsKey(obj.name))
        {
            m_poolDic[obj.name].Push(obj);
        }
        else
        {
            m_poolDic.Add(obj.name, new PoolData(obj, m_pool));
        }
    }

    public void Clear()
    {
        m_pool = null;
        m_poolDic.Clear();
    }
}

public class PoolData
{
    public GameObject fatherObj;
    public Queue<GameObject> poolQueue;

    public PoolData(GameObject obj, GameObject poolObj)
    {
        fatherObj = new GameObject(obj.name + "Pool");
        fatherObj.transform.SetParent(poolObj.transform);
        poolQueue = new Queue<GameObject>();
        poolQueue.Enqueue(obj);
        obj.transform.SetParent(fatherObj.transform);
        obj.SetActive(false);
    }

    public GameObject Get()
    {
        GameObject obj;
        obj = poolQueue.Dequeue();
        obj.transform.SetParent(null);
        obj.SetActive(true);
        return obj;
    }

    public void Push(GameObject obj)
    {
        obj.SetActive(false);
        poolQueue.Enqueue(obj);
        obj.transform.SetParent(fatherObj.transform);
    }
}