using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using EventHandler.PlayerData;

public class HUDHealth : MonoBehaviour
{
    [SerializeField] private GameObject healthPrefab;

    private List<Animator> animators;

    private int maxHp;
    private int currentHp;

    public UnityAction onShowEnd;

    private IEventManager eventManager;

    private WaitForSeconds showDuring;

    private readonly int showHash = Animator.StringToHash("Health_Show");
    private readonly int hurtHash = Animator.StringToHash("Health_Hurt");
    private readonly int healingHash = Animator.StringToHash("Health_Healing");
    void Awake()
    {
        animators = new List<Animator>();
        showDuring = new WaitForSeconds(0.7f);
    }

    public void Initialized(int maxHp)
    {
        this.maxHp = maxHp;
        currentHp = maxHp;

        for (int num = 0; num < maxHp; num++)
        {
            GameObject health = Instantiate(healthPrefab, transform);
            animators.Add(health.GetComponent<Animator>());
        }
        eventManager = ManagerLocator.Get<IEventManager>();
        eventManager.AddListener<HpChangedEventHandler>(OnHpChanged);
    }

    void OnDestroy()
    {
        eventManager.RemoveListener<HpChangedEventHandler>(OnHpChanged);
    }

    private void OnHpChanged(HpChangedEventHandler handler)
    {
        int hp = handler.hp;
        if (hp < currentHp)
        {
            for (int num = hp; num < currentHp; num++)
            {
                animators[num].Play(hurtHash);
            }
        }
        else if(hp > currentHp)
        {
            for (int num = currentHp; num < hp; num++)
            {
                animators[num].Play(healingHash);
            }
        }

        currentHp = hp;
    }

    public void Show()
    {
        StartCoroutine(IEShow());
    }

    private IEnumerator IEShow()
    {
        for (int num = 0; num < maxHp; num++)
        {
            animators[num].Play(showHash);
            yield return showDuring;
        }

        onShowEnd?.Invoke();
    }
}
