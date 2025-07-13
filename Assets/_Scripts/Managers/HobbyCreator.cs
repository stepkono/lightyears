using System;
using System.Collections.Generic;
using _Scripts;
using Unity.Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

public class HobbyCreator : MonoBehaviour
{
    private bool _firstHobbyCreated = false;
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
        AppEvents.OnHobbyLaunched += DestroyHobby;
    }

    /**
     * Destroys the current hobby planet if a hobby was not launched successfully.
     */
    private void DestroyHobby(bool launched)
    {
        if (!launched)
        {
            Destroy(_currentHobbyPlanet);
        }
    }
    
    /**
     * Creates and initializes a new hobby planet and HobbyData. Selects a random or default planet prefab
     * and sets up scene camera, creation date, and first-time setup flag.
     */
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
        
        GameObject hobbyPlanet = !_firstHobbyCreated 
            ? planetPrefabs[0] 
            : planetPrefabs[Random.Range(0, planetPrefabs.Length)];

        _firstHobbyCreated = true;
        _currentHobbyPlanet = Instantiate(hobbyPlanet, new Vector3(0, 0, 0), Quaternion.identity);
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

    /**
     * Sets the name for the current HobbyData instance and logs the change.
     */
    public void SetName(string hobbyName)
    {
        _currentHobbyData.SetName(hobbyName);
        Debug.Log("[INFO]: Hobby name has been set to: " + hobbyName);
    }

    /**
     * Activates the interval flag on the HobbyData, enabling interval input.
     */
    public void ActivateInterval()
    {
        _currentHobbyData.ActivateInterval();
    }

    /**
     * Sets the interval (in days) for the current hobby. Issues a warning if interval is not activated.
     */
    public void SetInterval(int days)
    {
        if (!_currentHobbyData.IntervalSet)
        {
            Debug.LogWarning("[INFO]: Interval has not been activated for this hobby yet.");
            return; 
        }
        _currentHobbyData.SetDaysInterval(days);
    }

    /**
     * Adds a list of friends to the current HobbyData.
     */
    public void AddFriendsToHobby(List<string> friends)
    {
        _currentHobbyData.AddFriends(friends);
    }

    /**
     * Removes all friends from the current HobbyData.
     */
    public void RemoveFriendsFromHobby()
    {
        _currentHobbyData.RemoveFriends();
    }

    /**
     * Saves the current hobby:
     * - Pushes the HobbyData to the planet's HobbyManager
     * - Registers the new hobby with the SystemManager in insertion order
     * - Switches view/camera and resets current references
     */
    public void SaveCurrentHobby()
    {
        // Save user input data in HobbyData
        _currentHobbyPlanet.GetComponent<HobbyManager>().SetHobbyData(_currentHobbyData);
        
        // Pass Hobby over to SystemManager for correct order insertion 
        _systemManager.SaveNewHobby(_currentHobbyPlanet.GetComponent<HobbyManager>());
        
        // Switch back to MainView 
        ViewsManager.Instance.DeactivateHobbyCreationView(true);
        _camerasManager.SetCurrentCamera(mainSceneCamera);
        
        // Reset contents of this HobbyCreator
        _currentHobbyData = null;
        _currentHobbyPlanet = null;
    }
}