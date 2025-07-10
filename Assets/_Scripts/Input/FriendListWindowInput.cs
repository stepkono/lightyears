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
        Debug.Log("REMOVED SELECTION FROM BUTTONS");
    }
}
