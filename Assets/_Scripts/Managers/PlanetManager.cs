using System;
using Unity.Cinemachine;
using UnityEngine;

public class PlanetManager : MonoBehaviour
{
    private int _visibleIndex = 1;
    private CamerasManager _camerasManager;
    private Transform _planetRoot;

    void Awake()
    {
        _camerasManager = CamerasManager.GetInstance();
        _planetRoot = transform.Find("PlanetRoot");
        
        var planetIndex = 1; 
        foreach (Transform child in _planetRoot.Find("EvoStages"))
        {
            child.gameObject.SetActive(_visibleIndex == planetIndex);
            planetIndex++;
        }
        foreach (Transform child in _planetRoot.Find("DestStages"))
        {
            child.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        float degPerSecond = 5; 
        _planetRoot.transform.Rotate(0, degPerSecond * Time.deltaTime, 0);
    }

    public void SetVisibleIndex(int planetIndex)
    {
        _visibleIndex = planetIndex;
    }

    public void OnTouch()
    {
        Debug.Log("Planet touched. " + name);
        var planetCam = GameObject.Find("DetailsViewPlanetCam");
        if (planetCam != null)
        {
            //CinemachineCamera cam = planetCam.GetComponent<CinemachineCamera>();;
            CinemachineCamera cmPlanetCamera = GetComponentInChildren<CinemachineCamera>();
            if (cmPlanetCamera != null)
            {
                 _camerasManager.SetCurrentCamera(cmPlanetCamera);
            }
            else
            {
                Debug.Log("[WARNING]: Planet CM camera component is null");
            }
        }
        else
        {
            Debug.Log("[WARNING]: Planet camera is null");
        }
    }
}
