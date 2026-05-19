using UnityEngine;

public class MonoSingletonBase<T> : MonoBehaviour where T: MonoBehaviour
{
	private static T m_instance;

	public static T Instance
	{
		get
		{
			if(m_instance == null)
			{
				m_instance = FindObjectOfType<T>();

				if (m_instance == null)
				{
					GameObject obj = new GameObject();
					obj.name = typeof(T).ToString();
					m_instance = obj.AddComponent<T>();
				}
			}
			return m_instance;
		}
	}

	public static T GetInstance()
    {
		return Instance;
    }

    private void OnDestroy()
    {
		if (m_instance == this)
			m_instance = default(T);
    }

	protected virtual void Initialize()
    {

    }

	protected MonoSingletonBase() 
	{
		Initialize();
	}
}
