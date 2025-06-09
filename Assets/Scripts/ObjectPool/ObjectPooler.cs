using System;
using System.Collections.Generic;
using UnityEngine;
public class ObjectPooler<T> 
{
    private Queue<T> _pool;
    private Transform _poolHolder;
    private GameObject _prefab;

    public ObjectPooler(GameObject prefab, Transform poolHolder, int initNum)
    {
        _prefab = prefab;
        _poolHolder = poolHolder;
        _pool = new Queue<T>();

        for (int i = 0; i < initNum; i++)
        {
            GameObject elem = GameObject.Instantiate(_prefab, _poolHolder.position, Quaternion.identity);
            elem.transform.SetParent(poolHolder.transform);
            T elemT = elem.GetComponent<T>();
            elem.SetActive(false);
            _pool.Enqueue(elemT);
        }
    }

    public T GetElem(bool isActive = false)
    {
        if (_pool.Count == 0)
        {
            GameObject elem = GameObject.Instantiate(_prefab, _poolHolder.position, Quaternion.identity);
            T elemT = elem.GetComponent<T>();
            if(isActive) elem.SetActive(true);
            return elemT;
        }
        else
        {
            T elem = _pool.Dequeue();
            if (isActive && elem is GameObject) (elem as GameObject).SetActive(true);
            return elem;
        }
    }

    public void ReturnPool(T elem, bool isInactive = false)
    {
        _pool.Enqueue(elem); // Add to UnusedPool
        
        if(isInactive && elem is GameObject) (elem as GameObject).SetActive(false);
    }
}