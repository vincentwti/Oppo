using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Pool
{
    public string poolName;
    public GameObject prefab;
    public Transform parent;
    public int size;
    public bool expandable;
}

public class SimplePooling : MonoBehaviour
{
    public List<Pool> poolList;

    protected Dictionary<string, List<GameObject>> itemDict = new Dictionary<string, List<GameObject>>();

    protected virtual void Start()
    {
        Debug.Log("pool count : " + poolList.Count);
        for (int i = 0; i < poolList.Count; i++)
        {
            Debug.Log("pool size : " + poolList[i].size);
            for (int j = 0; j < poolList[i].size; j++)
            {
                SpawnItem(poolList[i], false);
            }
        }
    }

    protected GameObject SpawnItem(Pool pool, bool enableItem)
    {
        GameObject go = Instantiate(pool.prefab, pool.parent, false);
        go.transform.localPosition = Vector3.zero;
        go.transform.localEulerAngles = Vector3.zero;
        go.transform.localScale = Vector3.one;
        go.SetActive(enableItem);
        if (itemDict.ContainsKey(pool.poolName))
        {
            itemDict[pool.poolName].Add(go);
        }
        else
        {
            itemDict.Add(pool.poolName, new List<GameObject> { go });
        }
        return go;
    }

    public GameObject GetItem(string poolName)
    {
        try
        {
            Pool pool = poolList.Where(x => x.poolName == poolName).Single();
            for (int i = 0; i < itemDict[poolName].Count; i++)
            {
                GameObject go = itemDict[poolName][i];
                if (!go.activeSelf)
                {
                    go.SetActive(true);
                    return go;
                }
            }

            if (pool.expandable)
            {
                return SpawnItem(pool, true);
            }
        }
        catch
        {
            return null;
        }
        return null;
    }
}
