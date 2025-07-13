using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Types;
using TMPro;
using UnityEngine;

public class StagesContainer
{
    private List<Stage> _evoStages = new List<Stage>();
    private List<Stage> _destStages = new List<Stage>();

    /**
     * Defines the single evolution and destruction stages
     * HoursRequired is the delta time value that has to be invested to get a stage upgrade
     */
    public StagesContainer(GameObject evoContainer, GameObject destContainer)
    {
        /*EVOLUTION*/
        Stage evoStage1 = new Stage("Ursprung", 0, 0, true);
        Stage evoStage2 = new Stage("Erwachen", 5 * 3600, 1, true);
        Stage evoStage3 = new Stage("Entfaltung", 5 * 3600, 2, true);
        Stage evoStage4 = new Stage("Veredelung", 5 * 3600, 3, true);
        Stage evoStage5 = new Stage("Zenith", 5 * 3600, 4, true);
        _evoStages.Add(evoStage1);
        _evoStages.Add(evoStage2);
        _evoStages.Add(evoStage3);
        _evoStages.Add(evoStage4);
        _evoStages.Add(evoStage5);

        if (evoContainer == null || destContainer == null)
        {
            Debug.LogWarning("Evo container and/or destContainer are null.");
            return;
        }

        Transform evoModels = evoContainer.transform;
        var index = 0;
        foreach (Transform child in evoModels)
        {
            _evoStages[index].SetModel(child.gameObject);
            index++;
        }

        /*DESTRUCTION*/
        Stage destStage = new Stage("Auslöschung", 0, false);
        Stage destStage2 = new Stage("Zerfall", 1, false);
        Stage destStage3 = new Stage("Erosion", 2, false);
        Stage destStage4 = new Stage("Verblassen", 3, false);
        Stage destStage5 = new Stage("Rissbildung", 4, false);
        _destStages.Add(destStage);
        _destStages.Add(destStage2);
        _destStages.Add(destStage3);
        _destStages.Add(destStage4);
        _destStages.Add(destStage5);

        Transform destModels = destContainer.transform;
        var index2 = 0;
        foreach (Transform child in destModels)
        {
            _destStages[index2].SetModel(child.gameObject);
            index2++;
        }

        Debug.Log("STAGES CONTAINER CREATED");
    }

    /**
     * Check which stage to get based on the current stage and time invested
     */
    public Stage GetStageForInvestedHours(float investedHours, Stage currentStage)
    {
        int currentIndex = currentStage.StageIndex;
        Debug.Log("CURRENT INDEX: " + currentIndex);
        Debug.Log("CURRENT DELTA: " + investedHours);
        while (currentIndex < _evoStages.Count - 1 && investedHours >= _evoStages[currentIndex + 1].HoursRequired)
        {
            Debug.Log("REQUIRED DELTA: " + _evoStages[currentIndex + 1].HoursRequired);
            investedHours -= _evoStages[currentIndex + 1].HoursRequired;
            currentStage = _evoStages[currentIndex + 1];
        }

        Debug.Log("NEW STAGE: " + currentStage.StageIndex);
        return currentStage;
    }

    public Stage GetStage(Stage currentStage, bool isGrowing)
    {
        if (isGrowing)
        {
            return null;
        }
        else
        {
            if (currentStage.StageIndex > 0 && !currentStage.Evo)
            {
                return _destStages[currentStage.StageIndex - 1];
            }
            else
            {
                return _destStages[currentStage.StageIndex];
            }
        }
    }

    public List<Stage> GetEvoStages()
    {
        return _evoStages;
    }

    public List<Stage> GetDestStages()
    {
        return _destStages;
    }
}

public struct InvestedTimeString
{
    public string Hours;
    public string Minutes;
    public string Seconds;

    public InvestedTimeString(string hours, string minutes, String seconds)
    {
        Hours = hours;
        Minutes = minutes;
        Seconds = seconds;
    }
}


public class HobbyManager : MonoBehaviour
{
    /*--------------------CHILDREN-------------------*/
    private Transform _orbitContainer;
    private Transform _planetContainer;
    private HobbyData _hobbyData;

    [SerializeField] private GameObject evoContainer;

    [SerializeField] private GameObject destContainer;

    /*--------------TECHNICAL METADATA---------------*/
    private float _rotatedDegrees = 0f;
    private GameObject _currentPlanetModel;
    private Vector3 _orbitRadius;
    private Vector3 _planetContainerTransform;
    private float _degPerSecond;

    private int _rang;

    /*-----------------HOBBY METADATA----------------*/
    private float _executionFrequency;
    private float _investedHours;
    private float _deltaHours;
    private bool _isGrowing;
    private bool _hasInvestedHoursInThisInterval;
    private int _intervalStreak; 
    private Stage _currentStage;
    private StagesContainer _stagesContainer;

    private void Awake()
    {
        Debug.Log("Hobby Manager Awake");
        _orbitContainer = transform.Find("OrbitContainer");
        _planetContainer = transform.Find("PlanetContainer");
        _stagesContainer = new StagesContainer(evoContainer, destContainer);

        _orbitRadius = _orbitContainer.localScale;
        _investedHours = 0;
        _deltaHours = 0;
        _intervalStreak = 0;
        _isGrowing = true;
        _hasInvestedHoursInThisInterval = false;
        _currentStage = _stagesContainer.GetEvoStages().First();
    }

    /**
     * Checks if the current stage can be upgraded or recovered based on the invested hours and growing state.
     */
    private void CheckForStageUpgrade()
    {
        if (_stagesContainer == null)
        {
            Debug.LogWarning("No stages container found.");
        }

        if (_stagesContainer.GetEvoStages()[_currentStage.StageIndex] == null)
        {
            Debug.LogWarning("No stage upgrade found.");
        }
        
        if (_isGrowing)
        {
            // Check hours if eligible for growth
            if (_currentStage.StageIndex < _stagesContainer.GetEvoStages().Count - 1 &&
                _deltaHours >= _stagesContainer.GetEvoStages()[_currentStage.StageIndex + 1].HoursRequired
               )
            {
                UpgradeStage();
            }
        }
        else
        {
            if (_deltaHours >= 2 * 3600)
            {
                _isGrowing = true;
                _deltaHours = 0;
                Debug.Log("[DEBUG]: Planet recovering...");
                destContainer.transform.GetChild(_currentStage.StageIndex).gameObject.SetActive(false);
                evoContainer.transform.GetChild(_currentStage.StageIndex).gameObject.SetActive(true);
            }
        }
    }

    /**
     * Upgrades the current evolutionary stage based on the invested time delta.
     */
    private void UpgradeStage()
    {
        evoContainer.transform.GetChild(_currentStage.StageIndex).gameObject.SetActive(false);
        destContainer.transform.GetChild(_currentStage.StageIndex).gameObject.SetActive(false);

        _currentStage = _stagesContainer.GetStageForInvestedHours(_deltaHours, _currentStage);

        evoContainer.transform.GetChild(_currentStage.StageIndex).gameObject.SetActive(true);
        _deltaHours = 0;
    }

    /**
     * Downgrades the current stage during the destruction phase.
     */
    private void DowngradeStage()
    {
        if (_isGrowing)
        {
            _isGrowing = false;
            evoContainer.transform.GetChild(_currentStage.StageIndex).gameObject.SetActive(false);
            Debug.Log("[DEBUG]: Planet entered destruction phase.");
        }
        else
        {
            destContainer.transform.GetChild(_currentStage.StageIndex).gameObject.SetActive(false);
            Debug.Log("[DEBUG]: Deep in destruction phase.");
        }

        _currentStage = _stagesContainer.GetStage(_currentStage, _isGrowing);
        Debug.Log("STAGE INDEX: " + _currentStage.StageIndex + " " + _currentStage.Name);
        destContainer.transform.GetChild(_currentStage.StageIndex).gameObject.SetActive(true);
    }

    /**
     * Handles rotation logic and detects when the planet has fully rotated, checking for potential downgrades if no hours were invested.
     */
    void Update()
    {
        float deltaRotation = _degPerSecond * Time.deltaTime * Utils.TIMESCALER;
        _rotatedDegrees += Math.Abs(deltaRotation);

        transform.Rotate(0, deltaRotation, 0);

        if (_rotatedDegrees >= 360f)
        {
            Debug.Log("[DEBUG]: Planet rotated.");
            _rotatedDegrees = 0; // reset internal interval counter 
            if (!_hasInvestedHoursInThisInterval)
            {
                Debug.Log("[DEBUG]: Planet stage got downgraded.");
                _intervalStreak = 0; 
                DowngradeStage();
            }
            else
            {
                _intervalStreak += 1; 
            }

            _hasInvestedHoursInThisInterval = false;
        }
    }

    /**
     * Assigns new HobbyData to this HobbyManager and updates the planet's UI name field.
     */
    public void SetHobbyData(HobbyData hobbyData)
    {
        _hobbyData = hobbyData;
        TMP_Text visualName = transform.Find("PlanetContainer/PlanetRoot/Canvas/HobbyName").GetComponent<TMP_Text>();
        visualName.text = _hobbyData.GetName() != null ? _hobbyData.GetName().ToUpper() : "";
    }

    /**
     * Sets the rotation speed of the planet (degrees per second).
     */
    public void SetRotationSpeed(float degSecond)
    {
        _degPerSecond = degSecond;
    }

    /**
     * Invests additional hours into this hobby, accumulates and checks for potential stage upgrade.
     */
    public void InvestHours(float newHours)
    {
        Debug.Log("[DEBUG]: Investing hours.");
        
        Debug.Log("[DEBUG]: Seconds invested: " + newHours);
        
        _investedHours += newHours;
        _deltaHours += newHours;
        _hasInvestedHoursInThisInterval = true;
        Debug.Log("[DEBUG]: DELTA HOURS SET TO " + _investedHours + ".");
        CheckForStageUpgrade();
    }

    /**
     * Returns the total invested hours as a struct with formatted hour, minute, and second strings.
     */
    public InvestedTimeString GetTotalInvestedHoursAsString()
    {
        int totalSeconds = (int)_investedHours;
        int hours = totalSeconds / 3600;
        int minutes = (totalSeconds % 3600) / 60;
        int seconds = (totalSeconds % 3600) % 60;

        return new InvestedTimeString(hours.ToString(), minutes.ToString(), seconds.ToString());
    }

    /**
     * Gets the display name of the current stage.
     */
    public String GetCurrentStageName()
    {
        return _currentStage.Name;
    }

    /**
     * Returns the current interval streak (consecutive success count for the stage).
     */
    public int GetIntervalStreak()
    {
        return _intervalStreak;
    }

    /**
     * Sets the current GameObject as the reference for the planet model.
     */
    public void SetCurrentPlanetModel(GameObject planetModel)
    {
        _currentPlanetModel = planetModel;
    }

    /**
     * Enables MeshRenderers for all evolutionary planet models.
     */
    public void ActivateRendering()
    {
        foreach (Transform planet in _planetContainer.Find("PlanetRoot").Find("EvoStages"))
        {
            planet.gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    /**
     * Disables MeshRenderers for all evolutionary planet models.
     */
    public void DeactivateRendering()
    {
        foreach (Transform planet in _planetContainer.Find("PlanetRoot").Find("EvoStages"))
        {
            planet.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    /**
     * Returns the current (absolute) rotation speed.
     */
    public float GetRotationSpeed()
    {
        return Math.Abs(_degPerSecond);
    }

    /**
     * Returns the HobbyData assigned to this manager.
     */
    public HobbyData GetHobbyData()
    {
        return _hobbyData;
    }

    /**
     * Returns a reference to the transform of the orbit container.
     */
    public Transform GetOrbitContainer()
    {
        return _orbitContainer;
    }

    /**
     * Adjusts the orbital scale and planet distance based on rank.
     */
    public void UpdateRang(int rang)
    {
        _rang = rang;

        // Update the scale of the orbit
        _orbitContainer.localScale = new Vector3(4f, 4f, 4f) + new Vector3(2f, 2f, 2f) * _rang;
        // Update the distance of planet from orbit center 
        _planetContainer.localPosition = _planetContainer.localPosition.normalized * _orbitContainer.localScale.x * 2f;
    }
}