using System.Collections.Generic;
using _Scripts;
using UnityEngine;

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
            AppEvents.OnHobbyLaunched += SaveCurrentHobby;
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
    
    private void SaveCurrentHobby(bool launched)
    {
        if (launched)
        {
            // Save Hobby
            hobbyCreator.GetComponent<HobbyCreator>().SaveCurrentHobby();
        
            // Reactivate rendering for system planets
            foreach (GameObject planet in systemManager.GetComponent<SystemManager>().HobbyPlanets)
            {
                planet.GetComponent<HobbyManager>().ActivateRendering();
            }
        }
    }

    public void RemoveFusionHobby()
    {
        hobbyCreator.GetComponent<HobbyCreator>().RemoveFriendsFromHobby();
    }

    public void MakeFusionHobby(List<string> friends)
    {
        hobbyCreator.GetComponent<HobbyCreator>().AddFriendsToHobby(friends);
    }

    public void TerminateInvestHoursView(HobbyManager hobby)
    {
        _viewsManager.DeactivateInvestHoursView(hobby);
    }
}
