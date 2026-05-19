using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public string sceneName;
    public int enterID;

    private ISceneLoadManager sceneLoadManager;

    private readonly string playerTag = "Player";

    void Start()
    {
        sceneLoadManager = ManagerLocator.Get<ISceneLoadManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(playerTag))
        {
            LevelData data = new LevelData(sceneName, enterID);
            sceneLoadManager.DelayLoadScene(data);
        }
    }
}
