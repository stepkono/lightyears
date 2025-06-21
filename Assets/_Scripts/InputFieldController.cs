using System;
using TMPro;
using UnityEngine;

public class InputFieldController : MonoBehaviour
{
    [SerializeField] private HobbyCreator hobbyCreator;

    private Controller _controller; 

    private void Awake()
    {
        _controller = Controller.Instance; 
        Debug.Log("INSTANCE CONTROLLER ID: " + _controller.GetInstanceID());
        _controller.OnSaveHobby += ResetInputField;
    }

    private void OnEnable()
    {
        Debug.Log("ON ENABLE RUN");
    }

    private void OnDisable()
    {
        _controller.OnSaveHobby -= ResetInputField;
    }
    
    public void SetHobbyName(string hobbyName)
    {
        hobbyCreator.SetName(hobbyName);
    }

    public void SetInterval(string interval)
    {
        hobbyCreator.ActivateInterval(); // TODO: This is only for debugging 
        int daysInterval = Int32.Parse(interval);   
        hobbyCreator.SetInterval(daysInterval);
    }

    private void ResetInputField()
    {
        gameObject.GetComponent<TMP_InputField>().text = "";
        Debug.Log("ResetInputField has been called");
    }
}
