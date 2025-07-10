using System.Collections;
using UnityEngine;

public class ShareButtonManager : MonoBehaviour
{
    [SerializeField] private ViewsManager viewsManager;
    [SerializeField] private FriendListManager friendListManager;
    [SerializeField] private GameObject gradient;
    
    private bool _fusionHobby;

    private void Start()
    {
        _fusionHobby = false;
    }

    public void ToggleFusionHobby()
    {
        friendListManager.ToggleFusionHobby();
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
