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

    private ViewsManager _viewsManager; 
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(transform.root.gameObject);
            
            _viewsManager = ViewsManager.Instance;
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
        
        // Background blur 
        globalVolume.GetComponent<VolumeManager>().ActivateVolumeBlur();
        
        // Activate HobbyCreationView-specific UI-Elements 
        _viewsManager.ActivateHobbyCreationView();
    }
    
    public void SaveCurrentHobby()
    {
        hobbyCreator.GetComponent<HobbyCreator>().SaveCurrentHobby();
    }
}
