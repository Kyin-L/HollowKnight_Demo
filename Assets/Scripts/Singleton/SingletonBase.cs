using System;
using UnityEngine;
using System.Reflection;

public abstract class SingletonBase<T> where T: class
{
	private static T m_instance;

	public static T Instance
	{
		get
		{
			if (m_instance == null)
            {
				Type type = typeof(T);
				ConstructorInfo info =  type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic,
															null,
															Type.EmptyTypes,
															null);
				if (info != null)
					m_instance = info.Invoke(null) as T;
				else
					Debug.LogError(type.ToString() + " is Public");
            }

			return m_instance;
		}
	}

	public static T GetInstance()
    {
		return Instance;
    }

    protected virtual void Initialize()
    {

    }

    protected SingletonBase()
    {
		Initialize();
    }
}
