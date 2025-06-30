using System;
using Unity.Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

public class HobbyCreator : MonoBehaviour
{
    [SerializeField] private GameObject systemManager; 
    [SerializeField] private CinemachineCamera mainSceneCamera;
    
    private CamerasManager _camerasManager;
    private SystemManager _systemManager;
    
    private HobbyData _currentHobbyData;
    private GameObject _currentHobbyPlanet; 

    private void Awake()
    {
        _camerasManager = CamerasManager.GetInstance();
        _systemManager = systemManager.GetComponent<SystemManager>();
    }
    
    public void CreateNewHobby()
    {
        // Init hobby 
        _currentHobbyData = new HobbyData();
        
        // Set creation date 
        _currentHobbyData.SetCreationDate(DateTime.Now);

        GameObject[] planetPrefabs = Resources.LoadAll<GameObject>("Models/Containers");
        if (planetPrefabs == null || planetPrefabs.Length == 0)
        {
            Debug.LogError("[ERROR]: No models prefabs found.");
            return; 
        }
        
        GameObject hobbyPlanet = planetPrefabs[Random.Range(0, planetPrefabs.Length)];
        _currentHobbyPlanet = Instantiate(hobbyPlanet, new Vector3(0, 0, 0), Quaternion.identity);
        Debug.Log("[INFO]: Hobby planet has been instantiated.");
        
        //TODO: THIS IS DEBUG ONLY 
        //_currentHobbyPlanet.GetComponent<HobbyManager>().InvestHours(25);
        
        // Set center view camera
        Transform centerCamera = _currentHobbyPlanet.transform.Find("PlanetContainer/CenterPlanetCam"); 
        if (centerCamera == null) 
        {
            Debug.Log("[ERROR]: No Center PlanetCamera found.");
            return; 
        }
        _camerasManager.SetCurrentCamera(centerCamera.GetComponent<CinemachineCamera>());
    }

    public void SetName(string hobbyName)
    {
        _currentHobbyData.SetName(hobbyName);
        Debug.Log("[INFO]: Hobby name has been set to: " + hobbyName);
    }

    public void ActivateInterval()
    {
        _currentHobbyData.ActivateInterval();
    }

    public void SetInterval(int days)
    {
        if (!_currentHobbyData.IntervalSet)
        {
            Debug.LogWarning("[INFO]: Interval has not been activated for this hobby yet.");
            return; 
        }
        _currentHobbyData.SetDaysInterval(days);
    }

    public void SaveCurrentHobby()
    {
        // Save user input data in HobbyData
        _currentHobbyPlanet.GetComponent<HobbyManager>().SetHobbyData(_currentHobbyData);
        //_currentHobbyPlanet.GetComponent<HobbyManager>().SetCurrentPlanetModel(_currentHobbyPlanet);
        
        // Pass Hobby over to SystemManager for correct order insertion 
        _systemManager.SaveNewHobby(_currentHobbyPlanet.GetComponent<HobbyManager>());
        // Switch back to MainView 
        ViewsManager.Instance.DeactivateHobbyCreationView();
        _camerasManager.SetCurrentCamera(mainSceneCamera);
        
        // Reset contents of this HobbyCreator
        _currentHobbyData = null;
        _currentHobbyPlanet = null;
    }
}