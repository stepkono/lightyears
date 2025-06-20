using UnityEngine;

public class ButtonsManager : MonoBehaviour
{
    [SerializeField] private GameObject controller;
    
    public void CreateNewHobby()
    {
        HobbyData hobbyData = new HobbyData();
        hobbyData.SetName("HobbyTest");
        hobbyData.SetDaysInterval(3);
        
        if (controller == null) { Debug.LogError("No controller assigned!"); return; }
        controller.GetComponent<Controller>().SaveNewHobby(hobbyData);
    }
}
