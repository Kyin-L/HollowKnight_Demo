using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public interface IMonoManager
{
    public event UnityAction m_update;

    public void AddListener(UnityAction func);

    public void RemoveLinstener(UnityAction func);

    Coroutine StartCoroutine(IEnumerator routine);
}
