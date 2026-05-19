using TMPro;
using UnityEngine;
using EventHandler.PlayerData;

public class HUDGeo : MonoBehaviour
{
    private Animator animator;
    private TextMeshProUGUI text;

    private IEventManager eventManager;

    private int showHash = Animator.StringToHash("Show");

    void Awake()
    {
        animator = GetComponent<Animator>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Initialized(int geo)
    {
        text.text = geo.ToString();
        eventManager = ManagerLocator.Get<IEventManager>();
        eventManager.AddListener<GeoChangedEventHandler>(OnGeoChanged);
    }

    public void Show()
    {

    }

    private void OnGeoChanged(GeoChangedEventHandler handler)
    {
        text.text = handler.geo.ToString();
    }
}