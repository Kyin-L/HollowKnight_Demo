using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Breakable : Hitable
{
    [SerializeField] protected BreakableConfig breakableConfig => hitableConfig as BreakableConfig;

    protected bool isBreak = false;
    protected Collider2D m_collider;
    protected Health health;

    protected override void Start()
    {
        base.Start();
        m_collider = GetComponent<Collider2D>();
        health = new Health(breakableConfig.hp, OnDamage, OnBreak);
    }

    protected virtual void OnEnable()
    {
        health?.Reset();
        isBreak = false;
    }

    protected virtual void Reset()
    {
#if UNITY_EDITOR
        var animator = GetComponent<Animator>();

        if (animator.runtimeAnimatorController == null)
        {
            string path = "Assets/Resources/Animator/Props/Breakable/Breakable.controller";
            animator.runtimeAnimatorController = UnityEditor.AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(path);
        }
#endif
    }

    public override void TakeDamage(DamageInfo info)
    {
        if (isBreak)
            return;

        info.amount = 1;
        health.TakeDamage(info);
        SpawnEffect(info);
    }

    protected virtual void OnDamage(DamageInfo info)
    {
        int index = health.maxHp - health.hp - 1;
        BreakableConfig.Feedback feedback = breakableConfig.feedbacks[index];
        PlayAnimation(feedback);
        PlaySound(info, feedback);
        SpawnDrops(feedback);
        SpawnEffect(info, feedback);
    }

    protected virtual void OnBreak(DamageInfo info)
    {
        m_collider.enabled = false;
        isBreak = true;

        OnDamage(info);
    }

    protected virtual void PlayAnimation(BreakableConfig.Feedback feedback)
    {
        if (animator != null && !string.IsNullOrEmpty(feedback.animation)) 
            animator.SetTrigger(feedback.animation);
    }

    protected virtual void PlaySound(DamageInfo info, BreakableConfig.Feedback feedback)
    {
        if (feedback.sound == null)
            return;

        if (info.damagePoint != null)
            audioManager.Play(feedback.sound, info.damagePoint.HasValue ? info.damagePoint.Value : transform.position);
    }

    protected virtual void SpawnDrops(BreakableConfig.Feedback feedback)
    {
        if (feedback.dropItemsPrefabs.Length > 0)
        {
            foreach (var dropItem in feedback.dropItemsPrefabs)
            {
                int count = Random.Range(dropItem.countRange.x, dropItem.countRange.y + 1);

                for (int num = 0; num < count; num++)
                {
                    GameObject drop = poolManager.Get(dropItem.prefab);
                    drop.transform.position = transform.position;

                    Rigidbody2D[] rbs = drop.GetComponentsInChildren<Rigidbody2D>();
                    if (rbs.Length > 0)
                    {
                        foreach (Rigidbody2D rb in rbs)
                        {
                            float angle = Random.Range(-30f, 30f);
                            float force = Random.Range(dropItem.forceRange.x, dropItem.forceRange.y);
                            Vector2 forceVector = Quaternion.Euler(0, 0, angle) * Vector2.up * force;
                            rb.AddForce(forceVector, ForceMode2D.Force);
                        }
                    }
                }
            }
        }
    }

    protected virtual void SpawnEffect(DamageInfo info, BreakableConfig.Feedback feedback)
    {
        if (feedback.effectPrefabs.Length > 0)
        {
            bool isRight = IsRight(info.source);
            foreach (GameObject effectPrefab in feedback.effectPrefabs)
            {
                GameObject effect = poolManager.Get(effectPrefab);
                effect.transform.position = effectTransform.position;
                effect.transform.rotation = effectTransform.rotation;
                effect.transform.localScale = new Vector3(isRight ? 1 : -1, 1, 1);
                if (effect.TryGetComponent(out ParticleSystem particle))
                {
                    particle.Play();
                }
            }
        }
    }

    protected virtual void SpawnEffect(DamageInfo info)
    {
        if (breakableConfig.hitEffects.Count == 0 || info.damagePoint == null)
            return;

        foreach (var hitEffect in breakableConfig.hitEffects)
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
}
