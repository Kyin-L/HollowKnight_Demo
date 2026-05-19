using System;
using UnityEngine;

public interface IPoolManager
{
    public GameObject Get(string path);

    public GameObject Get(string directory, string name);

    public GameObject Get(GameObject prefab);

    public void Push(GameObject obj);

    public void Clear();
}
