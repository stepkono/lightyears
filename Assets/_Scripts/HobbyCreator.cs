using Unity.Cinemachine;
using UnityEngine;

public class HobbyCreator : MonoBehaviour
{
    [SerializeField] private GameObject systemManager; 
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
        
        // Set center view camera
        CinemachineCamera centerCamera = _currentHobbyPlanet.transform.Find("CenterPlanetCamera").GetComponent<CinemachineCamera>();
        _camerasManager.SetCurrentCamera(centerCamera);
    }

    public void SetName(string hobbyName)
    {
        _currentHobbyData.SetName(hobbyName);
    }

    public void ActivateInterval()
    {
        _currentHobbyData.ActivateInterval();
    }

    public void SetInterval(int days)
    {
        _currentHobbyData.SetDaysInterval(days);
    }

    public void SaveCurrentHobby()
    {
        _currentHobbyPlanet.GetComponent<HobbyManager>().SetHobbyData(_currentHobbyData);
        _systemManager.SaveNewHobby(_currentHobbyPlanet.GetComponent<HobbyManager>());
    }
}