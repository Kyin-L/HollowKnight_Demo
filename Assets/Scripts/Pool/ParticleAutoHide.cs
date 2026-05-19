using UnityEngine;

public class ParticleAutoHide : MonoBehaviour
{
    private ParticleSystem m_particle;
    private IPoolManager m_poolManager;

    void Awake()
    {
        m_particle = GetComponent<ParticleSystem>();
        m_poolManager = ManagerLocator.Get<IPoolManager>();

        var main = m_particle.main;
        main.stopAction = ParticleSystemStopAction.Callback;
    }

    void OnParticleSystemStopped()
    {
        m_poolManager.Push(gameObject);
    }
}