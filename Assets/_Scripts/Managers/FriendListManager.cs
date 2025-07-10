using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendListManager : MonoBehaviour
{
    [SerializeField] private Controller controller;
    [SerializeField] private GameObject viewHobbyCreation;
    [SerializeField] private ViewsManager viewsManager;
    
    private List<string> _friends = new List<string>();
    private bool _fusionHobby = false;
    private ShareButtonManager _shareButtonManager;
    private FriendListWindowInput _friendListWindowInput;

    private void Start()
    {
        _shareButtonManager = viewHobbyCreation.transform.Find("ShareButtonContainer").GetComponent<ShareButtonManager>();
        _friendListWindowInput = viewHobbyCreation.transform.Find("ViewFriendsList").GetComponent<FriendListWindowInput>();
    }

    public void AddFriend(string friendName)
    {
        _friends.Add(friendName);
        Debug.Log("Friend added: " + friendName);
    }

    public void RemoveFriend(string friendName)
    {
        _friends.Remove(friendName);
        Debug.Log("Friend removed: " + friendName);
    }
    
    public void SaveFriends()
    {
        controller.MakeFusionHobby(_friends);
    }

    public void RemoveFusionHobby()
    {
        controller.RemoveFusionHobby();
    }
    
    public int FriendCount()
    {
        return _friends.Count;
    }

    public void ResetVisualSection()
    {
        _shareButtonManager.GradientOff();
        _friendListWindowInput.RemoveSelection();
    }

    public void ToggleFusionHobby()
    {
        Debug.Log("ToggleFusionHobby: " + _fusionHobby);
        if (!_fusionHobby)
        {
            // If friends already selected -> save selection to hobby data
            if (_friends.Count > 0)
            {
                SaveFriends();
            }
            viewsManager.OpenFriendsList();
            // Activate button gradient
            _shareButtonManager.GradientOn();
            _fusionHobby = true;
        }
        else
        {
            _fusionHobby = false;
            // Remove friends selection from hobby data
            RemoveFusionHobby();
            ResetVisualSection();
            _shareButtonManager.GradientOff();
        }
    }
}