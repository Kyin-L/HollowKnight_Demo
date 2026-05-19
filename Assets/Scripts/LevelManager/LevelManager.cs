using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private RespawnManager respawnManager;
    [SerializeField] private LevelConfig levelConfig;

    private DataManager dataManager;
    private IAudioManager audioManager;


    void Start()
    {
        dataManager = ManagerLocator.Get<DataManager>();

        if (dataManager.levelData != null)
            respawnManager.Enter(dataManager.levelData.enterpointID);

        audioManager = ManagerLocator.Get<IAudioManager>();
        audioManager.Play(levelConfig.BGM);
    }

    void Update()
    {
        
    }
}
