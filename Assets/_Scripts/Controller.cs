using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)] // So that this script runs before all the other ones
public class Controller : MonoBehaviour
{
    public static Controller Instance { get; private set; }
    
    [SerializeField] private GameObject systemManager;
    [SerializeField] private GameObject hobbyCreator;
    [SerializeField] private GameObject globalVolume;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CreateNewHobby()
    {
        Debug.Log("[INFO]: Clicked on Create New Hobby.");
        // Planet mesh 
        hobbyCreator.GetComponent<HobbyCreator>().CreateNewHobby();
        
        // Background blur 
        globalVolume.GetComponent<VolumeManager>().ActivateVolumeBlur();
        
        // Background gradient
        //Transform backgroundGradient = FindFirstObjectByType<Canvas>().transform.Find("PlanetCreationBackgroundGradient");
        GameObject backgroundGradient = GameObject.Find("PlanetCreationBackgroundGradient");
        if (backgroundGradient == null)
        {
            Debug.LogWarning("[INFO]: Failed to find background gradient.");
            return; 
        }
        StartCoroutine(ActivateBackgroundAfterDelay(0.5f, 0, 1, backgroundGradient));
    }

    IEnumerator ActivateBackgroundAfterDelay(float delay, float from, float to, GameObject backgroundGradient)
    {
        yield return new WaitForSeconds(delay);
        backgroundGradient.SetActive(true);
        
        float duration = 1.0f;
        float elapsedTime = 0;
        
        while (elapsedTime < duration)
        {
            float interpolator = elapsedTime / duration;
            Color imageColor = backgroundGradient.GetComponent<SpriteRenderer>().color;
            
            // Update alpha 
            imageColor.a = Mathf.Lerp(from, to, interpolator);
            backgroundGradient.GetComponent<SpriteRenderer>().color = imageColor;
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    
    public void SaveNewHobby(HobbyData hobbyData)
    {
        systemManager.GetComponent<SystemManager>().SaveNewHobby(hobbyData);
    }
}
