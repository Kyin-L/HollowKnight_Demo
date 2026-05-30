using UnityEngine;
using Cinemachine;

/// <summary>
/// 挂载在 CinemachineVirtualCamera 上，自动跟随 Tag 为 "Player" 的物体
/// </summary>
public class VirtualCameraAutoFollow : MonoBehaviour
{
    private CinemachineVirtualCamera vcam;

    void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        FindAndFollowPlayer();
    }

    void FindAndFollowPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            vcam.Follow = player.transform;
            vcam.LookAt = player.transform;
        }
        else
        {
            // 延迟重试，等待角色创建
            Invoke(nameof(FindAndFollowPlayer), 0.1f);
        }
    }
}