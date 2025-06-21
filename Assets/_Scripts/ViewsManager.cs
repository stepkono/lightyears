using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

[DefaultExecutionOrder(-2)]
public class ViewsManager : MonoBehaviour
{
    public static ViewsManager Instance { get; private set; }
    
    /*References to cameras*/
    [SerializeField] private CinemachineCamera mainSceneCamera;
    private CamerasManager _camerasManager;
    
    /*References to UI-GameObjects*/ 
    // View -> Hobby Creation
    [SerializeField] private GameObject hobbyCreationView; 
    [SerializeField] private GameObject backgroundGradient;
    [SerializeField] private GameObject systemUserName;
    // View -> Planet Details
    [SerializeField] private GameObject viewPlanetDetails;
    
    public GameObject CurrentActiveView { get; private set; }

    #region Events

    public delegate void HobbyCreationViewActive(bool active);

    public event HobbyCreationViewActive OnHobbyCreationViewActivation ; 
    
    #endregion

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(transform.root.gameObject);
            
            _camerasManager = CamerasManager.GetInstance();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ActivatePlanetDetailView(PlanetManager planet) 
    {
        planet.OnTouch();
        
        HobbyData hobbyData = planet.GetComponentInParent<HobbyManager>().GetHobbyData();
        var hobbyName = hobbyData.GetName();
        var hobbyNameTextField = viewPlanetDetails.transform.Find("HobbyName");
        hobbyNameTextField.GetComponent<TMP_Text>().text = hobbyName;
        
        viewPlanetDetails.SetActive(true);

        CurrentActiveView = viewPlanetDetails;
    }

    public void DeactivatePlanetDetailView()
    {
        viewPlanetDetails.SetActive(false);
        _camerasManager.SetCurrentCamera(mainSceneCamera);

        CurrentActiveView = null; 
    }

    public void DeactivateCurrentView()
    {
        if (CurrentActiveView == hobbyCreationView)
        {
            DeactivateHobbyCreationView();
        }
        if (CurrentActiveView == viewPlanetDetails)
        {
            DeactivatePlanetDetailView();
        }
    }

    public void ActivateHobbyCreationView()
    {
        CurrentActiveView = hobbyCreationView;
        
        OnHobbyCreationViewActivation?.Invoke(true);
        
        systemUserName.SetActive(false);
        
        // Slow fade-in anim for hobby creation background 
        StartCoroutine(ActivateBackgroundAfterDelay(0.5f, 0, 1));
    }

    public void DeactivateHobbyCreationView()
    {
        OnHobbyCreationViewActivation?.Invoke(false);
        
        backgroundGradient.SetActive(false);
        hobbyCreationView.SetActive(false);
        systemUserName.SetActive(true);
        
        CurrentActiveView = null; 
    } 
    
    IEnumerator ActivateBackgroundAfterDelay(float delay, float from, float to)
    {
        yield return new WaitForSeconds(delay);
        backgroundGradient.SetActive(true);
        hobbyCreationView.SetActive(true);
        
        float duration = 1.0f;
        float elapsedTime = 0;
        
        // Get Sprite to set alpha channel on dynamically
        SpriteRenderer spriteRenderer = backgroundGradient.GetComponent<SpriteRenderer>();
        while (elapsedTime < duration)
        {
            float interpolator = elapsedTime / duration; // Current ratio between from and to 
            Color imageColor = spriteRenderer.color; // Get current color 
            
            // Update alpha 
            imageColor.a = Mathf.Lerp(from, to, interpolator);
            spriteRenderer.color = imageColor;
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
