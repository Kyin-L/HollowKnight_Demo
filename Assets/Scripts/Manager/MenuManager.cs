using System.Collections;
using UnityEngine;
using EventHandler.Menu;

public class MenuManager : MonoBehaviour
{
    public MenuConfig m_menuConfig;
    public Animator m_mainMenuAnimator;
    public Animator m_optionMenuAnimator;

    private int m_animatorMainMenuFadeInHash;
    private int m_animatorMainMenuFadeOutHash;
    private int m_animatorOptionMenuFadeInHash;
    private int m_animatorOptionMenuFadeOutHash;

    private IAudioManager m_audioManager;
    private IEventManager m_eventManager;

    void Start()
    {
        m_eventManager = ManagerLocator.Get<IEventManager>();
        m_audioManager = ManagerLocator.Get<IAudioManager>();

        m_eventManager.AddListener<ButtonPressedEventHandler>(OnButtonPressed);
        m_eventManager.AddListener<ButtonSelectedEventHandler>(OnButtonSelected);

        m_audioManager.Play(m_menuConfig.BGM);

        m_animatorMainMenuFadeInHash = Animator.StringToHash("MainMenuFadeIn");
        m_animatorMainMenuFadeOutHash = Animator.StringToHash("MainMenuFadeOut");
        m_animatorOptionMenuFadeInHash = Animator.StringToHash("OptionMenuFadeIn");
        m_animatorOptionMenuFadeOutHash = Animator.StringToHash("OptionMenuFadeOut");
    }

    void OnDestroy()
    {
        m_eventManager.RemoveListener<ButtonPressedEventHandler>(OnButtonPressed);
        m_eventManager.RemoveListener<ButtonSelectedEventHandler>(OnButtonSelected);
    }

    private void OnButtonPressed(ButtonPressedEventHandler handler)
    {
        m_audioManager.PlayOneShot(m_menuConfig.effectPressed);
        switch (handler.buttonType)
        {
            case ButtonPressedEventHandler.ButtonType.GameStart:
                LevelData data = new LevelData("Cave", 0);
                ManagerLocator.Get<ISceneLoadManager>().DelayLoadScene(data);
                break;
            case ButtonPressedEventHandler.ButtonType.Options:
                StartCoroutine(MainMenuChangeToOptionMenu());
                break;
            case ButtonPressedEventHandler.ButtonType.GameExit:
                Application.Quit();
                break;
            case ButtonPressedEventHandler.ButtonType.OptionBack:
                StartCoroutine(OptionMenuChangeToMainMenu());
                break;
        }
    }

    private void OnButtonSelected(ButtonSelectedEventHandler handler)
    {
        m_audioManager.PlayOneShot(m_menuConfig.effectSelect);
    }

    private IEnumerator MainMenuChangeToOptionMenu()
    {
        m_mainMenuAnimator.Play(m_animatorMainMenuFadeOutHash);
        yield return null;

        yield return new WaitUntil(() =>
        {
            AnimatorStateInfo state = m_mainMenuAnimator.GetCurrentAnimatorStateInfo(0);
            return state.shortNameHash == m_animatorMainMenuFadeOutHash;
        });

        yield return new WaitWhile(() =>
        {
            AnimatorStateInfo state = m_mainMenuAnimator.GetCurrentAnimatorStateInfo(0);
            return state.normalizedTime < 1f;
        });

        m_optionMenuAnimator.Play(m_animatorOptionMenuFadeInHash);
    }

    private IEnumerator OptionMenuChangeToMainMenu()
    {
        m_optionMenuAnimator.Play(m_animatorOptionMenuFadeOutHash);
        yield return null;

        yield return new WaitUntil(() =>
        {
            AnimatorStateInfo state = m_optionMenuAnimator.GetCurrentAnimatorStateInfo(0);
            return state.shortNameHash == m_animatorOptionMenuFadeOutHash;
        });

        yield return new WaitWhile(() =>
        {
            AnimatorStateInfo state = m_optionMenuAnimator.GetCurrentAnimatorStateInfo(0);
            return state.normalizedTime < 1f;
        });

        m_mainMenuAnimator.Play(m_animatorMainMenuFadeInHash);
    }
}
