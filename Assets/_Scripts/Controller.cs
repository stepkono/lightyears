using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)] // So that this script runs before all the other ones
public class Controller : MonoBehaviour
{
    public static Controller Instance { get; private set; }
    
    [SerializeField] private GameObject systemManager;
    [SerializeField] private GameObject hobbyCreator;
    [SerializeField] private GameObject globalVolume;
    
    #region Events

    public delegate void SaveHobby();

    public event SaveHobby OnSaveHobby;

    #endregion

    private ViewsManager _viewsManager; 
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(transform.root.gameObject);
            
            _viewsManager = ViewsManager.Instance;
            Debug.Log("INSTANCE CONTROLLER ID: " + Instance.GetInstanceID());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CreateNewHobby()
    {
        
        Debug.Log("[INFO]: Clicked on Create New Hobby.");
        // Planet mesh 
        hobbyCreator.GetComponent<HobbyCreator>().CreateNewHobby();
        
        // Remove distracting planets
        foreach (GameObject planet in systemManager.GetComponent<SystemManager>().HobbyPlanets)
        {
            planet.GetComponent<HobbyManager>().DeactivateRendering();
        }
        
        // Background blur 
        globalVolume.GetComponent<VolumeManager>().ActivateVolumeBlur();
        
        // Activate HobbyCreationView-specific UI-Elements 
        _viewsManager.ActivateHobbyCreationView();
    }
    
    public void SaveCurrentHobby()
    {
        OnSaveHobby?.Invoke();
        hobbyCreator.GetComponent<HobbyCreator>().SaveCurrentHobby();
        
        foreach (GameObject planet in systemManager.GetComponent<SystemManager>().HobbyPlanets)
        {
            planet.GetComponent<HobbyManager>().ActivateRendering();
        }
    }
}
