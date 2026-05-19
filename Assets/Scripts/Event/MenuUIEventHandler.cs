namespace EventHandler.Menu
{
    public class ButtonPressedEventHandler : IEventHandler
    {
        public enum ButtonType
        {
            GameStart,
            Options,
            GameExit,
            OptionBack,
        }
        public ButtonType buttonType;

        public static ButtonPressedEventHandler Get(ButtonType bt)
        {
            return new ButtonPressedEventHandler { buttonType = bt };
        }
    }

    public class SliderChangeEventHandler : IEventHandler
    {
        public enum SliderType
        {
            Master,
            Music,
            Effect,
        }
        public SliderType sliderType;
        public int volume;

        public static SliderChangeEventHandler Get(SliderType st, int volume)
        {
            return new SliderChangeEventHandler { sliderType = st, volume = volume };
        }
    }


    public class ButtonSelectedEventHandler : IEventHandler
    {
        public string str;

        public static ButtonSelectedEventHandler Get()
        {
            return new ButtonSelectedEventHandler { };
        }
    }
}