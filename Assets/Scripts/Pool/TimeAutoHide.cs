using UnityEngine;

public class TimeAutoHide : MonoBehaviour
{
    [SerializeField] private float timeToHide = 10f;

    private IPoolManager poolManager;

    void Start()
    {
        poolManager = ManagerLocator.Get<IPoolManager>();
        Invoke("Hide", timeToHide);
    }

    private void Hide()
    {
        poolManager.Push(gameObject);
    }
}
