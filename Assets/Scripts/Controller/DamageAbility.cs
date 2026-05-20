using System.Collections;
using UnityEngine;
using EventHandler.Respawn;
using Cinemachine;

public class DamageAbility : AbilityBase
{
    private float invincibilityDuration;

    private bool isInvincible;
    private float invincibilityTimer;

    private CinemachineImpulseSource impulse;

    private PlayerData playerData;
    private IEventManager eventManager;

    private SpriteRenderer knight;

    private readonly int damagedHash = Animator.StringToHash("Damaged");
    private readonly int deadHash = Animator.StringToHash("Dead");

    public bool IsAlive => playerData.HP > 0;

    public DamageAbility(PlayerController playerController, PlayerContext context) : base(playerController, context)
    {
        knight = playerController.GetComponent<SpriteRenderer>();
        impulse = playerController.GetComponent<CinemachineImpulseSource>();

        invincibilityDuration = context.playerConfig.invincibilityDuration;
        isInvincible = false;
        invincibilityTimer = 0f;

        playerData = ManagerLocator.Get<DataManager>().playerData;
        eventManager = ManagerLocator.Get<IEventManager>();

        context.onDamaged = OnDamage;
    }

    public override void OnUpdate()
    {
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0)
                isInvincible = false;
        }
    }

    public override void OnFixedUpdate()
    {

    }

    public void OnDamage(DamageInfo info)
    {
        if (!IsAlive || isInvincible) return;

        playerData.HP -= info.amount;

        isInvincible = true;
        invincibilityTimer = invincibilityDuration;

        animator.Play(damagedHash);
        playerController.StartCoroutine(InvincibilityFlash(invincibilityDuration));
        context.damagedEffect.Play();
        context.playerAudio.PlayOneShotDamaged();
        impulse.GenerateImpulse();

        if (playerData.HP <= 0)
        {
            OnDeath();
        }
        else
        {
            if (info.respawnFlag)
            {
                playerController.ControlDisable();
                eventManager.EventTrigger(ScreenToBlackEventHandler.Get(ReplaceEventHandler.Get()));
            }
            else
            {
                playerController.ControlDisable(0.3f);
            }
            context.addDamagedForce?.Invoke(context.playerConfig.hurtForce);
        }
    }

    private IEnumerator InvincibilityFlash(float duration)
    {
        Color white = Color.white;
        Color darkGray = new Color(64f / 255f, 64f / 255f, 64f / 255f, 1f);
        float durationPerFlash = duration / 6f;
        for (int i = 0; i < 3; i++)
        {
            float timer = 0;
            while (timer < durationPerFlash)
            {
                float progress = timer / durationPerFlash;
                float t = Mathf.SmoothStep(0, 1, progress);
                knight.color = Color.Lerp(white, darkGray, t);
                timer += Time.deltaTime;
                yield return null;
            }

            timer = 0;
            while (timer < durationPerFlash)
            {
                float progress = timer / durationPerFlash;
                float t = Mathf.SmoothStep(0, 1, progress);
                knight.color = Color.Lerp(darkGray, white, t);
                timer += Time.deltaTime;
                yield return null;
            }
        }

        knight.color = white;
    }

    public void OnDeath()
    {
        context.isDead = true;
        animator.Play(deadHash);
        rb.velocity = Vector2.zero;
        context.onDead?.Invoke();
    }
}
