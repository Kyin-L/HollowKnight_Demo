using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MonoManager : MonoSingletonBase<MonoManager>, IMonoManager
{
    public event UnityAction m_update;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Update()
    {
        m_update?.Invoke();
    }

    public void AddListener(UnityAction func)
    {
        m_update += func;
    }

    public void RemoveLinstener(UnityAction func)
    {
        m_update -= func;
    }

    Coroutine IMonoManager.StartCoroutine(IEnumerator routine)
    {
        return StartCoroutine(routine);
    }
}