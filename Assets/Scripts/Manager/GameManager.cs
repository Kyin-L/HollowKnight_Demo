using UnityEngine;

[DefaultExecutionOrder(-100)]
public class GameManager : MonoSingletonBase<GameManager>
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        RegisterMonoManager();
        RegisterEventManager();

        RegisterResourceManager(); //mono
        RegisterConfigManager();//resource
        RegisterDataManager(); //config
        RegisterPoolManager(); //resourece

        RegisterAudioManager(); //resource,event,pool
        RegisterSceneLoadManager(); //resource,mono,data
    }

    private void RegisterMonoManager()
    {
        ManagerLocator.Register<IMonoManager>(MonoManager.Instance);
    }

    private void RegisterEventManager()
    {
        ManagerLocator.Register<IEventManager>(EventManager.Instance);
    }

    private void RegisterResourceManager()
    {
        ManagerLocator.Register<IResourceManager>(ResourceManager.Instance);
    }

    private void RegisterPoolManager()
    {
        ManagerLocator.Register<IPoolManager>(PoolManager.Instance);
    }

    private void RegisterAudioManager()
    {
        ManagerLocator.Register<IAudioManager>(AudioManager.Instance);
    }

    private void RegisterSceneLoadManager()
    {
        ManagerLocator.Register<ISceneLoadManager>(SceneLoadManager.Instance);
    }

    private void RegisterConfigManager()
    {
        ManagerLocator.Register<ConfigManager>(ConfigManager.Instance);
    }

    private void RegisterDataManager()
    {
        ManagerLocator.Register<DataManager>(DataManager.Instance);
        DataManager.Instance.Init(ManagerLocator.Get<ConfigManager>());
    }
}
