using UnityEngine;
using UnityEngine.Serialization;

public class SwipeDetection : MonoBehaviour
{
    [SerializeField] private float _MaxTime; 
    [SerializeField] private float _MinDistance;
    
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
        Debug.Log("hello");
        if (swipeDistance >= _MinDistance && swipeTime <= _MaxTime)
        {
            Debug.Log("Swipe detected.");
            Debug.DrawLine(_startPosition, _endPosition, Color.green, 5f);
        }
    }
}
