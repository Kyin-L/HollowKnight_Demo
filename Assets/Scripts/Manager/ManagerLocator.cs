using System;
using System.Collections.Generic;

public static class ManagerLocator
{
    private static readonly Dictionary<Type, object> m_services = new Dictionary<Type, object>();

    public static void Register<T>(T service) where T : class
    {
        var type = typeof(T);
        if (!m_services.ContainsKey(type))
            m_services[type] = service;
    }

    public static T Get<T>() where T : class
    {
        var type = typeof(T);
        if (m_services.TryGetValue(type, out var service))
            return service as T;

        throw new Exception($"服务 {type} 未注册");
    }

    public static void Unregister<T>() where T : class
    {
        m_services.Remove(typeof(T));
    }
}