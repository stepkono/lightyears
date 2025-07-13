using System.Collections;
using UnityEngine;

public class ButtonSelectFriend : MonoBehaviour
{
    [SerializeField] private FriendListManager friendListManager;
    [SerializeField] private string friendName;
    
    private FriendListWindowInput _friendListWindowInput;
    private CanvasGroup _selectedBackground; 
    private bool _selected; 

    private void Awake()
    {
        _selected = false;
        _selectedBackground = transform.Find("SelectedBackground").GetComponent<CanvasGroup>();
        _friendListWindowInput = friendListManager.GetViewHobbyCreation().transform.Find("ViewFriendsList/ButtonSaveFriends").GetComponent<FriendListWindowInput>();
        
        // Subscribe to the OnListClosed event
        FriendListWindowInput.OnListClosed += OnListClosed;
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from the event to prevent memory leaks
        FriendListWindowInput.OnListClosed -= OnListClosed;
    }
    
    private void OnListClosed()
    {
        // Set _selected to false when the list is closed
        _selected = false;
    }

    // Set to be called on button press
    public void SelectFriend()
    {
        if (!_selected)
        {
            _friendListWindowInput.AddFriend(friendName);
            StartCoroutine(ActivateSelectedVisual(0, 1));
            _selected = true;
        }
        else
        {
            _friendListWindowInput.RemoveFriend(friendName);
            StartCoroutine(DeactivateSelectedVisual(1, 0));
            _selected = false;
        }
    }

    public void ResetSelectedFriends()
    {
        _selected = false;
    }
    
    private IEnumerator ActivateSelectedVisual(float from, float to)
    {
        float elapsedTimeGradient = 0f;
        float duration = 0.3f;

        while (elapsedTimeGradient < duration)
        {
            float interpolatorGradient = elapsedTimeGradient / duration; // Current ratio between from and to 
            _selectedBackground.alpha = Mathf.Lerp(from, to, interpolatorGradient); // Update gradient alpha 

            elapsedTimeGradient += Time.deltaTime;

            yield return null;
        }
    }
    
    private IEnumerator DeactivateSelectedVisual(float from, float to)
    {
        float elapsedTimeGradient = 0f;
        float duration = 0.3f;

        while (elapsedTimeGradient < duration)
        {
            float interpolatorGradient = elapsedTimeGradient / duration; // Current ratio between from and to 
            _selectedBackground.alpha = Mathf.Lerp(from, to, interpolatorGradient); // Update gradient alpha 

            elapsedTimeGradient += Time.deltaTime;

            yield return null;
        }
    }
    
}
