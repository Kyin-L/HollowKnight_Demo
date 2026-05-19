using UnityEngine;

public class Spike : MonoBehaviour
{
    private readonly string playerTag = "Player";
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D collider = collision.collider;
        if (collider.CompareTag(playerTag))
        {
            if (collider.TryGetComponent(out IDamagable damagable))
            {
                DamageInfo damageInfo = new DamageInfo(1, gameObject, respawnFlag: true);
                damagable.TakeDamage(damageInfo);
            }
        }
    }
}
