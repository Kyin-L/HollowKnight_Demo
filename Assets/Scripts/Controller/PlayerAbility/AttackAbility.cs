using System;
using System.Collections.Generic;
using UnityEngine;

public class AttackAbility : AbilityBase
{
    private AttackConfig attackData;
    private InputData inputData;

    private AttackColliders attackColliders;

    private float attackTimer = 0f;
    private float comboTimer = 0f;

    private List<Collider2D> colliders;

    private IPoolManager poolManager;

    private readonly int attack1Hash = Animator.StringToHash("Attack1");
    private readonly int attack2Hash = Animator.StringToHash("Attack2");
    private readonly int attackUpHash = Animator.StringToHash("AttackUp");
    private readonly int attackDownHash = Animator.StringToHash("AttackDown");

    private readonly int spikeLayer = LayerMask.NameToLayer("Spike");
    private readonly int enemyLayer = LayerMask.NameToLayer("Enemy");

    public class AttackColliders
    {
        public Collider2D forwardSlash;
        public Collider2D upSlash;
        public Collider2D downSlash;
    }

    public AttackAbility(PlayerController playerController, PlayerContext context) : base(playerController, context)
    {
        attackData = context.playerConfig.attackConfig;
        inputData = context.inputData;
        attackColliders = playerController.attackColliders;

        colliders = new List<Collider2D>();

        poolManager = ManagerLocator.Get<IPoolManager>();

        context.onAttack = OnAttack;
        context.canAttack = CanAttack;
        context.forwadSlash = ForwardSlash;
        context.upSlash = UpSlash;
        context.downSlash = DownSlash;
    }

    public override void OnUpdate()
    {
        attackTimer += Time.deltaTime;
        comboTimer += Time.deltaTime;
    }

    public override void OnFixedUpdate()
    {

    }

    public bool CanAttack()
    {
        return attackTimer > attackData.attackDuration;
    }

    public void OnAttack()
    {
        if (!context.inputData.enable)
            return;

        if (!CanAttack())
            return;

        attackTimer = 0;

        if (!context.isGround && inputData.VectorInput.y < 0)
        {
            comboTimer = attackData.comboWindow;
            animator.Play(attackDownHash);
            context.playerAudio.PlayOneShotDownSlash();
        }
        else if (inputData.VectorInput.y > 0)
        {
            comboTimer = attackData.comboWindow;
            animator.Play(attackUpHash);
            context.playerAudio.PlayOneShotUpSlash();
        }
        else
        {
            if (comboTimer > attackData.comboWindow)
            {
                comboTimer = 0;
                animator.Play(attack1Hash);
                context.playerAudio.PlayOneShotForwardSlash1();
            }
            else
            {
                comboTimer = attackData.comboWindow;
                animator.Play(attack2Hash);
                context.playerAudio.PlayOneShotForwardSlash2();
            }
        }
    }

    private void ForwardSlash() => ApplySlash(attackColliders.forwardSlash, AddRecoilForce, Vector2.left * playerController.transform.localScale.x);
    private void UpSlash() => ApplySlash(attackColliders.upSlash, AddUpRecoilForce, Vector2.up);
    private void DownSlash() => ApplySlash(attackColliders.downSlash, AddDownRecoilForce, Vector2.down);

    private void ApplySlash(Collider2D slash, Action recoilAction = null, Vector2? force = null)
    {
        colliders.Clear();
        int count = Physics2D.OverlapCollider(slash, attackData.enemyContactFilter, colliders);

        if (count > 0)
        {
            recoilAction?.Invoke();

            Dictionary<Collider2D, Vector2> dic = GetRaycastDictionary(slash);

            if (force != null)
                force *= attackData.attackForce;

            foreach (Collider2D c in colliders)
            {
                ApplyDamage(slash, c, dic, force);
            }
        }
    }

    private void AddRecoilForce()
    {
        foreach (Collider2D c in colliders)
        {
            CustomTag customTag = c.GetComponentInParent<CustomTag>();
            if (customTag != null)
            {
                if (customTag.HasTag(RecoilTag.Forward))
                {
                    context.addRecoil?.Invoke(attackData.recoilForce);
                    break;
                }
            }
        }
    }

    private void AddUpRecoilForce()
    {
        foreach (Collider2D c in colliders)
        {
            CustomTag customTag = c.GetComponentInParent<CustomTag>();
            if (customTag != null)
            {
                if (customTag.HasTag(RecoilTag.Up))
                {
                    context.addUpRecoil?.Invoke();
                    break;
                }
            }
        }
    }

    private void AddDownRecoilForce()
    {
        foreach (Collider2D c in colliders)
        {
            CustomTag customTag = c.GetComponentInParent<CustomTag>();
            if (customTag != null)
            {
                if (customTag.HasTag(RecoilTag.Down))
                {
                    context.addDownRecoil?.Invoke(attackData.downRecoilVelocity);
                    break;
                }
            }
        }
    }

    private void ApplyDamage(Collider2D slash, Collider2D collider, Dictionary<Collider2D, Vector2> dic, Vector2? force)
    {
        if (collider.TryGetComponent(out IDamagable d))
        {
            Vector2 damagePoint;
            if (dic.ContainsKey(collider))
            {
                damagePoint = dic[collider];
            }
            else
            {
                damagePoint = slash.ClosestPoint(collider.bounds.center);
            }
            d.TakeDamage(new DamageInfo(attackData.damage, playerController.gameObject, damagePoint, force));
        }
    }

    private Vector2[] GetPoints(Collider2D slash)
    {
        Vector2 playerPos = slash.transform.position;

        Bounds bounds = slash.bounds;

        Vector2[] edgePoints = new Vector2[]
        {
        new Vector2(bounds.min.x, bounds.center.y), // 左中
        new Vector2(bounds.center.x, bounds.max.y), // 上中
        new Vector2(bounds.max.x, bounds.center.y), // 右中
        new Vector2(bounds.center.x, bounds.min.y)  // 下中
        };

        Vector2 nearest = edgePoints[0];
        Vector2 farthest = edgePoints[0];
        float minDistance = Vector2.Distance(playerPos, farthest);
        float maxDistance = Vector2.Distance(playerPos, farthest);

        foreach (Vector2 point in edgePoints)
        {
            float distance = Vector2.Distance(playerPos, point);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                farthest = point;
            }
            else if (distance < minDistance)
            {
                minDistance = distance;
                nearest = point;
            }
        }
        return new Vector2[2] { nearest, farthest - nearest };
    }

    private Dictionary<Collider2D, Vector2> GetRaycastDictionary(Collider2D slash)
    {
        Vector2[] points = GetPoints(slash);
        RaycastHit2D[] raycastHits = Physics2D.RaycastAll(points[0], points[1], points[1].magnitude, attackData.enemyContactFilter.layerMask);

        Dictionary<Collider2D, Vector2> hitPointMap = new Dictionary<Collider2D, Vector2>();
        foreach (RaycastHit2D hit in raycastHits)
        {
            if (hit.collider != null && !hitPointMap.ContainsKey(hit.collider))
            {
                hitPointMap[hit.collider] = hit.point;
            }
        }

        return hitPointMap;
    }
}