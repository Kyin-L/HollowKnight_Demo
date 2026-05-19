using Cinemachine;
using UnityEngine;

public class CollapserTute : MonoBehaviour
{
    [SerializeField] private AudioClip sound;
    [SerializeField] private float during = 2f;
    private Collider2D[] colliders;
    private Animator animator;
    private IAudioManager audioManager;
    private CinemachineImpulseSource impulseSource;

    private readonly string playerTag = "Player";
    void Start()
    {
        colliders = GetComponents<Collider2D>();
        animator = GetComponent<Animator>();
        audioManager = ManagerLocator.Get<IAudioManager>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            foreach(Collider2D c in colliders)
            {
                c.enabled = false;
            }

            animator.Play("Break");
            audioManager.Play(sound, transform.position);
            impulseSource.GenerateImpulseWithForce(2f);

            if(collision.TryGetComponent(out IInputReceiver player))
            {
                player.ControlDisable(during);
            }
        }
    }
}
