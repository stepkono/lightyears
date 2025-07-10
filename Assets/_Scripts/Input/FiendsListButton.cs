using System.Collections;
using UnityEngine;

public class FiendsListButton : MonoBehaviour
{
    [SerializeField] private FriendListManager friendListManager;
    [SerializeField] private string friendName;
    private CanvasGroup _selectedBackground; 
    private bool _selected; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _selected = false;
        _selectedBackground = transform.Find("SelectedBackground").GetComponent<CanvasGroup>();
    }

    public void SelectFriend()
    {
        if (!_selected)
        {
            friendListManager.AddFriend(friendName);
            StartCoroutine(ActivateSelectedVisual(0, 1));
            _selected = true;
        }
        else
        {
            friendListManager.RemoveFriend(friendName);
            StartCoroutine(DeactivateSelectedVisual(1, 0));
            _selected = false;
        }
    }
    
    IEnumerator ActivateSelectedVisual(float from, float to)
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
    
    IEnumerator DeactivateSelectedVisual(float from, float to)
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
