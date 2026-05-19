using System.Collections.Generic;
using UnityEngine;
using EventHandler.Respawn;

public partial class PlayerController : MonoBehaviour, IDamagable, IPickUp
{
    [SerializeField] private PlayerContext context;
        
    private InputBuffer inputBuffer;

    private List<IAbility> abilities = new List<IAbility>();

    public AttackAbility.AttackColliders attackColliders;

    private DataManager dataManager;
    private IEventManager eventManager;
    private PlayerData playerData;

    private Animator animator;

    public PlayerAudio playerAudio => context.playerAudio;

    private readonly int pickUpHash = Animator.StringToHash("PickUp");
    private readonly int respawnHash = Animator.StringToHash("Respawn");

    void Awake()
    {
        inputBuffer = new();
        inputBuffer.SetReceiver(this);
        attackColliders = new();
        attackColliders.forwardSlash = transform.Find("SlashTrigger/Forward").GetComponent<Collider2D>();
        attackColliders.upSlash = transform.Find("SlashTrigger/Up").GetComponent<Collider2D>();
        attackColliders.downSlash = transform.Find("SlashTrigger/Down").GetComponent<Collider2D>();

        dataManager = ManagerLocator.Get<DataManager>();
        eventManager = ManagerLocator.Get<IEventManager>();
        playerData = ManagerLocator.Get<DataManager>().playerData;

        animator = GetComponent<Animator>();

        RegisterAudio();
        RegisterAbilities();
    }

    void Start()
    {
        context.onDead = OnDeath;
    }

    void Update()
    {
        if (context.isDead)
            return;

        foreach (IAbility ability in abilities)
        {
            ability.OnUpdate();
        }
    }

    void FixedUpdate()
    {
        if (context.isDead)
            return;

        foreach (IAbility ability in abilities)
        {
            ability.OnFixedUpdate();
        }
    }

    private void RegisterAudio()
    {
        context.playerAudio = new PlayerAudio(context.playerConfig.audioConfig);
        context.playerAudio.audio = transform.Find("Audio").GetComponent<AudioSource>();
        context.playerAudio.move = transform.Find("Audio/Move").GetComponent<AudioSource>();
    }

    public void RegisterAbilities()
    {
        abilities.Clear();

        abilities.Add(new GroundDetector(this, context));
        abilities.Add(new DamageAbility(this, context));
        abilities.Add(new MoveAbility(this, context));
        abilities.Add(new JumpAbility(this, context));
        abilities.Add(new AttackAbility(this, context));
    }

    public void TakeDamage(DamageInfo info)
    {
        context.onDamaged?.Invoke(info);
    }

    private void OnDeath()
    {
        inputBuffer.SetReceiver(null);
        context.inputData.Reset();

        Invoke("DeathEvent", 3f);
    }

    private void DeathEvent()
    {
        eventManager.EventTrigger(ScreenToBlackEventHandler.Get(RespawnEventHandler.Get()));
    }

    public void AddGeo(int geo)
    {
        animator.Play(pickUpHash);
        dataManager.playerData.Geo += geo;
    }
}

public partial class PlayerController : IInputReceiver
{
    public void OnMove(Vector2 direction)
    {
        context.onMove?.Invoke(direction);
    }

    public void OnMoveCanceled()
    {
        context.onMoveCanceled?.Invoke();
    }

    public void OnJumpStart()
    {
        context.onJumpStart?.Invoke();
    }

    public void OnJumpCanceled()
    {
        context.onJumpCanceled?.Invoke();
    }

    public void OnAttack()
    {
        context.onAttack?.Invoke();
    }

    public bool CanAttack()
    {
        if (context.canAttack == null)
            return false;

        return context.canAttack();
    }

    public void ControlEnable()
    {
        context.inputData.enable = true;
    }

    public void ControlDisable(float time = 0)
    {
        if (context.inputData.enable == false)
            return;

        context.inputData.enable = false;
        if (time > 0)
            Invoke("ControlEnable", time);
    }
}

public partial class PlayerController
{
    //Animtor回调相关

    public void ForwardSlash()
    {
        context.forwadSlash?.Invoke();
    }

    public void UpSlash()
    {
        context.upSlash?.Invoke();
    }

    public void DownSlash()
    {
        context.downSlash?.Invoke();
    }

    public void PlayAshEffect()
    {
        context.ashEffect.Play();
    }

    public void PlayShadeEffect()
    {
        context.shadeEffect.Play();
    }

    public void Respawn()
    {
        inputBuffer.SetReceiver(this);
        playerData.Respawn();
        animator.SetTrigger(respawnHash);
        context.isDead = false;
    }
}