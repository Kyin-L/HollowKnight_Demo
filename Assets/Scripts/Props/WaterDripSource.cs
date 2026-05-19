using UnityEngine;

public class WaterDripSource : MonoBehaviour
{
    [SerializeField] private GameObject waterDripPrefab;
    [SerializeField] private Vector2 during = new Vector2(3f, 5f);
    private Animator animator;

    private float time;

    private IPoolManager poolManager;

    private readonly int dropHash = Animator.StringToHash("Drop");

    void Start()
    {
        poolManager = ManagerLocator.Get<IPoolManager>();

        animator = GetComponent<Animator>();
        time = Random.Range(during.x, during.y);
    }

    void Update()
    {
        time -= Time.deltaTime;
        if(time <= 0)
        {
            Drop();
            time = Random.Range(during.x, during.y);
        }
    }

    private void Drop()
    {
        animator.SetTrigger(dropHash);
    }

    public void SpawnWaterDrip()
    {
        GameObject waterDrip = poolManager.Get(waterDripPrefab);
        waterDrip.transform.position = transform.position;
    }
}
