using System.Collections;
using _Scripts;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-2)]
public class ViewsManager : MonoBehaviour
{
    public static ViewsManager Instance { get; private set; }

    /*References to cameras*/
    [SerializeField] private CinemachineCamera mainSceneCamera;
    private CamerasManager _camerasManager;

    /*References to UI-GameObjects*/
    // View -> Main 
    [SerializeField] private GameObject viewMain;
    [SerializeField] private GameObject gradientPlanet;
    [SerializeField] private GameObject gradientButton1;
    [SerializeField] private GameObject gradientButton2;
    
    // View -> Hobby Creation
    [SerializeField] private GameObject viewHobbyCreation;
    [SerializeField] private GameObject hobbyCreationView;
    [SerializeField] private GameObject backgroundGradient;

    [SerializeField] private GameObject systemUserName;

    // View -> Planet Details
    [SerializeField] private GameObject viewPlanetDetails;

    public GameObject CurrentActiveView { get; private set; }

    #region Events

    public delegate void HobbyCreationViewActive(bool active);

    public event HobbyCreationViewActive OnHobbyCreationViewActivation;

    #endregion

    private LaunchSlider _launchSlider;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(transform.root.gameObject);

            _camerasManager = CamerasManager.GetInstance();
            _launchSlider = viewHobbyCreation.transform.Find("LaunchSlider").GetComponent<LaunchSlider>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ActivateHobbyCreationView()
    {
        DeactivateViewMain();

        // Deactivate this view, if planet launched
        AppEvents.OnHobbyLaunched += DeactivateHobbyCreationView; 

        CurrentActiveView = viewHobbyCreation;
        OnHobbyCreationViewActivation?.Invoke(true);

        // Slow fade-in anim for hobby creation background
        StartCoroutine(ActivateBackgroundAfterDelay(0.5f, 0, 1));
    }
    public void DeactivateHobbyCreationView()
    {
        // Unsub from launch event 
        AppEvents.OnHobbyLaunched -= DeactivateHobbyCreationView;
        
        OnHobbyCreationViewActivation?.Invoke(false);

        backgroundGradient.SetActive(false);
        viewHobbyCreation.SetActive(false);
        ActivateViewMain();

        CurrentActiveView = null;
        
        Debug.Log("[DEBUG]: ViewsManager: Hobby creation view deactivated.");
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

    private void ActivateViewMain()
    {
        viewMain.SetActive(true);
    }
    private void DeactivateViewMain()
    {
        viewMain.SetActive(false);
    }

    IEnumerator ActivateBackgroundAfterDelay(float delay, float from, float to)
    {
        yield return new WaitForSeconds(delay);
        
        gradientPlanet.SetActive(true);
        viewHobbyCreation.SetActive(true);

        float elapsedTimeIntervalButton = 0; 
        float elapsedTimeShareButton = -0.5f;
        float elapsedTimeReminderButton = -1f;
        float elapsedTimeGradient = -1.5f;
        float duration = 1f;

        // Get sprites and images to set alpha channel on dynamically
        SpriteRenderer spriteRenderer = gradientPlanet.GetComponent<SpriteRenderer>();
        GameObject intervalButtonContainer = viewHobbyCreation.transform.Find("IntervalSelectionContainer").gameObject;
        GameObject shareButtonContainer = viewHobbyCreation.transform.Find("ShareButtonContainer").gameObject;
        GameObject reminderButtonContainer = viewHobbyCreation.transform.Find("ReminderButtonContainer").gameObject;
        
        while (elapsedTimeGradient < duration)
        {
            // Interval Button
            float interpolatorIntervalButton = elapsedTimeIntervalButton / duration; // Current ratio between from and to 
            foreach (Transform child in intervalButtonContainer.transform)
            {
                if (child.TryGetComponent<Image>(out var intervalButtonImage))
                {
                    var intervalButtonColor = intervalButtonImage.color; // Get current color 
                    intervalButtonColor.a = Mathf.Lerp(from, to, interpolatorIntervalButton); // Update gradient alpha 
                    intervalButtonImage.color = intervalButtonColor; // Set updated alpha
                }

                if (child.TryGetComponent<TMP_Text>(out var text))
                {
                    var textColor = text.color;
                    textColor.a = Mathf.Lerp(from, to, interpolatorIntervalButton);
                    text.color = textColor;
                }
            }
            
            // Share Button
            float interpolatorShareButton = elapsedTimeShareButton / duration; // Current ratio between from and to 
            foreach (Transform child in shareButtonContainer.transform)
            {
                if (child.TryGetComponent<Image>(out var shareButtonImage))
                {
                    var shareButtonColor = shareButtonImage.color; // Get current color 
                    shareButtonColor.a = Mathf.Lerp(from, to, interpolatorShareButton); // Update gradient alpha 
                    shareButtonImage.color = shareButtonColor; // Set updated alpha
                }
            }
            
            //  Reminder Button 
            float interpolatorReminderButton = elapsedTimeReminderButton / duration; // Current ratio between from and to 
            foreach (Transform child in reminderButtonContainer.transform)
            {
                if (child.TryGetComponent<Image>(out var reminderButtonImage))
                {
                    var reminderButtonColor = reminderButtonImage.color; // Get current color 
                    reminderButtonColor.a = Mathf.Lerp(from, to, interpolatorReminderButton); // Update gradient alpha 
                    reminderButtonImage.color = reminderButtonColor; // Set updated alpha
                }
            }
            
            // Gradient 
            float interpolatorGradient = elapsedTimeGradient / duration; // Current ratio between from and to 
            Color imageColor = spriteRenderer.color; // Get current color 
            imageColor.a = Mathf.Lerp(from, to, interpolatorGradient); // Update gradient alpha 
            spriteRenderer.color = imageColor; // Set updated alpha
            
            elapsedTimeIntervalButton += Time.deltaTime;
            elapsedTimeShareButton += Time.deltaTime;
            elapsedTimeReminderButton += Time.deltaTime;
            elapsedTimeGradient += Time.deltaTime;
            
            yield return null;
        }
    }
}