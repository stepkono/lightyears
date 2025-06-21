using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1)] // So that this script runs before all the other ones
public class TouchManager : MonoBehaviour
{
    public static TouchManager Instance { get; private set; }
    [SerializeField] private CinemachineCamera mainSceneCamera;
    
    #region Events
    public delegate void StartTouch(Vector2 position, float startedTime);
    public event StartTouch OnStartTouch;
    public delegate void EndTouch(Vector2 position, float endTime);
    public event EndTouch OnEndTouch;
    #endregion
    
    private InputSystem _inputSystem;
    private ViewsManager _viewsManager;
    
    private bool _firstTouch = true;
    private bool _allowTouch = true;
        
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(transform.root.gameObject);
            
            /*SETUP*/
            _inputSystem = new InputSystem();
            _viewsManager = ViewsManager.Instance;
            
            // Disable gestures if hobbyCreationView is currently active
            _viewsManager.OnHobbyCreationViewActivation += viewActive => _allowTouch = !viewActive; 
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
        _inputSystem.UI.InitialTouchSetup.started += ctx => StartTouchPrimary(ctx);
        _inputSystem.UI.InitialTouchSetup.canceled += ctx => EndTouchPrimary(ctx);
        //_inputSystem.Touch.PrimaryContact.started += ctx => StartTouchPrimary(ctx);
        //_inputSystem.Touch.PrimaryContact.canceled += ctx => EndTouchPrimary(ctx);
    }

    private void StartTouchPrimary(InputAction.CallbackContext ctx)
    {
        if (!_allowTouch) return; // Deactivate gesture if specific view is set 
        
        //Vector2 touchInWorldPos = Utils.ScreenToWorld(_inputSystem.Touch.PrimaryPosition.ReadValue<Vector2>(), _mainCamera);
        Vector2 touchInScreenPos;
        if (_firstTouch)
        {
            touchInScreenPos = ctx.ReadValue<Vector2>();
            _firstTouch = false;
        }
        else
        {
            touchInScreenPos = _inputSystem.UI.PrimaryPosition.ReadValue<Vector2>();
        }
        Debug.Log("[INFO]: START TOUCH: " + touchInScreenPos.x + " - " + touchInScreenPos.y);
        if (OnStartTouch != null) OnStartTouch(touchInScreenPos, (float)ctx.startTime);
    }

    private void EndTouchPrimary(InputAction.CallbackContext ctx)
    {
        if (!_allowTouch) return; // Deactivate gesture if specific view is set
        
        //Vector2 touchInWorldPos = Utils.ScreenToWorld(_inputSystem.Touch.PrimaryPosition.ReadValue<Vector2>(), _mainCamera);
        Vector2 touchInScreenPos = _inputSystem.UI.PrimaryPosition.ReadValue<Vector2>();
        Debug.Log("[INFO]: END TOUCH: " + touchInScreenPos.x + " - " + touchInScreenPos.y);
        if (OnEndTouch != null) OnEndTouch(touchInScreenPos, (float)ctx.time);
    }
    
    

    /*public Vector2 PrimaryTouchPosition()
    {
        //return Utils.ScreenToWorld(_inputSystem.UI.PrimaryPosition.ReadValue<Vector2>(), _mainCamera);
    }*/
     
} 
