using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendListWindowInput : MonoBehaviour
{
    [SerializeField] private ViewsManager viewsManager;
    [SerializeField] private FriendListManager friendListManager;

    private List<String> _friends = new List<string>(); 
    private bool _saveAllowed = false;
    
    // Event and method to fire when CloseList() is called
    public static event System.Action OnListClosed;
    
    public static void TriggerListClosed()
    {
        OnListClosed?.Invoke();
    }

    public void CloseList()
    {
        viewsManager.CloseFriendsList();
        friendListManager.ToggleFusionHobby();
        
        // Fire the event to notify all subscribers
        OnListClosed?.Invoke();
    }

    public void SaveSelection()
    {
        if (_saveAllowed)
        {
            viewsManager.CloseFriendsList();
            friendListManager.SaveFriends(_friends);
        }
    }

    public void AddFriend(String friendName)
    {
        _friends.Add(friendName);
        EnableSave();
    }

    public void RemoveFriend(String friendName)
    {
        _friends.Remove(friendName);
        if (_friends.Count == 0)
        {
            DisableSave();
        }
    }
    
    public void EnableSave()
    {
        if (gameObject.name == "ButtonSaveFriends")
        {
            GetComponent<Button>().interactable = true;
            Image img = GetComponent<Image>();
            Color c = img.color;
            c.a = 1f;
            img.color = c;
            _saveAllowed = true;
        }
    }

    public void DisableSave()
    {
        if (gameObject.name == "ButtonSaveFriends")
        {
            GetComponent<Button>().interactable = false;
            Image img = GetComponent<Image>();
            Color c = img.color;
            c.a = 0.5f;
            img.color = c;
            _saveAllowed = false;
        }
    }

    /**
     * Called to reset selected friends form the list back to default
     */
    public void RemoveSelection()
    {
        _friends.Clear();
        DisableSave();
        if (gameObject.name == "ViewFriendsList")
        {
            List<Transform> _friendsButtons = new List<Transform>();
            Transform content = transform.Find("ScrollArea").Find("Content");
            
            foreach (Transform friendButton in content)
            {
                _friendsButtons.Add(friendButton);
            }
        
            foreach (Transform friendsButton in _friendsButtons)
            {
                friendsButton.Find("SelectedBackground").GetComponent<CanvasGroup>().alpha = 0;
                friendsButton.GetComponent<ButtonSelectFriend>().ResetSelectedFriends();
            }
        }
    }
}
