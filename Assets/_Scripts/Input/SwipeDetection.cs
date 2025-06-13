using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;
using UnityFigmaBridge.Editor.FigmaApi;
using Color = UnityEngine.Color;

enum Direction
{
    UP, DOWN, LEFT, RIGHT
}

public class SwipeDetection : MonoBehaviour
{
    [SerializeField] private float _MaxTime;
    [SerializeField] private float _MinDistance;
    [SerializeField] private CinemachineCamera _topDownCamera;

    private TouchManager _touchManager;

    private Vector2 _startPosition;
    private Vector2 _endPosition;
    private float _startTime;
    private float _endTime;

    private void Awake()
    {
        _touchManager = TouchManager.Instance;
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
        DetectSwipe();
    }

    private void DetectSwipe()
    {
        float swipeTime = _endTime - _startTime;
        Vector2 swipeDirection = _endPosition - _startPosition;
        float swipeDistance = swipeDirection.magnitude;

        Debug.Log("Checking swipe...");
        Debug.Log("Distance: " + swipeDistance);
        Debug.Log("Time: " + swipeTime);
        if (swipeDistance >= _MinDistance && swipeTime <= _MaxTime)
        {
            Debug.Log("Swipe DETECTED.");
            Debug.Log("Startpos: " + _startPosition);
            Debug.Log("Endpos: " + _endPosition);
            Debug.DrawLine(_startPosition, _endPosition, Color.green, 5f);
            
            float verticalBias = Vector2.Dot( Vector2.up, swipeDirection.normalized);
            float horizontalBias = Vector2.Dot( Vector2.right, swipeDirection.normalized);
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
            
            switchCamera(coreDirection);
        }
    }

    private void switchCamera(Direction swipeDirection)
    {
        switch (swipeDirection)
        {
            case Direction.UP:
            {
                Debug.Log("Swipe UP");
                _topDownCamera.GetComponent<CinemachineCamera>().Priority = 2; 
                break;
            }
            case Direction.DOWN:
            {
                Debug.Log("Swipe DOWN");
                if (Camera.main != null)
                {
                    Camera.main.GetComponent<CinemachineCamera>().Priority = 2;
                }
                else
                {
                    Debug.LogError("[ERROR]: No main camera found");
                }
                break;
            }
            case Direction.LEFT: Debug.Log("Swipe LEFT"); break;
            case Direction.RIGHT: Debug.Log("Swipe RIGHT"); break;
        }
    }
}