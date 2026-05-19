using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thanks : MonoBehaviour
{

    void Start()
    {
        Invoke("ReturnToMenu", 8f);
    }

    private void ReturnToMenu()
    {
        ManagerLocator.Get<ISceneLoadManager>().DelayLoadScene("Menu");
    }
}
