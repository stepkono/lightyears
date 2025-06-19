using System;
using Unity.Cinemachine;
using UnityEngine;

public class PlanetManager : MonoBehaviour
{
    private int _visibleIndex = 1;
    private CamerasManager _camerasManager; 

    void Awake()
    {
        _camerasManager = CamerasManager.GetInstance();
        var planetIndex = 1; 
        foreach (Transform child in transform)
        {
            if (child.GetComponent<CinemachineCamera>() == null)
            {
                child.gameObject.SetActive(_visibleIndex == planetIndex);
            }
            planetIndex++;
        }
    }

    public void SetVisibleIndex(int planetIndex)
    {
        _visibleIndex = planetIndex;
    }

    public void OnTouch()
    {
        Debug.Log("Planet touched. " + name);
        var planetCam = GameObject.Find("PlanetCam");
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
