using UnityEngine;
using EventHandler.Menu;
using UnityEngine.EventSystems;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public ButtonPressedEventHandler.ButtonType m_buttonType;
    private Animator m_animator;

    private IEventManager m_eventManager;
    private IAudioManager m_audioManager;

    void Start()
    {
        m_eventManager = ManagerLocator.Get<IEventManager>();
        m_audioManager = ManagerLocator.Get<IAudioManager>();

        m_animator = GetComponent<Animator>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_animator.SetBool("selected", true);
        m_eventManager.EventTrigger(ButtonSelectedEventHandler.Get());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_animator.SetBool("selected", false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        m_animator.SetTrigger("pressed");
        m_eventManager.EventTrigger(ButtonPressedEventHandler.Get(m_buttonType));
    }

    public void PlayOneShot(AudioClip audioClip)
    {
        m_audioManager.PlayOneShot(audioClip);
    }
}
