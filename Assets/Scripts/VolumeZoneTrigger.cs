using UnityEngine;
using UnityEngine.Rendering;

public class VolumeZoneTrigger : MonoBehaviour
{
    public Volume volume;
    public float transitionSpeed = 2f;

    private float currentWeight = 0f;
    private float targetWeight = 0f;

    void Start()
    {
        if(volume == null)
            volume = GetComponentInChildren<Volume>();

        volume.weight = 0f;
        currentWeight = 0f;
        targetWeight = 0f;
    }

    void Update()
    {
        if (Mathf.Abs(currentWeight - targetWeight) > 0.01f)
        {
            currentWeight = Mathf.Lerp(currentWeight, targetWeight, Time.deltaTime * transitionSpeed);

            if (Mathf.Abs(currentWeight - targetWeight) < 0.01f)
                currentWeight = targetWeight;

            volume.weight = currentWeight;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            targetWeight = 1f;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            targetWeight = 0f;
    }
}