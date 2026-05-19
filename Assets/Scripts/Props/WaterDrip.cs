using UnityEngine;

public class WaterDrip : MonoBehaviour
{
    [SerializeField] private AudioClip[] sounds;

    private Animator animator;
    private Rigidbody2D rb;
    private IPoolManager poolManager;
    private IAudioManager audioManager;

    private int groundLayer;
    private readonly int onGroundHash = Animator.StringToHash("OnGround");

    void Awake()
    {
        poolManager = ManagerLocator.Get<IPoolManager>();
        audioManager = ManagerLocator.Get<IAudioManager>();

        groundLayer = LayerMask.NameToLayer("Ground");
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == groundLayer)
        {
            animator.SetTrigger(onGroundHash);
            int index = Random.Range(0, sounds.Length);
            audioManager.Play(sounds[index],transform.position);
        }
    }

    public void Hide()
    {
        poolManager.Push(gameObject);
    }
}
