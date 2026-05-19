using UnityEngine;

public class AnimatorEventHide : MonoBehaviour
{
    private IPoolManager poolManager;

    void Start()
    {
        poolManager = ManagerLocator.Get<IPoolManager>();
    }

    public void Hide()
    {
        poolManager.Push(gameObject);
    }
}
