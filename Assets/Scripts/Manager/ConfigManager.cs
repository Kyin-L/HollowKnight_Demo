public class ConfigManager : SingletonBase<ConfigManager>
{
    public PlayerConfig playerConfig;
    public BreakableConfig breakableConfig;

    private IResourceManager resourceManager;

    private readonly string playerConfigPath = "ScriptableObject/Player/PlayerConfig";

    private ConfigManager()
    {
        resourceManager = ManagerLocator.Get<IResourceManager>();

        playerConfig = resourceManager.Load<PlayerConfig>(playerConfigPath);
    }
}
