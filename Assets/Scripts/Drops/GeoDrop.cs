using UnityEngine;

public class GeoDrop : MonoBehaviour
{
    [SerializeField] private GeoDropConfig geoDropConfig;
    [SerializeField] private Collider2D trigger;
    private IPoolManager poolManager;
    private IAudioManager audioManager;

    private readonly string playerTag = "Player";

    void Start()
    {
        poolManager = ManagerLocator.Get<IPoolManager>();
        audioManager = ManagerLocator.Get<IAudioManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            trigger.enabled = true;

            if (geoDropConfig.groundSounds != null && geoDropConfig.groundSounds.Length > 0)
            {
                int index = Random.Range(0, geoDropConfig.groundSounds.Length);
                audioManager.Play(geoDropConfig.groundSounds[index], gameObject.transform.position);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            if (collision.TryGetComponent<IPickUp>(out IPickUp pickUp))
            {
                pickUp.AddGeo(geoDropConfig.amount);
                poolManager.Push(gameObject);
            }
        }
    }
}
