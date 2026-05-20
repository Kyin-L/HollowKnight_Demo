using UnityEngine;

public class Stalactite : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private AudioClip sound;
    [SerializeField] private Collider2D trigger;

    private IPoolManager poolManager;
    private IAudioManager audioManager;

    private int groundLayer;
    private readonly string playerTag = "Player";
    void Start()
    {
        groundLayer = LayerMask.NameToLayer("Ground");
        poolManager = ManagerLocator.Get<IPoolManager>();
        audioManager = ManagerLocator.Get<IAudioManager>();
    }

    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == groundLayer)
        {
            audioManager.Play(sound, transform.position);
            trigger.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(playerTag))
        {
            if(collision.TryGetComponent(out IDamagable damagable))
            {
                damagable.TakeDamage(new DamageInfo(damage, gameObject));
            }
        }
    }
}
