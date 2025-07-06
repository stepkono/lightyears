using System;
using System.Collections;
using _Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TimerSlider : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    [SerializeField, Range(0f, 1f)] private float acceptThreshold = 0.9f;
    [SerializeField] private float returnSpeed = 3f;
    [SerializeField] private UnityEvent onAccepted;
    [SerializeField] private GameObject viewParent;
    [SerializeField] private float easingTime = 0.2f;
    [SerializeField] private Controller controller;

    private HobbyManager _hobby; 
    private string _vertexColorHex = "#8689E3";
    private Color _vertexColor;
    
    // State management
    private enum TimerState
    {
        Idle,           // Initial state, ready for input
        Dragging,       // User is dragging the slider
        TimerRunning,   // Timer is active and counting
        AnimationPlaying // End animation sequence is playing
    }
    
    private TimerState _currentState = TimerState.Idle;
    private bool _inputBlocked = false;
    private bool _timerIsRunning = false;
    
    // UI Components
    private Slider _slider;
    private Transform _timerText;
    private Transform _inputTimeUI;
    private Transform _dynamicTitle;
    
    // Timer text components
    private TMP_Text _hoursText;
    private TMP_Text _hCap;
    private TMP_Text _secondsText;
    private TMP_Text _minCap;
    private TMP_Text _minutesText;
    private TMP_Text _sCap;
    
    // Canvas groups for fading
    private CanvasGroup _timerTextGroup;
    private CanvasGroup _inputTimeUIGroup;
    private CanvasGroup _dynamicTitleGroup;
    
    // Timer variables
    private float _elapsedTime = 0f;
    private Vector3 _timerTextStartPosition;
    
    // Animation variables
    private float _sliderVelocity = 0f;
    private Coroutine _animationCoroutine;

    void Awake()
    {
        if (!ColorUtility.TryParseHtmlString(_vertexColorHex, out _vertexColor))
        {
            Debug.LogWarning("Failed to parse vertex color hex. Using white.");
        }

        InitializeComponents();
        SetupCanvasGroups();
        ResetToInitialState();
    }

    public void SetHobby(HobbyManager hobby)
    {
        _hobby = hobby;
    }
    
    private void InitializeComponents()
    {
        _slider = GetComponent<Slider>();
        _slider.value = 0f;
        _slider.onValueChanged.AddListener(OnSliderChanged);

        _timerText = viewParent.transform.Find("Timer");
        _inputTimeUI = viewParent.transform.Find("BlockInvestTime");
        _dynamicTitle = viewParent.transform.Find("DynamicTitle");

        _hoursText = _timerText.transform.Find("Hours").GetComponent<TMP_Text>();
        _minutesText = _timerText.transform.Find("Minutes").GetComponent<TMP_Text>();
        _secondsText = _timerText.transform.Find("Seconds").GetComponent<TMP_Text>();
        _hCap = _timerText.transform.Find("HCap").GetComponent<TMP_Text>();
        _minCap = _timerText.transform.Find("MCap").GetComponent<TMP_Text>();
        _sCap = _timerText.transform.Find("SCap").GetComponent<TMP_Text>();
        
        _timerTextStartPosition = _timerText.localPosition;
    }
    
    private void SetupCanvasGroups()
    {
        _timerTextGroup = _timerText.GetComponent<CanvasGroup>();
        if (_timerTextGroup == null)
            _timerTextGroup = _timerText.gameObject.AddComponent<CanvasGroup>();
            
        _inputTimeUIGroup = _inputTimeUI.GetComponent<CanvasGroup>();
        if (_inputTimeUIGroup == null)
            _inputTimeUIGroup = _inputTimeUI.gameObject.AddComponent<CanvasGroup>();
            
        _dynamicTitleGroup = _dynamicTitle.GetComponent<CanvasGroup>();
        if (_dynamicTitleGroup == null)
            _dynamicTitleGroup = _dynamicTitle.gameObject.AddComponent<CanvasGroup>();
    }
    
    private void ResetToInitialState()
    {
        _currentState = TimerState.Idle;
        _inputBlocked = false;
        
        // Reset timer
        _elapsedTime = 0f;
        UpdateTimerDisplay();
        
        // Reset positions and colors
        _timerText.localPosition = _timerTextStartPosition;
        SetTimerTextColor(Color.black);
        
        // Reset UI visibility
        _timerText.gameObject.SetActive(false);
        _inputTimeUI.gameObject.SetActive(true);
        _dynamicTitle.GetComponent<TMP_Text>().text = "Zeit eintragen";
        
        // Reset alpha values
        _timerTextGroup.alpha = 0f;
        _inputTimeUIGroup.alpha = 1f;
        _dynamicTitleGroup.alpha = 1f;
        
        // Reset slider
        _slider.value = 0f;
    }

    void Update()
    {
        HandleSliderMovement();
        HandleTimerUpdate();
        CheckForTimerEnd();
    }
    
    private void HandleSliderMovement()
    {
        if (_slider.value < 0.3)
        {
            _slider.value = Mathf.SmoothDamp(_slider.value, 0f, ref _sliderVelocity, easingTime);
        }
        
        switch (_currentState)
        {
            case TimerState.Idle:
                if (_currentState != TimerState.Dragging)
                {
                    _slider.value = Mathf.SmoothDamp(_slider.value, 0f, ref _sliderVelocity, easingTime);
                }
                break;
                
            case TimerState.TimerRunning:
                if (_currentState != TimerState.Dragging)
                {
                    _slider.value = Mathf.SmoothDamp(_slider.value, 0.6f, ref _sliderVelocity, easingTime);
                }
                break;
        }
    }
    
    private void HandleTimerUpdate()
    {
        if (_timerIsRunning)
        {
            _elapsedTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }
    
    private void UpdateTimerDisplay()
    {
        int hours = (int)(_elapsedTime / 3600);
        int minutes = (int)(_elapsedTime / 60) % 60;
        int seconds = (int)(_elapsedTime % 60);

        _hoursText.text = hours.ToString("00");
        _minutesText.text = minutes.ToString("00");
        _secondsText.text = seconds.ToString("00");
    }
    
    private void CheckForTimerEnd()
    {
        if (_timerIsRunning && _slider.value < 0.3f)
        {
            Debug.Log("STOPPING THE TIMER.");
            _timerIsRunning = false;
            
            _hobby.InvestHours(_elapsedTime);
            
            StartTimerEndSequence();
        }
    }
    
    private void StartTimerEndSequence()
    {
        if (_currentState == TimerState.AnimationPlaying) return;
        
        _currentState = TimerState.AnimationPlaying;
        _inputBlocked = true;
        
        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
        }
        
        _animationCoroutine = StartCoroutine(PlayTimerEndAnimation());
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_inputBlocked) return;
        
        _currentState = TimerState.Dragging;
        _timerText.gameObject.SetActive(true);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_inputBlocked) return;
        
        if (_slider.value >= acceptThreshold)
        {
            StartTimer();
        }
        else
        {
            _currentState = TimerState.Idle;
        }
    }
    
    private void StartTimer()
    {
        _currentState = TimerState.TimerRunning;
        _timerIsRunning = true;
        _inputTimeUI.gameObject.SetActive(false);
        _elapsedTime = 0f;
        
        StartCoroutine(FadeTextWithCallback(_dynamicTitleGroup, "Timer läuft...", 0.3f, null));
        Debug.Log("[DEBUG]: Timer has started.");
    }

    private void OnSliderChanged(float val)
    {
        if (_inputBlocked) return;
        
        // Only update alpha when in Idle or Dragging state
        if (_currentState == TimerState.Idle || _currentState == TimerState.Dragging)
        {
            _timerTextGroup.alpha = val;
            _inputTimeUIGroup.alpha = Mathf.Abs(val - 1f);
        }
    }

    private IEnumerator PlayTimerEndAnimation()
    {
        // Step 1: Move timer text upwards and change color
        yield return StartCoroutine(AnimateTimerTextUpAndChangeColor(1f));
        
        // Step 2: Change dynamic title to "Timer beendet"
        yield return StartCoroutine(FadeTextWithCallback(_dynamicTitleGroup, "Timer beendet", 0.3f, null));
        
        // Step 3: Wait 3 seconds
        yield return new WaitForSeconds(3f);
        
        controller.TerminateInvestHoursView(_hobby);
        
        //---------------------------------------------------------------//
        
        // Step 4: Change dynamic title to "Zeit eintragen"
        //yield return StartCoroutine(FadeTextWithCallback(_dynamicTitleGroup, "Zeit eintragen", 0.3f, null));
        
        // Step 5: Fade out timer text
        //yield return StartCoroutine(FadeCanvasGroup(_timerTextGroup, 1f, 0f, 0.3f));
        
        // Step 6: Reset everything for next use
        //ResetAfterAnimation();
    }
    
    private IEnumerator AnimateTimerTextUpAndChangeColor(float duration)
    {
        float time = 0f;
        Vector3 startPos = _timerTextStartPosition;
        Vector3 targetPos = startPos + new Vector3(0f, 60f, 0f);
        Color fromColor = Color.black;
        Color toColor = _vertexColor;
        
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            
            // Interpolate position
            _timerText.localPosition = Vector3.Lerp(startPos, targetPos, t);
            
            // Interpolate color
            Color currentColor = Color.Lerp(fromColor, toColor, t);
            SetTimerTextColor(currentColor);
            
            yield return null;
        }
        
        // Ensure final state
        _timerText.localPosition = targetPos;
        SetTimerTextColor(toColor);
    }
    
    private IEnumerator FadeTextWithCallback(CanvasGroup canvasGroup, string newText, float duration, System.Action callback)
    {
        // Fade out
        yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 1f, 0f, duration));
        
        // Change text
        _dynamicTitle.GetComponent<TMP_Text>().text = newText;
        
        // Fade in
        yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 0f, 1f, duration));
        
        callback?.Invoke();
    }
    
    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float fromAlpha, float toAlpha, float duration)
    {
        float time = 0f;
        
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            canvasGroup.alpha = Mathf.Lerp(fromAlpha, toAlpha, t);
            yield return null;
        }
        
        canvasGroup.alpha = toAlpha;
    }
    
    private void SetTimerTextColor(Color color)
    {
        _hoursText.color = color;
        _minutesText.color = color;
        _secondsText.color = color;
        _hCap.color = color;
        _minCap.color = color;
        _sCap.color = color;
    }
    
    private void ResetAfterAnimation()
    {
        // Deactivate timer text, activate input time UI
        _timerText.gameObject.SetActive(false);
        _inputTimeUI.gameObject.SetActive(true);
        
        // Fade in input time UI
        StartCoroutine(FadeCanvasGroup(_inputTimeUIGroup, 0f, 1f, 0.3f));
        
        // Reset all state
        ResetToInitialState();
    }
}
