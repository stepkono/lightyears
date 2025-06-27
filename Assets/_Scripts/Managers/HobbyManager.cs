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
        Stage evoStage1 = new Stage("first", 0, 0);
        Stage evoStage2 = new Stage("second", 24, 1);
        Stage evoStage3 = new Stage("third", 64, 2);
        Stage evoStage4 = new Stage("fourth", 128, 3);
        Stage evoStage5 = new Stage("fifth", 256, 4);
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
        Stage destStage = new Stage("first_dest", 1);
        Stage destStage2 = new Stage("second_dest", 2);
        Stage destStage3 = new Stage("third_dest", 3);
        Stage destStage4 = new Stage("fourth_dest", 4);
        Stage destStage5 = new Stage("fifth_dest", 5);
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
        var stageIndex = 0;
        while (investedHours > currentStage.HoursRequired && stageIndex < _destStages.Count)
        {
            currentStage = _evoStages[stageIndex++];
        }
        return currentStage; 
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

public class HobbyManager : MonoBehaviour
{
    /*--------------------CHILDREN-------------------*/
    private Transform _orbitContainer;
    private Transform _planetContainer;
    private HobbyData _hobbyData;
    
    [SerializeField] private GameObject evoContainer;
    [SerializeField] private GameObject destContainer;
    /*--------------TECHNICAL METADATA---------------*/
    private GameObject _currentPlanetModel; 
    private Vector3 _orbitRadius;
    private Vector3 _planetContainerTransform;
    private float _degPerSecond;
    private int _rang; 
    /*-----------------HOBBY METADATA----------------*/
    private float _executionFrequency;
    private float _investedHours;
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
        
        if (_investedHours >= _stagesContainer.GetEvoStages()[_currentStage.StageIndex].HoursRequired &&
            _currentStage.StageIndex < _stagesContainer.GetEvoStages().Count - 1
           )
        {
            UpgradeStage();
        }
    }

    private void UpgradeStage()
    {
        evoContainer.transform.GetChild(_currentStage.StageIndex).gameObject.SetActive(false);
        
        _currentStage = _stagesContainer.GetStageForInvestedHours(_investedHours, _currentStage);
        Debug.Log("STAGE INDEX: " + _currentStage.StageIndex);
        String test = evoContainer.transform.GetChild(_currentStage.StageIndex).gameObject.name;
        Debug.Log("NAME OF THE OBJECT: " + test);
        //_currentPlanetModel = Instantiate(evoContainer.transform.GetChild(_currentStage.StageIndex).gameObject, new Vector3(0, 0, 0), Quaternion.identity);

        evoContainer.transform.GetChild(_currentStage.StageIndex).gameObject.SetActive(true);
    }

    private void DowngradeStage()
    {
        
    }
    
    void Update()
    {
        transform.Rotate(0, _degPerSecond * Time.deltaTime * Utils.TIMESCALER, 0);
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
        _investedHours += newHours;
        CheckForStageUpgrade();
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
