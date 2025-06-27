using _Scripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LaunchSlider : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    [SerializeField, Range(0f, 1f)] private float acceptThreshold = 0.9f;
    [SerializeField] private float returnSpeed = 3f;
    [SerializeField] private UnityEvent onAccepted;
    
    private Slider _slider;
    private bool _isDragging = false;
    private bool _accepted = false;

    void Awake()
    {
        _slider = GetComponent<Slider>();
        _slider.value = 0f;
        _slider.onValueChanged.AddListener(OnSliderChanged);
    }

    void Update()
    {
        if (!_isDragging && !_accepted)
        {
            _slider.value = Mathf.MoveTowards(_slider.value, 0f, returnSpeed * Time.deltaTime);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
        _isDragging = true;
    } 

    public void OnEndDrag(PointerEventData eventData3)
    {
        _isDragging = false;

        if (_slider.value >= acceptThreshold)
        {
            _accepted = true;
            _slider.value = 1f;
            
            // Inform ViewsManager to close this view
            AppEvents.RaiseHobbyLaunched();
            
            Debug.Log("[DEBUG]: LaunchSlider: HOBBY LAUNCHED.");
            _slider.value = 0f; 
        }
    }

    private void OnSliderChanged(float val)
    {
        if (val < acceptThreshold)
            _accepted = false;
    }
}
