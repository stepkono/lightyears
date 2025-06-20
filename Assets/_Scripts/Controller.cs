using UnityEngine;
using UnityEngine.Rendering;

[DefaultExecutionOrder(-1)] // So that this script runs before all the other ones
public class Controller : MonoBehaviour
{
    public static Controller Instance { get; private set; }
    
    [SerializeField] private GameObject systemManager;
    [SerializeField] private GameObject hobbyCreator;
    [SerializeField] private GameObject globalVolume;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CreateNewHobby()
    {
        Debug.Log("[INFO]: Clicked on Create New Hobby.");
        hobbyCreator.GetComponent<HobbyCreator>().CreateNewHobby();
        globalVolume.GetComponent<VolumeManager>().ActivateVolumeBlur();
    }
    
    public void SaveNewHobby(HobbyData hobbyData)
    {
        systemManager.GetComponent<SystemManager>().SaveNewHobby(hobbyData);
    }
}
