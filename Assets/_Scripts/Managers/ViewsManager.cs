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
    [SerializeField] private GameObject friendListManager; 
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

    public void OpenFriendsList()
    {
        GameObject friendsList = viewHobbyCreation.transform.Find("ViewFriendsList").gameObject;
        friendsList.SetActive(true);
    }

    public void CloseFriendsList()
    {
        GameObject friendsList = viewHobbyCreation.transform.Find("ViewFriendsList").gameObject;
        friendsList.SetActive(false);
        Debug.Log("CLOSED");
    }

    public void DeactivateHobbyCreationView(bool launched)
    {
        // Unsub from launch event 
        AppEvents.OnHobbyLaunched -= DeactivateHobbyCreationView;

        OnHobbyCreationViewActivation?.Invoke(false);
        
        friendListManager.GetComponent<FriendListManager>().ResetVisualSection(true);
        backgroundGradient.SetActive(false);
        viewHobbyCreation.SetActive(false);
        _camerasManager.SetCurrentCamera(mainSceneCamera);
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
        viewPlanetDetails.GetComponent<CanvasGroup>().alpha = 0;
        viewPlanetDetails.SetActive(false);
        _camerasManager.SetCurrentCamera(mainSceneCamera);

        viewPlanetDetails.transform.Find("ButtonAddHours").GetComponent<ButtonsManager>().RemoveSelectedHobby();
        
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
        InputFieldController hoursInputField = timerSlider.transform.Find("Background").Find("Interactables").Find("HoursInputField").GetComponent<InputFieldController>();
        InputFieldController minutesInputField = timerSlider.transform.Find("Background").Find("Interactables").Find("MinutesInputField").GetComponent<InputFieldController>();
        hoursInputField.SetHobby(hobby);
        minutesInputField.SetHobby(hobby);
        
        ButtonsManager buttonManagerInvestHoursView = timerSlider.transform.Find("Background").Find("Interactables").Find("AcceptButton").GetComponent<ButtonsManager>();
        buttonManagerInvestHoursView.SetSelectedHobby(hobby);
        ButtonsManager goBackButton = viewInvestHours.transform.Find("GoBackButton").GetComponent<ButtonsManager>();
        goBackButton.SetSelectedHobby(hobby);
        
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
            DeactivateHobbyCreationView(true);
        }

        if (CurrentActiveView == viewPlanetDetails)
        {
            DeactivatePlanetDetailView();
        }
    }

    public void ToggleGuidanceArrow()
    {
        GameObject arrowUp = viewMain.transform.Find("VisualGuidanceArrow/Up").gameObject;
        GameObject arrowDown = viewMain.transform.Find("VisualGuidanceArrow/Down").gameObject;
        
        if (arrowUp.activeSelf)
        {
            arrowUp.SetActive(false);
            arrowDown.SetActive(true);
        }
        else
        {
            arrowUp.SetActive(true);
            arrowDown.SetActive(false);
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

    IEnumerator ActivateHobbyStatsAfterDelay(float delay, PlanetManager planet)
    {
        var viewCanvas = viewPlanetDetails.GetComponent<CanvasGroup>();
        var viewRoot = viewPlanetDetails.transform;
        var hobbyStats = viewRoot.Find("HobbyStats");
        var hoursInvested = viewRoot.Find("HoursInvested");
        //hobbyStats.gameObject.SetActive(true);

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
        var hoursCountTextFieldStats = hobbyStats.Find("HoursCount");
        var minCountTextField = hobbyStats.Find("MinutesCount");
        var secCountTextField = hobbyStats.Find("SecondsCount");
        hoursCountTextField.GetComponent<TMP_Text>().text = totalHoursInvested.Hours;
        hoursCountTextFieldStats.GetComponent<TMP_Text>().text = totalHoursInvested.Hours;
        minCountTextField.GetComponent<TMP_Text>().text = totalHoursInvested.Minutes;
        secCountTextField.GetComponent<TMP_Text>().text = totalHoursInvested.Seconds;
        
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
            
            viewCanvas.alpha = Mathf.Lerp(from, to, interpolator);
            /*statsCanvas.alpha = Mathf.Lerp(from, to, interpolator);
            hoursInvestedCanvas.alpha = Mathf.Lerp(from, to, interpolator);
            hoursInvestedCanvas.alpha = Mathf.Lerp(from, to, interpolator);*/

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
        CanvasGroup shareButtonContainer = viewHobbyCreation.transform.Find("ShareButtonContainer").GetComponent<CanvasGroup>();
        CanvasGroup reminderButtonContainer = viewHobbyCreation.transform.Find("ReminderButtonContainer").GetComponent<CanvasGroup>();

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
            shareButtonContainer.alpha = Mathf.Lerp(from, to, interpolatorShareButton);

            //  Reminder Button 
            float interpolatorReminderButton =
                elapsedTimeReminderButton / duration; // Current ratio between from and to 
            reminderButtonContainer.alpha = Mathf.Lerp(from, to, interpolatorReminderButton);

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