using UnityEngine;
using UnityEngine.UI;
using EventHandler.Menu;

public class MenuSlider : MonoBehaviour
{
    public SliderChangeEventHandler.SliderType m_sliderType;
    private Slider m_slider;

    private IEventManager m_eventManager;

    void Start()
    {
        m_eventManager = ManagerLocator.Get<IEventManager>();

        m_slider = GetComponent<Slider>();
        m_slider.onValueChanged.AddListener((float value) => m_eventManager.EventTrigger(SliderChangeEventHandler.Get(m_sliderType, (int)value)));
    }
}
