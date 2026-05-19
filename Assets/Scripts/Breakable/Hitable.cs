using UnityEngine;

public class Hitable : MonoBehaviour, IDamagable
{
    [SerializeField] protected HitableConfig hitableConfig;
    protected Animator animator;
    protected Transform effectTransform;

    protected IAudioManager audioManager;
    protected IPoolManager poolManager;


    protected virtual void Start()
    {
        audioManager = ManagerLocator.Get<IAudioManager>();
        poolManager = ManagerLocator.Get<IPoolManager>();
        TryGetComponent(out animator);

        Transform trans = transform.Find("Effect");
        effectTransform = trans != null ? trans : transform;
    }

    public virtual void TakeDamage(DamageInfo info)
    {
        OnHit(info);
    }

    private void OnHit(DamageInfo info)
    {
        PlayAnimation();
        PlaySound(info);
        SpawnEffect(info);
    }

    private void PlayAnimation() 
    {
        if (animator != null && !string.IsNullOrEmpty(hitableConfig.animation))
            animator.SetTrigger(hitableConfig.animation);
    }
    private void PlaySound(DamageInfo info) 
    {
        if (hitableConfig.sound == null)
            return;

        if (info.damagePoint != null)
            audioManager.Play(hitableConfig.sound, info.damagePoint.HasValue ? info.damagePoint.Value : transform.position);
    }
    private void SpawnEffect(DamageInfo info) 
    {
        if (hitableConfig.hitEffects.Count == 0 || !info.damagePoint.HasValue)
            return;

        foreach (var hitEffect in hitableConfig.hitEffects)
        {
            GameObject effect = poolManager.Get(hitEffect.hitEffectPrefab);
            effect.transform.position = info.damagePoint.Value;

            if (hitEffect.needLookAt)
            {
                Vector2 direction = (Vector2)info.source.transform.position - info.damagePoint.Value;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                effect.transform.rotation = Quaternion.Euler(0, 0, angle);
            }

            if (effect.TryGetComponent(out ParticleSystem particle))
            {
                particle.Play();
            }
        }
    }

    protected virtual bool IsRight(GameObject go)
    {
        return transform.position.x > go.transform.position.x;
    }
}