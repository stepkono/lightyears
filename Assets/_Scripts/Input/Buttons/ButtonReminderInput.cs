using System;
using System.Collections;
using _Scripts;
using UnityEngine;

public class ButtonReminderInput : MonoBehaviour
{
    private Transform _reminderButtonGradient; 
    private bool _reminderSelected;

    private void Awake()
    {
        _reminderButtonGradient = transform.Find("GradientReminderButton").transform;
        _reminderSelected = false;
    }

    private void OnEnable()
    {
        AppEvents.OnHobbyLaunched += DeactiveReminderButton;
    }

    private void OnDisable()
    {
        AppEvents.OnHobbyLaunched -= DeactiveReminderButton;
    }

    private void DeactiveReminderButton(bool hardReset)
    {
        _reminderSelected = false;
        _reminderButtonGradient.GetComponent<CanvasGroup>().alpha = 0;
    }

    // Set to be called on button click
    public void ToggleReminder()
    {
        if (!_reminderSelected)
        {
            _reminderSelected = true;
            StartCoroutine(ActivateGradient(0, 1));
        }
        else
        {
            _reminderSelected = false;
            StartCoroutine(DeactivateGradient(1, 0));
        }
    }
    
    private IEnumerator ActivateGradient(float from, float to)
    {
        CanvasGroup canvas = _reminderButtonGradient.GetComponent<CanvasGroup>();
        float elapsedTimeGradient = 0f;
        float duration = 0.7f;

        while (elapsedTimeGradient < duration)
        {
            float interpolatorGradient = elapsedTimeGradient / duration; // Current ratio between from and to 
            canvas.alpha = Mathf.Lerp(from, to, interpolatorGradient); // Update gradient alpha 

            elapsedTimeGradient += Time.deltaTime;

            yield return null;
        }
    }
    
    private IEnumerator DeactivateGradient(float from, float to)
    {
        CanvasGroup canvas = _reminderButtonGradient.GetComponent<CanvasGroup>();

        float elapsedTimeGradient = 0f;
        float duration = 0.7f;

        while (elapsedTimeGradient < duration)
        {
            float interpolatorGradient = elapsedTimeGradient / duration; // Current ratio between from and to 
            canvas.alpha = Mathf.Lerp(from, to, interpolatorGradient); // Update gradient alpha 

            elapsedTimeGradient += Time.deltaTime;

            yield return null;
        }
    }
}
