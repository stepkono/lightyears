using UnityEngine;

public class HobbyManager : MonoBehaviour
{
    /*--------------------CHILDREN-------------------*/
    private Transform _orbitContainer;
    private Transform _planetContainer;
    /*--------------TECHNICAL METADATA---------------*/
    private float _orbitRadius;
    private Vector3 _planetContainerTransform;
    private float _degPerSecond;
    /*-----------------HOBBY METADATA----------------*/
    private float _executionFrequency;
    // 
    
    private void Awake()
    {
        _orbitContainer = transform.Find("OrbitContainer");
        _planetContainer = transform.Find("PlanetContainer");

        _orbitRadius = _orbitContainer.localScale.x; 
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetFrequency(0.00042f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, _degPerSecond * Time.deltaTime * Utils.TIMESCALER, 0);
    }

    public void SetFrequency(float daysFrequency)
    {
        _degPerSecond = -360f / (daysFrequency * 24 * 60 * 60);
    }
}
