using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// 挂载在主相机上，自动查找 UI Camera 并添加到 Camera Stack
/// </summary>
public class AutoSetupUICamera : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = GetComponent<Camera>();
        FindAndAddUICamera();
    }

    void FindAndAddUICamera()
    {
        // 查找 UI Camera（通过名称或 Tag）
        GameObject uiCameraObj = GameObject.Find("UICamera");
        if (uiCameraObj == null)
        {
            uiCameraObj = GameObject.FindGameObjectWithTag("UICamera");
        }

        if (uiCameraObj != null)
        {
            Camera uiCamera = uiCameraObj.GetComponent<Camera>();
            if (uiCamera != null)
            {
                AddToStack(uiCamera);
            }
        }
        else
        {
            // 延迟重试，等待 UI Camera 创建
            Invoke(nameof(FindAndAddUICamera), 0.1f);
        }
    }

    void AddToStack(Camera uiCamera)
    {
        var cameraData = mainCamera.GetUniversalAdditionalCameraData();
        if (cameraData != null && !cameraData.cameraStack.Contains(uiCamera))
        {
            cameraData.cameraStack.Add(uiCamera);
        }
    }
}