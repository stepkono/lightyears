using Unity.Cinemachine;
using UnityEngine;

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
        var hobbyPrefab = Resources.Load<GameObject>("HobbyContainer");
        if (hobbyPrefab == null)
        {
            Debug.LogError("[ERROR]: No Hobby Prefab found.");
            return; 
        }
        _currentHobbyPlanet = Instantiate(hobbyPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        Debug.Log("[INFO]: Hobby planet has been instantiated.");
        
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