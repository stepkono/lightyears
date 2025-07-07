using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Types;
using UnityEngine;

public class StagesContainer
{
    private List<Stage> _evoStages = new List<Stage>();
    private List<Stage> _destStages = new List<Stage>();

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
                Debug.Log("[DEBUG]: RETURNING CURRENT STAGE IN DESTRUCTION: " + currentStage.StageIndex);
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //SetRotationSpeed(0.00042f);
    }

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

    private void UpgradeStage()
    {
        evoContainer.transform.GetChild(_currentStage.StageIndex).gameObject.SetActive(false);
        destContainer.transform.GetChild(_currentStage.StageIndex).gameObject.SetActive(false);

        _currentStage = _stagesContainer.GetStageForInvestedHours(_deltaHours, _currentStage);

        evoContainer.transform.GetChild(_currentStage.StageIndex).gameObject.SetActive(true);
        _deltaHours = 0;
    }

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

    public void SetHobbyData(HobbyData hobbyData)
    {
        _hobbyData = hobbyData;
    }

    public void SetRotationSpeed(float degSecond)
    {
        _degPerSecond = degSecond;
    }

    public void InvestHours(float newHours)
    {
        Debug.Log("[DEBUG]: Investing hours.");
        
        int hours = (int)(newHours / 3600);
        int minutes = (int)(newHours / 60) % 60;
        int seconds = (int)(newHours % 60);
        
        Debug.Log("[DEBUG]: Seconds invested: " + newHours);
        
        _investedHours += newHours;
        _deltaHours += newHours;
        _hasInvestedHoursInThisInterval = true;
        Debug.Log("[DEBUG]: DELTA HOURS SET TO " + _investedHours + ".");
        CheckForStageUpgrade();
    }

    public InvestedTimeString GetTotalInvestedHoursAsString()
    {
        int totalSeconds = (int)_investedHours;
        int hours = totalSeconds / 3600;
        int minutes = (totalSeconds % 3600) / 60;
        int seconds = (totalSeconds % 3600) % 60;

        return new InvestedTimeString(hours.ToString(), minutes.ToString(), seconds.ToString());
    }

    public String GetCurrentStageName()
    {
        return _currentStage.Name;
    }

    public int GetIntervalStreak()
    {
        return _intervalStreak;
    }

    public void SetCurrentPlanetModel(GameObject planetModel)
    {
        _currentPlanetModel = planetModel;
    }

    public void ActivateRendering()
    {
        foreach (Transform planet in _planetContainer.Find("PlanetRoot").Find("EvoStages"))
        {
            planet.gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    public void DeactivateRendering()
    {
        foreach (Transform planet in _planetContainer.Find("PlanetRoot").Find("EvoStages"))
        {
            planet.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    public float GetRotationSpeed()
    {
        return Math.Abs(_degPerSecond);
    }

    public HobbyData GetHobbyData()
    {
        return _hobbyData;
    }

    public Transform GetOrbitContainer()
    {
        return _orbitContainer;
    }

    public void UpdateRang(int rang)
    {
        _rang = rang;

        // Update the scale of the orbit
        _orbitContainer.localScale = new Vector3(4f, 4f, 4f) + new Vector3(2f, 2f, 2f) * _rang;
        // Update the distance of planet from orbit center 
        _planetContainer.localPosition = _planetContainer.localPosition.normalized * _orbitContainer.localScale.x * 2f;
    }
}