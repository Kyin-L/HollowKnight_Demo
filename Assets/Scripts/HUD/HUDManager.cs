using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using EventHandler.Respawn;

public class HUDManager : MonoBehaviour
{
    private bool isShow = false;

    private IEventManager eventManager;
    private HUDSouls souls;
    private HUDHealth health;
    private HUDGeo geo;
    private Image black;

    private PlayableDirector playableDirector;

    private DataManager dataManager;

    private WaitForSecondsRealtime wait = new WaitForSecondsRealtime(0.5f);

    private void Awake()
    {
        eventManager = ManagerLocator.Get<IEventManager>();
        souls = GetComponentInChildren<HUDSouls>();
        health = GetComponentInChildren<HUDHealth>();
        geo = GetComponentInChildren<HUDGeo>();
        playableDirector = GetComponent<PlayableDirector>();
        black = transform.Find("Black").GetComponent<Image>();
        dataManager = ManagerLocator.Get<DataManager>();
    }

    void Start()
    {
        souls.Initialized(dataManager.playerData.Souls);
        health.Initialized(dataManager.playerData.HP);
        geo.Initialized(dataManager.playerData.Geo);

        eventManager.AddListener<ScreenToBlackEventHandler>(ScreenToBlack);
    }

    public void Show()
    {
        if (!isShow)
        {
            isShow = true;
            playableDirector.Play();
        }
    }

    public void ScreenToBlack(ScreenToBlackEventHandler handler)
    {
        StartCoroutine(IEScreenToBlack(handler.eventHandler));
    }

    public IEnumerator IEScreenToBlack(IEventHandler handler)
    {
        Time.timeScale = 0.1f;
        yield return wait;

        Color color = black.color;
        float time = 0;
        while (time < 1)
        {
            time += Time.unscaledDeltaTime*2f;
            color.a = Mathf.Lerp(0, 1, time);
            black.color = color;
            yield return null;
        }
        Time.timeScale = 1f;
        eventManager.EventTrigger(handler);
    }
}

