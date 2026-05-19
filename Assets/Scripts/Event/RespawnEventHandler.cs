
namespace EventHandler.Respawn
{
    public class ScreenToBlackEventHandler : IEventHandler
    {
        public IEventHandler eventHandler;

        public static ScreenToBlackEventHandler Get(IEventHandler handler)
        {
            return new ScreenToBlackEventHandler { eventHandler = handler };
        }
    }

    public class ReplaceEventHandler : IEventHandler
    {
        public static ReplaceEventHandler Get()
        {
            return new ReplaceEventHandler { };
        }
    }

    public class RespawnEventHandler : IEventHandler
    {
        public static RespawnEventHandler Get()
        {
            return new RespawnEventHandler { };
        }
    }
}
