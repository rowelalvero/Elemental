using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class CameraController : Singleton<CameraController>
{
    private CinemachineCamera cinemachineVirtualCamera;

    public void Start()
    {
        SetPlayerCameraFollow();
    }

    public void SetPlayerCameraFollow()
    {
        cinemachineVirtualCamera = Object.FindFirstObjectByType<CinemachineCamera>();
        cinemachineVirtualCamera.Follow = PlayerController.Instance.transform;
    }
}
