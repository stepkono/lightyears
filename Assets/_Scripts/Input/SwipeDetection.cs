using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using Color = UnityEngine.Color;

enum Direction
{
    UP,
    DOWN,
    LEFT,
    RIGHT
}

public class SwipeDetection : MonoBehaviour
{
    [SerializeField] private float maxTapTime;
    [SerializeField] private float minLongPressTime;
    [SerializeField] private float maxSwipeTime;
    [SerializeField] private float minSwipeDistance;
    [SerializeField] private CinemachineCamera _topDownCamera;
    [SerializeField] private CinemachineCamera _mainSceneCamera;

    private TouchManager _touchManager;
    private ViewsManager _viewsManager;

    private Vector2 _startPosition;
    private Vector2 _endPosition;
    private float _startTime;
    private float _endTime;

    private Camera _mainCamera;
    private CinemachineBrain _cmBrain;
    private Transform _activeCameraPos;
    private CamerasManager _camerasManager;

    private void Awake()
    {
        _touchManager = TouchManager.Instance;
        _camerasManager = CamerasManager.GetInstance();
        _viewsManager = ViewsManager.Instance;

        if (Camera.main == null) return;
        _cmBrain = Camera.main.GetComponent<CinemachineBrain>();
        _mainCamera = Camera.main;
    }

    /*-------------SUB TO INPUT EVENTS--------------*/
    private void OnEnable()
    {
        _touchManager.OnStartTouch += SwipeStart;
        _touchManager.OnEndTouch += SwipeEnd;
    }

    private void OnDisable()
    {
        _touchManager.OnEndTouch -= SwipeEnd;
        _touchManager.OnStartTouch -= SwipeStart;
    }

    /*--------------ACTION EXECUTION---------------*/
    private void SwipeStart(Vector2 startPos, float startTime)
    {
        _startPosition = startPos;
        _startTime = startTime;
    }

    private void SwipeEnd(Vector2 endPos, float endTime)
    {
        _endPosition = endPos;
        _endTime = endTime;
        DetectAction();
    }

    private void DetectAction()
    {
        float actionTime = _endTime - _startTime;
        Vector2 swipeDirection = _endPosition - _startPosition;
        float swipeDistance = swipeDirection.magnitude;

        if (actionTime <= maxTapTime && swipeDistance < minSwipeDistance)
        {
            ExecuteTap();
        }
        else if (actionTime >= minLongPressTime && swipeDistance < minSwipeDistance)
        {
            ExecuteLongPress();
        }
        else if (swipeDistance >= minSwipeDistance && actionTime <= maxSwipeTime)
        {
            ExecuteSwipe();
        }
        else if (actionTime >= maxSwipeTime)
        {
            ExecuteDrag();
        }
    }

    private void ExecuteLongPress()
    {
        Debug.Log("[INFO]: Executing long press...");
    }

    private void ExecuteTap()
    {
        Debug.Log("[INFO]: Executing tap...");

        if (_mainCamera != null)
        {
            
            Ray ray = _mainCamera.ScreenPointToRay(_startPosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var planet = hit.collider.GetComponent<PlanetManager>();
                if (planet != null)
                {
                    Debug.Log("Planet hit.");
                    _viewsManager.ActivatePlanetDetailView(planet);
                }
            }
            else
            {
                Debug.Log("Free tap");
                
                if (_viewsManager.CurrentActiveView != null)
                {
                    //_viewsManager.DeactivateCurrentView(); 
                }
                //_camerasManager.SetCurrentCamera(_mainSceneCamera);
            }
        }
        else
        {
            Debug.Log("No main camera found");
        }
    }

    private void ExecuteDrag()
    {
        Debug.Log("[INFO]: Executing drag...");
    }

    private void ExecuteSwipe()
    {
        float swipeTime = _endTime - _startTime;
        Vector2 swipeDirection = _endPosition - _startPosition;
        float swipeDistance = swipeDirection.magnitude;

        Debug.Log("Checking swipe...");
        Debug.Log("Distance: " + swipeDistance);
        Debug.Log("Time: " + swipeTime);
        if (swipeDistance >= minSwipeDistance && swipeTime <= maxSwipeTime)
        {
            Debug.Log("Swipe DETECTED.");
            Debug.Log("Startpos: " + _startPosition);
            Debug.Log("Endpos: " + _endPosition);
            Debug.DrawLine(_startPosition, _endPosition, Color.green, 5f);

            float verticalBias = Vector2.Dot(Vector2.up, swipeDirection.normalized);
            float horizontalBias = Vector2.Dot(Vector2.right, swipeDirection.normalized);
            Direction coreDirection;

            // Find out if the x or y axes are dominant 
            if (Math.Abs(verticalBias) > Math.Abs(horizontalBias))
            {
                coreDirection = verticalBias > 0 ? Direction.UP : Direction.DOWN;
            }
            else
            {
                coreDirection = horizontalBias > 0 ? Direction.RIGHT : Direction.LEFT;
            }

            SwitchCamera(coreDirection);
        }
    }

    private void SwitchCamera(Direction swipeDirection)
    {
        switch (swipeDirection)
        {
            case Direction.UP:
            {
                Debug.Log("Swipe UP");
                
                if (_viewsManager.CurrentActiveView != null)
                {
                    _viewsManager.DeactivateCurrentView(); 
                }
                _camerasManager.SetCurrentCamera(_mainSceneCamera);
                //_mainSceneCamera.GetComponent<CinemachineCamera>().Priority = 0;
                //_topDownCamera.GetComponent<CinemachineCamera>().Priority = 1;
                break;
            }
            case Direction.DOWN:
            {
                Debug.Log("Swipe DOWN");
                
                if (_viewsManager.CurrentActiveView != null)
                {
                    _viewsManager.DeactivateCurrentView(); 
                }
                _camerasManager.SetCurrentCamera(_topDownCamera);
                //_topDownCamera.GetComponent<CinemachineCamera>().Priority = 0;
                //_mainSceneCamera.GetComponent<CinemachineCamera>().Priority = 1;
                break;
            }
            case Direction.LEFT: Debug.Log("Swipe LEFT"); break;
            case Direction.RIGHT: Debug.Log("Swipe RIGHT"); break;
        }
    }
}