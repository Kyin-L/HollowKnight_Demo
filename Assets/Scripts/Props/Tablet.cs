using UnityEngine;

public class Tablet : MonoBehaviour
{
    [SerializeField] private float fadeSpeed = 2f;
    [SerializeField] private SpriteRenderer[] lights;
    [SerializeField] private ParticleSystem glows;
    [SerializeField] private ParticleSystem rim;
    [SerializeField] private AudioClip sound;
    [SerializeField] private AnimationCurve curve;
    private ParticleSystem.EmissionModule emission;

    private float interpolation = 0;
    private float currentAlpha = 0;
    private float targetAlpha = 0;

    private IAudioManager audioManager;

    private readonly string playerTag = "Player";

    void Start()
    {
        emission = glows.emission;
        audioManager = ManagerLocator.Get<IAudioManager>();
    }

    void Update()
    {
        if (currentAlpha == targetAlpha)
            return;

        interpolation = Mathf.MoveTowards(interpolation, targetAlpha, Time.deltaTime * fadeSpeed);
        currentAlpha = curve.Evaluate(interpolation);
        SetAlpha();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(playerTag))
        {
            targetAlpha = 1;
            rim.Play();
            audioManager.Play(sound, transform.position);
            emission.enabled = true;

        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            targetAlpha = 0;
            emission.enabled = false;
        }
    }

    private void SetAlpha()
    {
        foreach(SpriteRenderer sr in lights)
        {
            Color color = sr.color;
            color.a = currentAlpha;
            sr.color = color;
        }
    }
}
