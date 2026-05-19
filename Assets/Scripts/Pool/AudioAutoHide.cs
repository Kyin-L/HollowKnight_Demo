using UnityEngine;

public class AudioAutoHide : MonoBehaviour
{
    private AudioSource m_audioSource;
    private IPoolManager m_poolManager;

    void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
        m_poolManager = ManagerLocator.Get<IPoolManager>();
    }

    void Update()
    {
        if(m_audioSource.clip!= null && !m_audioSource.isPlaying)
        {
            m_audioSource.clip = null;
            m_poolManager.Push(gameObject);
        }
    }
}
