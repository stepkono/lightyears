using System;
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
    
    // View -> Invest Hours
    [SerializeField] private GameObject viewInvestHours;

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

        viewPlanetDetails.SetActive(true);
        DeactivateViewMain();

        StartCoroutine(ActivateHobbyStatsAfterDelay(0.5f, planet));

        CurrentActiveView = viewPlanetDetails;
    }

    public void DeactivatePlanetDetailView()
    {
        viewPlanetDetails.SetActive(false);
        _camerasManager.SetCurrentCamera(mainSceneCamera);

        viewPlanetDetails.transform.Find("ButtonAddHours").GetComponent<ButtonsManager>().RemoveSelectedHobby();

        ResetPlanetDetailUI();
        
        ActivateViewMain();
        CurrentActiveView = null;
    }

    public void ActivateInvestHoursView(HobbyManager hobby)
    {
        CurrentActiveView.gameObject.SetActive(false);
        
        OnHobbyCreationViewActivation?.Invoke(true);
        
        CinemachineCamera centerPlanetCam = hobby.gameObject.transform.Find("PlanetContainer").Find("CenterPlanetCam").GetComponent<CinemachineCamera>();
        _camerasManager.SetCurrentCamera(centerPlanetCam);
        
        viewInvestHours.SetActive(true);
        TimerSlider timerSlider = viewInvestHours.transform.Find("Mask").Find("Slider").GetComponent<TimerSlider>();
        timerSlider.SetHobby(hobby);
        
        CurrentActiveView = viewInvestHours;
    }
    
    public void DeactivateInvestHoursView(HobbyManager hobby)
    {
        viewInvestHours.SetActive(false);
        
        OnHobbyCreationViewActivation?.Invoke(false);
        PlanetManager planetManager = hobby.gameObject.transform.Find("PlanetContainer").GetComponent<PlanetManager>();
        
        ActivatePlanetDetailView(planetManager);
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

    private void ResetPlanetDetailUI()
    {
        var viewRoot = viewPlanetDetails.transform;
        
        Transform hobbyNameTextField = viewRoot.Find("HobbyName");
        var hoursCountTextField = viewRoot.Find("HoursInvested").Find("HoursCount");
        var stageNameTextField = viewRoot.Find("HobbyStats").Find("LifeStageName");
        var streakCountTextField = viewRoot.Find("HobbyStats").Find("StreakCount");
        var creationDateTextField = viewRoot.Find("HobbyStats").Find("CreationDate");

        var statsBackground = viewRoot.Find("HobbyStats").Find("Background");
        var staticTextCreationDate = viewRoot.Find("HobbyStats").Find("Entstehungsdatum");
        var staticTextStageName = viewRoot.Find("HobbyStats").Find("Entwicklungsstufe");
        var staticTextStreak = viewRoot.Find("HobbyStats").Find("Streak");
        var staticTextStunden = viewRoot.Find("HoursInvested").Find("Stunden");
        var staticButtonAddHours = viewRoot.Find("ButtonAddHours");

        var hobbyNameColor = hobbyNameTextField.GetComponent<TMP_Text>().color;
        hobbyNameColor.a = 0;
        hobbyNameTextField.GetComponent<TMP_Text>().color = hobbyNameColor;

        var hoursInvestedColor = hoursCountTextField.GetComponent<TMP_Text>().color;
        hoursInvestedColor.a = 0;
        hoursCountTextField.GetComponent<TMP_Text>().color = hoursInvestedColor;

        var stageNameColor = stageNameTextField.GetComponent<TMP_Text>().color;
        stageNameColor.a = 0;
        stageNameTextField.GetComponent<TMP_Text>().color = stageNameColor;

        var streakCountColor = streakCountTextField.GetComponent<TMP_Text>().color;
        streakCountColor.a = 0;
        streakCountTextField.GetComponent<TMP_Text>().color = streakCountColor;

        var creationDateColor = creationDateTextField.GetComponent<TMP_Text>().color;
        creationDateColor.a = 0;
        creationDateTextField.GetComponent<TMP_Text>().color = creationDateColor;
        
        var backgroundColor = statsBackground.GetComponent<Image>().color;
        backgroundColor.a = 0;
        statsBackground.GetComponent<Image>().color = backgroundColor;

        var staticTextCreationDateColor = staticTextCreationDate.GetComponent<TMP_Text>().color;
        staticTextCreationDateColor.a = 0;
        staticTextCreationDate.GetComponent<TMP_Text>().color = staticTextCreationDateColor;

        var staticTextStageNameColor = staticTextStageName.GetComponent<TMP_Text>().color;
        staticTextStageNameColor.a = 0;
        staticTextStageName.GetComponent<TMP_Text>().color = staticTextStageNameColor;

        var staticTextStreakColor = staticTextStreak.GetComponent<TMP_Text>().color;
        staticTextStreakColor.a = 0;
        staticTextStreak.GetComponent<TMP_Text>().color = staticTextStreakColor;

        var staticTextStundenColor = staticTextStunden.GetComponent<TMP_Text>().color;
        staticTextStundenColor.a = 0;
        staticTextStunden.GetComponent<TMP_Text>().color = staticTextStundenColor;

        var staticButtonAddHoursColor = staticButtonAddHours.GetComponent<Image>().color;
        staticButtonAddHoursColor.a = 0;
        staticButtonAddHours.GetComponent<Image>().color = staticButtonAddHoursColor;
    }

    IEnumerator ActivateHobbyStatsAfterDelay(float delay, PlanetManager planet)
    {
        var viewRoot = viewPlanetDetails.transform;

        HobbyManager hobby = planet.GetComponentInParent<HobbyManager>();
        HobbyData hobbyData = hobby.GetHobbyData();

        // Hobby name
        var hobbyName = hobbyData.GetName();
        Transform hobbyNameTextField = viewRoot.Find("HobbyName");
        hobbyNameTextField.GetComponent<TMP_Text>().text = hobbyName;

        // Total invested hours 
        var totalHoursInvested = hobby.GetTotalInvestedHoursAsString();
        Debug.Log($"TOTAL INVESTED HOURS: {totalHoursInvested.Hours}h {totalHoursInvested.Minutes}min");
        var hoursCountTextField = viewRoot.Find("HoursInvested").Find("HoursCount");
        var minCountTextField = viewRoot.Find("HoursInvested").Find("MinutesCount");
        hoursCountTextField.GetComponent<TMP_Text>().text = totalHoursInvested.Hours;
        minCountTextField.GetComponent<TMP_Text>().text = totalHoursInvested.Minutes;
        
        // Name of the current development stage of the planet
        var stageName = hobby.GetCurrentStageName();
        var stageNameTextField = viewRoot.Find("HobbyStats").Find("LifeStageName");
        stageNameTextField.GetComponent<TMP_Text>().text = stageName;

        // Streak count 
        var streakCount = hobby.GetIntervalStreak().ToString();
        var streakCountTextField = viewRoot.Find("HobbyStats").Find("StreakCount");
        streakCountTextField.GetComponent<TMP_Text>().text = streakCount;

        // Creation date 
        var creationDate = hobbyData.GetCreationDate().ToString("dd.MM.yyyy");
        var creationDateTextField = viewRoot.Find("HobbyStats").Find("CreationDate");
        creationDateTextField.GetComponent<TMP_Text>().text = creationDate;

        // Static Stats-elems
        var statsBackground = viewRoot.Find("HobbyStats").Find("Background");
        var staticTextCreationDate = viewRoot.Find("HobbyStats").Find("Entstehungsdatum");
        var staticTextStageName = viewRoot.Find("HobbyStats").Find("Entwicklungsstufe");
        var staticTextStreak = viewRoot.Find("HobbyStats").Find("Streak");
        var staticTextStunden = viewRoot.Find("HoursInvested").Find("Stunden");
        var staticButtonAddHours = viewRoot.Find("ButtonAddHours");

        ButtonsManager buttonManager =
            viewPlanetDetails.transform.Find("ButtonAddHours").GetComponent<ButtonsManager>();
        buttonManager.SetSelectedHobby(hobby);

        yield return new WaitForSeconds(delay);

        float duration = 1;
        float elapsedTime = 0;
        float from = 0;
        float to = 1;

        while (elapsedTime <= duration)
        {
            float interpolator = elapsedTime / duration;

            // Dynamic elements 
            var hobbyNameColor = hobbyNameTextField.GetComponent<TMP_Text>().color;
            hobbyNameColor.a = Mathf.Lerp(from, to, interpolator);
            hobbyNameTextField.GetComponent<TMP_Text>().color = hobbyNameColor;

            var hoursInvestedColor = hoursCountTextField.GetComponent<TMP_Text>().color;
            hoursInvestedColor.a = Mathf.Lerp(from, to, interpolator);
            hoursCountTextField.GetComponent<TMP_Text>().color = hoursInvestedColor;
            
            var minCountTextFieldColor = hoursCountTextField.GetComponent<TMP_Text>().color;
            minCountTextFieldColor.a = Mathf.Lerp(from, to, interpolator);
            minCountTextField.GetComponent<TMP_Text>().color = minCountTextFieldColor;

            var stageNameColor = stageNameTextField.GetComponent<TMP_Text>().color;
            stageNameColor.a = Mathf.Lerp(from, to, interpolator);
            stageNameTextField.GetComponent<TMP_Text>().color = stageNameColor;

            var streakCountColor = streakCountTextField.GetComponent<TMP_Text>().color;
            streakCountColor.a = Mathf.Lerp(from, to, interpolator);
            streakCountTextField.GetComponent<TMP_Text>().color = streakCountColor;

            var creationDateColor = creationDateTextField.GetComponent<TMP_Text>().color;
            creationDateColor.a = Mathf.Lerp(from, to, interpolator);
            creationDateTextField.GetComponent<TMP_Text>().color = creationDateColor;

            // Static elements
            var backgroundColor = statsBackground.GetComponent<Image>().color;
            backgroundColor.a = Mathf.Lerp(from, to, interpolator);
            statsBackground.GetComponent<Image>().color = backgroundColor;

            var staticTextCreationDateColor = staticTextCreationDate.GetComponent<TMP_Text>().color;
            staticTextCreationDateColor.a = Mathf.Lerp(from, to, interpolator);
            staticTextCreationDate.GetComponent<TMP_Text>().color = staticTextCreationDateColor;

            var staticTextStageNameColor = staticTextStageName.GetComponent<TMP_Text>().color;
            staticTextStageNameColor.a = Mathf.Lerp(from, to, interpolator);
            staticTextStageName.GetComponent<TMP_Text>().color = staticTextStageNameColor;

            var staticTextStreakColor = staticTextStreak.GetComponent<TMP_Text>().color;
            staticTextStreakColor.a = Mathf.Lerp(from, to, interpolator);
            staticTextStreak.GetComponent<TMP_Text>().color = staticTextStreakColor;

            var staticTextStundenColor = staticTextStunden.GetComponent<TMP_Text>().color;
            staticTextStundenColor.a = Mathf.Lerp(from, to, interpolator);
            staticTextStunden.GetComponent<TMP_Text>().color = staticTextStundenColor;

            var staticButtonAddHoursColor = staticButtonAddHours.GetComponent<Image>().color;
            staticButtonAddHoursColor.a = Mathf.Lerp(from, to, interpolator);
            staticButtonAddHours.GetComponent<Image>().color = staticButtonAddHoursColor;

            elapsedTime += Time.deltaTime;

            yield return null;
        }
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
            float interpolatorIntervalButton =
                elapsedTimeIntervalButton / duration; // Current ratio between from and to 
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
            float interpolatorReminderButton =
                elapsedTimeReminderButton / duration; // Current ratio between from and to 
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