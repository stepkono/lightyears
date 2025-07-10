using System;
using _Scripts;
using TMPro;
using UnityEngine;
using Buffer = _Scripts.Buffer;

public class InputFieldController : MonoBehaviour
{
    [SerializeField] private HobbyCreator hobbyCreator;
    private HobbyManager _hobby; 
    
    private Controller _controller; 

    private void Awake()
    {
        _controller = Controller.Instance; 
    }

    private void OnEnable()
    {
        AppEvents.OnHobbyLaunched += ResetInputField;
    }

    private void OnDisable()
    {
        AppEvents.OnHobbyLaunched -= ResetInputField;
    }
    
    public void SetHobbyName(string hobbyName)
    {
        hobbyCreator.SetName(hobbyName);
    }

    public void SetInterval(string interval)
    {
        hobbyCreator.ActivateInterval();
        int daysInterval = Int32.Parse(interval);   
        Debug.Log("INTERVAL SET: " + daysInterval);
        hobbyCreator.SetInterval(daysInterval);
    }

    public void SetHours(string hours)
    {
        if (int.TryParse(hours, out int parsedHours))
        {
            int seconds = parsedHours * 3600;
            Buffer.AddToBuffer(seconds);
        }
        else
        {
            Debug.LogWarning("Invalid hours input: " + hours);
        }
    }

    public void SetMinutes(string minutes)
    {
        if (int.TryParse(minutes, out int parsedMinutes))
        {
            int seconds = parsedMinutes * 60;
            Buffer.AddToBuffer(seconds);
        }
        else
        {
            Debug.LogWarning("Invalid hours input: " + minutes);
        }
    }

    public void SetHobby(HobbyManager hobby)
    {
        _hobby = hobby;
    }
    
    private void ResetInputField(bool launched)
    {
        gameObject.GetComponent<TMP_InputField>().text = "";
        Debug.Log("ResetInputField has been called");
    }
}
