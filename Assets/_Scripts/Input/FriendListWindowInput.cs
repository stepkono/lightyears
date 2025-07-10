using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class FriendListWindowInput : MonoBehaviour
{
    [SerializeField] private ViewsManager viewsManager;
    [SerializeField] private FriendListManager friendListManager;

    public void CloseList()
    {
        viewsManager.CloseFriendsList();
        friendListManager.ToggleFusionHobby();
    }

    public void SaveSelection()
    {
        viewsManager.CloseFriendsList();
        friendListManager.SaveFriends();
    }

    public void RemoveSelection()
    {
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
            }
        }
    }
}
