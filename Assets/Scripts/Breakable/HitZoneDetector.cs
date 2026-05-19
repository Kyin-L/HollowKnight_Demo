using UnityEngine;

public class HitZoneDetector : MonoBehaviour
{
    [SerializeField] private Collider2D hitZone;

    private readonly string playerTag = "Player";

    private void Start()
    {
        hitZone.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(playerTag))
            hitZone.enabled = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
            hitZone.enabled = false;
    }
}
