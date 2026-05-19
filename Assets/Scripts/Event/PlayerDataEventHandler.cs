namespace EventHandler.PlayerData
{
    public class HpChangedEventHandler : IEventHandler
    {
        public int hp;

        public static HpChangedEventHandler Get(int hp)
        {
            return new HpChangedEventHandler { hp = hp };
        }
    }

    public class SoulChangedEventHandler : IEventHandler
    {
        public int souls;
        public static SoulChangedEventHandler Get(int souls)
        {
            return new SoulChangedEventHandler { souls = souls };
        }
    }

    public class GeoChangedEventHandler : IEventHandler
    {
        public int geo;
        public static GeoChangedEventHandler Get(int geo)
        {
            return new GeoChangedEventHandler { geo = geo };
        }
    }
}