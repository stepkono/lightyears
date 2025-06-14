using UnityEngine;

[DefaultExecutionOrder(-1)] // So that this script runs before all the other ones
public class Controller : MonoBehaviour
{
    public static Controller Instance { get; private set; }
    
    [SerializeField] private GameObject systemManager;

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

    public void SaveNewHobby(HobbyData hobbyData)
    {
        systemManager.GetComponent<SystemManager>().SaveNewHobby(hobbyData);
    }
}
