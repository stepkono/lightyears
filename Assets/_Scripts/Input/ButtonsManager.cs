using System;
using UnityEngine;

public class ButtonsManager : MonoBehaviour
{
    [SerializeField] private GameObject controller;
    private Controller _controller;

    private void Awake()
    {
        _controller = controller.GetComponent<Controller>();
    }

    public void CreateNewHobby()
    {
        _controller.CreateNewHobby();
    }

    public void SaveCurrentHobby()
    {
        _controller.SaveCurrentHobby();
    }
}
