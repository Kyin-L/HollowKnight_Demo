using System;
using UnityEngine.Events;

public interface IEventManager
{
    public bool HasKey(Type key);

    public void AddListener<T>(UnityAction<T> action) where T : IEventHandler;

    public void RemoveListener<T>(UnityAction<T> action) where T : IEventHandler;

    public void EventTrigger(IEventHandler handler);

    public void Clear();
}

