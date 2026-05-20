using UnityEngine;

[DefaultExecutionOrder(-99)]
public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private RespawnManager respawnManager;
    [SerializeField] private LevelConfig levelConfig;

    private DataManager dataManager;
    private IAudioManager audioManager;
    private IPoolManager poolManager;

    void Start()
    {
        dataManager = ManagerLocator.Get<DataManager>();
        dataManager.playerData.Respawn();

        if (dataManager.levelData != null)
            respawnManager.Enter(dataManager.levelData.enterpointID);

        audioManager = ManagerLocator.Get<IAudioManager>();
        audioManager.Play(levelConfig.BGM);

        poolManager = ManagerLocator.Get<IPoolManager>();
        poolManager.Clear();
    }

    void Update()
    {
        
    }
}
