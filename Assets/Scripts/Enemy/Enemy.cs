using UnityEngine;

public class Enemy : MonoBehaviour, IDamagable
{
    [SerializeField] protected EnemyConfig enemyConfig;
    [SerializeField] protected float deadUpSpeed = 10;

    protected bool isDead;
    protected bool deadFlag;
    protected Health health;
    protected BehaviorTree.EnemyContext context;

    protected Transform effectTransform;
    protected Rigidbody2D rb;
    protected Animator animator;
    protected new Collider2D collider;

    protected IPoolManager poolManager;
    protected IAudioManager audioManager;

    protected readonly string playerTag = "Player";

    protected virtual void Start()
    {
        isDead = false;
        health = new Health(enemyConfig.hp, OnDamage, OnDead);

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        context = new BehaviorTree.EnemyContext(this, rb, animator);
        enemyConfig.Initialize(context);

        poolManager = ManagerLocator.Get<IPoolManager>();
        audioManager = ManagerLocator.Get<IAudioManager>();

        Transform trans = transform.Find("Effect");
        effectTransform = trans != null ? trans : transform;
    }

    protected virtual void Update()
    {
        if (isDead)
            return;

        enemyConfig.OnUpdate(context);

        isDead = deadFlag;
    }

    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        if (isDead)
            return;

        if (collision.collider.CompareTag(playerTag))
        {
            if (collision.collider.TryGetComponent(out IDamagable player))
            {
                player.TakeDamage(new DamageInfo(enemyConfig.damage, gameObject));
            }
        }
    }

    public virtual void TakeDamage(DamageInfo info)
    {
        health.TakeDamage(info);
        SpawnEffect(info);
    }

    protected virtual void OnDamage(DamageInfo info)
    {
        EnemyConfig.HitFeedback feedback = enemyConfig.hitFeedback;
        ApplyPhysics(Vector2.zero, info.force); 
        PlayAnimation(feedback.animation);
        PlaySound(info, feedback.sound);
        SpawnEffect(info, feedback.effectPrefabs);
    }

    protected virtual void OnDead(DamageInfo info)
    {
        deadFlag = true;

        EnemyConfig.DeadFeedback feedback = enemyConfig.deadFeedback;
        ApplyPhysics(Vector2.up * deadUpSpeed, info.force);
        PlayAnimation(feedback.animation);
        PlaySound(info, feedback.sound);
        SpawnEffect(info, feedback.effectPrefabs);
        SpawnDrops(feedback.dropItems);
    }

    protected virtual void ApplyPhysics(Vector2 deadSpeed, Vector2? force)
    {
        context.rb.velocity = deadSpeed;

        if (force.HasValue)
            context.rb.AddForce(force.Value);
    }

    protected virtual void PlayAnimation(string animation)
    {
        if (animator != null && !string.IsNullOrEmpty(animation))
            animator.SetTrigger(animation);
    }

    protected virtual void PlaySound(DamageInfo info, AudioClip[] sound)
    {
        if (sound == null ||sound.Length == 0)
            return;

        int index = Random.Range(0, sound.Length);

        if (info.damagePoint != null)
            audioManager.Play(sound[index], info.damagePoint.HasValue ? info.damagePoint.Value : transform.position);
    }

    protected virtual void SpawnDrops(EnemyConfig.DropsItem[] dropItems)
    {
        if (dropItems.Length > 0)
        {
            foreach (var dropItem in dropItems)
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

    protected virtual void SpawnEffect(DamageInfo info, GameObject[] effectPrefabs)
    {
        if (effectPrefabs.Length > 0)
        {
            bool isRight = IsRight(info.source);
            foreach (GameObject effectPrefab in effectPrefabs)
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
        if (enemyConfig.hitEffects.Count == 0 || info.damagePoint == null)
            return;

        foreach (var hitEffect in enemyConfig.hitEffects)
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
