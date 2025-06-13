using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)] // So that this script runs before all the other ones
public class TouchManager : MonoBehaviour
{
    public static TouchManager Instance { get; private set; }
    
    private InputSystem _inputSystem;
    private Camera _mainCamera;
    private CinemachineBrain _cmBrain;
    private Transform _activeCameraPos; 
    
    #region Events
    public delegate void StartTouch(Vector2 position, float startedTime);
    public event StartTouch OnStartTouch;
    public delegate void EndTouch(Vector2 position, float endTime);
    public event EndTouch OnEndTouch;
    #endregion
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            _inputSystem = new InputSystem();
            
            _cmBrain = Camera.main.GetComponent<CinemachineBrain>();
            _mainCamera = Camera.main;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        _inputSystem.Enable();
    }

    private void OnDisable()
    {
        _inputSystem.Disable();
    }

    void Start()
    {
        _inputSystem.Touch.PrimaryContact.started += ctx => StartTouchPrimary(ctx);
        _inputSystem.Touch.PrimaryContact.canceled += ctx => EndTouchPrimary(ctx);
    }

    private void StartTouchPrimary(InputAction.CallbackContext ctx)
    {
        // Try to cast the interface wrapper of the current camera as base, which has transform 
        if (_cmBrain.ActiveVirtualCamera is CinemachineVirtualCameraBase activeCameraBase)
        {
            _activeCameraPos = activeCameraBase.transform;
        }
        else
        {
            Debug.LogError("[ERROR]: Casting of active virtual camera as a base failed.");
        }
        
        //Vector2 touchInWorldPos = Utils.ScreenToWorld(_inputSystem.Touch.PrimaryPosition.ReadValue<Vector2>(), _mainCamera);
        Vector2 touchInScreenPos = _inputSystem.Touch.PrimaryPosition.ReadValue<Vector2>();
        if (OnStartTouch != null) OnStartTouch(touchInScreenPos, (float)ctx.startTime);
    }

    private void EndTouchPrimary(InputAction.CallbackContext ctx)
    {
        //Vector2 touchInWorldPos = Utils.ScreenToWorld(_inputSystem.Touch.PrimaryPosition.ReadValue<Vector2>(), _mainCamera);
        Vector2 touchInScreenPos = _inputSystem.Touch.PrimaryPosition.ReadValue<Vector2>();
        if (OnEndTouch != null) OnEndTouch(touchInScreenPos, (float)ctx.time);
    }

    public Vector2 PrimaryTouchPosition()
    {
        return Utils.ScreenToWorld(_inputSystem.Touch.PrimaryPosition.ReadValue<Vector2>(), _mainCamera);
    }
     
}
