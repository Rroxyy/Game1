using System;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Base")]
    [SerializeField]private Camera mainCamera;

    public CameraController cameraController;

    public static CameraManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public Camera GetMainCamera()
    {
        return mainCamera;
    }
}