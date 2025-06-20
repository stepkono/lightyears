using UnityEngine;

public class HobbyManager : MonoBehaviour
{
    /*--------------------CHILDREN-------------------*/
    private Transform _orbitContainer;
    private Transform _planetContainer;
    private HobbyData _hobbyData;
    /*--------------TECHNICAL METADATA---------------*/
    private Vector3 _orbitRadius;
    private Vector3 _planetContainerTransform;
    private float _degPerSecond;
    private int _rang; 
    /*-----------------HOBBY METADATA----------------*/
    private float _executionFrequency;
    // 
    
    private void Awake()
    {
        _orbitContainer = transform.Find("OrbitContainer");
        _planetContainer = transform.Find("PlanetContainer");

        _orbitRadius = _orbitContainer.localScale; 
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //SetRotationSpeed(0.00042f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, _degPerSecond * Time.deltaTime * Utils.TIMESCALER, 0);
    }

    public void SetHobbyData(HobbyData hobbyData)
    {
        _hobbyData = hobbyData;
    }
    
    public void SetRotationSpeed(float daysFrequency)
    {
        _degPerSecond = -360f / (daysFrequency * 24 * 60 * 60);
    }

    public void SetOrbitRadius(float radius)
    {
        
    }

    public float GetRotationSpeed()
    {
        return _degPerSecond;
    }

    public HobbyData GetHobbyData()
    {
        return _hobbyData;
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
