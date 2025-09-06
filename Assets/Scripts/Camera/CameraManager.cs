using System;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]private Camera mainCamera;
    
    
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