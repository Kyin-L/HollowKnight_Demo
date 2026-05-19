using UnityEngine;

public class InitManager : MonoBehaviour
{
    public string sceneName;
    void Awake()
    {
        if (!FindObjectOfType<GameManager>())
        {
            GameObject gameManager = new GameObject();
            gameManager.name = "GameManager";
            gameManager.AddComponent<GameManager>();
        }
    }

    void Start()
    {
        ManagerLocator.Get<ISceneLoadManager>().DelayLoadScene(sceneName);
    }
}
