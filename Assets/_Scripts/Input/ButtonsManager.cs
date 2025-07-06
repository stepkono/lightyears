using System;
using UnityEngine;

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

    public void SaveCurrentHobby()
    {
        _controller.SaveCurrentHobby();
    }

    public void addHours()
    {
        Debug.Log("[DEBUG]: CLICKED BUTTON hours.");
        _selectedHobby.InvestHours(1);
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
    
}
