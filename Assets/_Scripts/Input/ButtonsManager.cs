using System;
using UnityEngine;

public class ButtonsManager : MonoBehaviour
{
    private Controller _controller;

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
}
