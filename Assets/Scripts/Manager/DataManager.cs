public class DataManager : SingletonBase<DataManager>
{
    public PlayerData playerData;
    public LevelData levelData;

    private DataManager() 
    { 

    }

    public void Init(ConfigManager configManager)
    {
        playerData = new(configManager.playerConfig);
    }
}
