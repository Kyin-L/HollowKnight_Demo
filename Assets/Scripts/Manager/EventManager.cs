using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;


public class EventManager : SingletonBase<EventManager>, IEventManager
{
    private EventManager() {}

    private Dictionary<Type, List<EventInfo>> eventDic = new Dictionary<Type, List<EventInfo>>();

    private class EventInfo
    {
        public UnityAction<IEventHandler> handler { get; }
        public object originalHandler { get; }

        public EventInfo(UnityAction<IEventHandler> handler, object originalHandler)
        {
            this.handler = handler;
            this.originalHandler = originalHandler;
        }
    }

    public bool HasKey(Type key)
    {
        return eventDic.ContainsKey(key);
    }

    public void AddListener<T>(UnityAction<T> action) where T : IEventHandler
    {
        Type key = typeof(T);

        UnityAction<IEventHandler> eventHandler = (tmpEvent) => action((T)tmpEvent);

        if (!HasKey(key))
        {
            eventDic[key] = new List<EventInfo>();
        }

        eventDic[key].Add(new EventInfo(eventHandler, action));
    }

    public void RemoveListener<T>(UnityAction<T> action) where T : IEventHandler
    {
        Type key = typeof(T);

        if (HasKey(key))
        {
            var remove = eventDic[key].FirstOrDefault(w => w.originalHandler.Equals(action));

            if (remove != null)
            {
                eventDic[key].Remove(remove);

                if (eventDic[key].Count == 0)
                {
                    eventDic.Remove(key);
                }
            }
        }
    }

    public void EventTrigger(IEventHandler handler)
    {
        Type key = handler.GetType();
        
        if (HasKey(key))
        {
            var events = eventDic[key].ToArray();
            foreach (var evt in events)
            {
                evt.handler?.Invoke(handler);
            }
        }
    }

    public void Clear()
    {
        eventDic.Clear();
    }
}


