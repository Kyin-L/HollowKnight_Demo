using UnityEngine;

[DefaultExecutionOrder(-99)]
public class LevelManager : MonoSingletonBase<LevelManager>
{
    public GameObject player;
    public GameObject ui;
    public GameObject currentLevel;

    [SerializeField] private RespawnManager respawnManager;
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private LevelConfig levelConfig;

    private DataManager dataManager;
    private IAudioManager audioManager;
    private IPoolManager poolManager;

    void Start()
    {
        GetManager();

        BuildLevel();
    }

    private void GetManager()
    {
        dataManager = ManagerLocator.Get<DataManager>();
        dataManager.playerData.Respawn();

        audioManager = ManagerLocator.Get<IAudioManager>();
        audioManager.Play(levelConfig.BGM);

        poolManager = ManagerLocator.Get<IPoolManager>();
        poolManager.Clear();
    }

    private void BuildLevel()
    {
        player = poolManager.Get(gameConfig.player);
        ui = poolManager.Get(gameConfig.ui);
        currentLevel = poolManager.Get(levelConfig.levelPrefab);
    }
}
