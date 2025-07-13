using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    private FriendListWindowInput _friendListWindowInputSaveButton;

    private void Start()
    {
        _shareButtonManager = viewHobbyCreation.transform.Find("ShareButtonContainer").GetComponent<ShareButtonManager>();
        _friendListWindowInput = viewHobbyCreation.transform.Find("ViewFriendsList").GetComponent<FriendListWindowInput>();
        _friendListWindowInputSaveButton = viewHobbyCreation.transform.Find("ViewFriendsList/ButtonSaveFriends").GetComponent<FriendListWindowInput>();
    }

    public void AddFriend(string friendName)
    {
        _friends.Add(friendName);
        _friendListWindowInputSaveButton.EnableSave(); 
        Debug.Log("Friend added: " + friendName);
    }

    public void RemoveFriend(string friendName)
    {
        _friends.Remove(friendName);
        if (_friends.Count == 0)
        {
            _friendListWindowInputSaveButton.DisableSave(); 
        }
        _shareButtonManager.SetFriendsAvatars(_friends);
        Debug.Log("Friend removed: " + friendName);
    }
    
    public void SaveFriends(List<string> friends)
    {
        _friends = friends;
        controller.MakeFusionHobby(friends);
        _shareButtonManager.SetFriendsAvatars(friends);
    }

    public void RemoveFusionHobby()
    {
        
        controller.RemoveFusionHobby();
    }
    
    public int FriendCount()
    {
        return _friends.Count;
    }

    public void ResetVisualSection(bool hardReset = false)
    {
        _friends.Clear();
        _fusionHobby = false;
        _shareButtonManager.DeactivateVisuals(hardReset);
        _friendListWindowInput.RemoveSelection();
    }

    public void ToggleFusionHobby()
    {
        Debug.Log("ToggleFusionHobby: " + _fusionHobby);
        if (!_fusionHobby)
        {
            viewsManager.OpenFriendsList();
            // Activate button gradient
            _shareButtonManager.ActivateVisuals();
            _fusionHobby = true;
        }
        else
        {
            _fusionHobby = false;
            _friends.Clear();
            // Remove friends selection from hobby data
            RemoveFusionHobby();
            _friendListWindowInput.RemoveSelection();
            _shareButtonManager.DeactivateVisuals();
            
            // Fire the event to reset selection in FriendsListButton
            FriendListWindowInput.TriggerListClosed();
        }
    }

    public GameObject GetViewHobbyCreation()
    {
        return viewHobbyCreation;
    }
}