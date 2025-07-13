using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShareButtonManager : MonoBehaviour
{
    [SerializeField] private ViewsManager viewsManager;
    [SerializeField] private FriendListManager friendListManager;
    [SerializeField] private GameObject gradient;
    
    private Transform _friendsContainer;

    private void Start()
    {
        _friendsContainer = transform.Find("FriendsContainer");
    }

    public void ToggleFusionHobby()
    {
        friendListManager.ToggleFusionHobby();
    }

    private void SetButtonTitle(string title)
    {
        GetComponentInChildren<TMP_Text>().text = title;
    }

    public void ActivateVisuals()
    {
        _friendsContainer.gameObject.SetActive(true);
        SetButtonTitle("Fusion");
        GradientOn();
    }

    public void DeactivateVisuals(bool hardReset = false)
    {
        SetButtonTitle("Teilen");
        GradientOff(hardReset);
        Transform avatarsContainer = _friendsContainer.Find("FriendsAvatars");
        
        foreach (Transform avatar in avatarsContainer)
        {
            avatar.gameObject.SetActive(false);
        }
        _friendsContainer.gameObject.SetActive(false);
    }
    
    public void SetFriendsAvatars(List<string> friends)
    {
        Transform friendsContainer = transform.Find("FriendsContainer");
        friendsContainer.gameObject.SetActive(true);
        Transform avatarsContainer = friendsContainer.Find("FriendsAvatars");
        
        foreach (Transform avatar in avatarsContainer)
        {
            if (friends.Contains(avatar.gameObject.name))
            {
                avatar.gameObject.SetActive(true);
            }
            else
            {
                avatar.gameObject.SetActive(false);
            }
        }
    }

    public void GradientOn()
    {
        StartCoroutine(ActivateGradient(0, 1));
    }
    
    public void GradientOff(bool hardReset = false)
    {
        if (!hardReset)
        {
            StartCoroutine(DeactivatGradient(1, 0));
        }

        gradient.GetComponent<CanvasGroup>().alpha = 0;
    }

    IEnumerator ActivateGradient(float from, float to)
    {
        gradient.SetActive(true);
        CanvasGroup canvas = gradient.GetComponent<CanvasGroup>();

        float elapsedTimeGradient = 0f;
        float duration = 0.7f;

        while (elapsedTimeGradient < duration)
        {
            float interpolatorGradient = elapsedTimeGradient / duration; // Current ratio between from and to 
            canvas.alpha = Mathf.Lerp(from, to, interpolatorGradient); // Update gradient alpha 

            elapsedTimeGradient += Time.deltaTime;

            yield return null;
        }
    }
    
    IEnumerator DeactivatGradient(float from, float to)
    {
        CanvasGroup canvas = gradient.GetComponent<CanvasGroup>();

        float elapsedTimeGradient = 0f;
        float duration = 0.7f;

        while (elapsedTimeGradient < duration)
        {
            float interpolatorGradient = elapsedTimeGradient / duration; // Current ratio between from and to 
            canvas.alpha = Mathf.Lerp(from, to, interpolatorGradient); // Update gradient alpha 

            elapsedTimeGradient += Time.deltaTime;

            yield return null;
        }
        gradient.SetActive(false);
    }
}
