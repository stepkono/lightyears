using System;
using _Scripts;
using TMPro;
using UnityEngine;
using Buffer = _Scripts.Buffer;

public class ButtonsManager : MonoBehaviour
{
    private Controller _controller;
    private HobbyManager _selectedHobby; 
    [SerializeField] ViewsManager viewsManager;

    private void Awake()
    {
        _controller = Controller.Instance;
    }

    public void CreateNewHobby()
    {
        _controller.CreateNewHobby();
    }

    public void addHours()
    {
        Debug.Log("[DEBUG]: CLICKED BUTTON hours.");
        _selectedHobby.InvestHours(1);
    }

    public void AcceptInvestedTime()
    {
        _selectedHobby.InvestHours(Buffer.investedSeconds);
        Buffer.investedSeconds = 0;
        
        viewsManager.CurrentActiveView
            .transform.Find("Mask/Slider/Background/Interactables/HoursInputField")
            .GetComponent<TMP_InputField>().text = "";
        viewsManager.CurrentActiveView
            .transform.Find("Mask/Slider/Background/Interactables/MinutesInputField")
            .GetComponent<TMP_InputField>().text = "";
        
        _controller.TerminateInvestHoursView(_selectedHobby);
    }

    public void OpenViewInvestHours()
    {
        viewsManager.ActivateInvestHoursView(_selectedHobby);
    }

    public void SetSelectedHobby(HobbyManager hobby)
    {
        _selectedHobby = hobby; 
    }

    public void RemoveSelectedHobby()
    {
        _selectedHobby = null; 
    }
    
    public void GetBackFromHobbyCreationView()
    {
        AppEvents.RaiseHobbyLaunched(false);
    }

    public void GetBackFromInvestHoursView()
    {
        _controller.TerminateInvestHoursView(_selectedHobby);
    }
}
