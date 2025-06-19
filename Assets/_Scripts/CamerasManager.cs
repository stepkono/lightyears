using System;
using Unity.Cinemachine;
using UnityEngine;

[DefaultExecutionOrder(-2)] // So that this script runs before all the other ones
public class CamerasManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CinemachineCamera mainViewCamera;

    private static CamerasManager INSTANCE; 
    
    private CinemachineCamera _currentCamera; 

    private void Awake()
    {
        if (INSTANCE == null)
        {
            INSTANCE = this; 
            DontDestroyOnLoad(gameObject);
            
            if (mainCamera == null)
            {
                Debug.LogError("[ERROR]: Main camera is not assigned.");
                return;
            }
            mainCamera = Camera.main;
            _currentCamera = mainViewCamera;
            _currentCamera.Priority = 1; 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static CamerasManager GetInstance()
    {
        return INSTANCE;
    }

    public void SetCurrentCamera(CinemachineCamera newCamera)
    {
        _currentCamera.Priority = 0; 
        newCamera.Priority = 1;
        _currentCamera = newCamera;
    }

    public CinemachineCamera GetCurrentCamera()
    {
        return _currentCamera;
    }
}