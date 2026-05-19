using UnityEngine;

public class StalactiteTrap : MonoBehaviour
{
    [SerializeField] private GameObject stalactiteBreakablePrefab;
    [SerializeField] private AudioClip sound;

    private Collider2D trigger;
    private Animator animator;
    private IPoolManager poolManager;
    private IAudioManager audioManager;

    private readonly string playerTag = "Player";

    private readonly string FallHash = "Fall";

    void Start()
    {
        trigger = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        poolManager = ManagerLocator.Get<IPoolManager>();
        audioManager = ManagerLocator.Get<IAudioManager>();
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(playerTag))
        {
            trigger.enabled = false;
            animator.Play(FallHash);
            audioManager.Play(sound, transform.position);
        }
    }

    public void SpawnStalactiteBreakable()
    {
        GameObject stalactiteBreakable = poolManager.Get(stalactiteBreakablePrefab);
        stalactiteBreakable.transform.position = transform.position;
    }
}
