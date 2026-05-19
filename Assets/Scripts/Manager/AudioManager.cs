using UnityEngine;
using UnityEngine.Audio;
using EventHandler.Menu;

public class AudioManager : SingletonBase<AudioManager>, IAudioManager
{
    private AudioManager() { }

    public AudioMixer m_audioMixer;
    private AudioSource m_audioBGM;
    private AudioSource m_audioEffect;

    private IResourceManager m_resourceManager;
    private IEventManager m_eventManager;
    private IPoolManager m_poolManager;

    private string m_audioManagerPrefabPath = "Prefabs/Audio/AudioManager";
    private string m_audioPrefabPath = "Prefabs/Audio/Audio";
    private string m_audioMixerPath = "AudioMixer/Actors";

    protected override void Initialize()
    {
        m_resourceManager = ManagerLocator.Get<IResourceManager>();
        m_eventManager = ManagerLocator.Get<IEventManager>();
        m_poolManager = ManagerLocator.Get<IPoolManager>();

        GameObject audio = GameObject.Find("GameManager/Audio");
        if (audio == null)
        {
            audio = GameObject.Instantiate(m_resourceManager.Load<GameObject>(m_audioManagerPrefabPath));
            audio.name = "AudioManager";
            audio.transform.SetParent(GameObject.Find("GameManager").transform);
            m_audioBGM = audio.transform.Find("BGM").GetComponent<AudioSource>();
            m_audioEffect = audio.transform.Find("Effect").GetComponent<AudioSource>();
            m_audioMixer = m_resourceManager.Load<AudioMixer>(m_audioMixerPath);
            m_eventManager.AddListener<SliderChangeEventHandler>(OnSliderChange);
        }
    }

    public void Play(AudioClip audioClip)
    {
        m_audioBGM.clip = audioClip;
        m_audioBGM.Play();
    }

    public void Play(AudioClip audioClip,Vector2 position,float volume = 1)
    {
        GameObject audioGO = m_poolManager.Get(m_audioPrefabPath);
        audioGO.transform.position = position;
        AudioSource audio = audioGO.GetComponent<AudioSource>();
        audio.clip = audioClip;
        audio.volume = volume;
        audio.Play();
    }

    public void PlayOneShot(AudioClip audioClip)
    {
        m_audioEffect.PlayOneShot(audioClip);
    }

    private void OnSliderChange(SliderChangeEventHandler handler)
    {
        switch (handler.sliderType)
        {
            case SliderChangeEventHandler.SliderType.Master:
                SetMasterVolume(handler.volume);
                break;
            case SliderChangeEventHandler.SliderType.Music:
                SetMusicVolume(handler.volume);
                break;
            case SliderChangeEventHandler.SliderType.Effect:
                SetSoundVolume(handler.volume);
                break;
        }
    }


    private void SetMasterVolume(float volume)
    {
        m_audioMixer.SetFloat("MasterVolume", -6 * (10 - volume));
    }

    private void SetSoundVolume(float volume)
    {
        m_audioMixer.SetFloat("SoundVolume", -6 * (10 - volume));
    }

    private void SetMusicVolume(float volume)
    {
        m_audioMixer.SetFloat("MusicVolume", -6 * (10 - volume));
    }
}
