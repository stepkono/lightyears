using System;
using Unity.Cinemachine;
using UnityEngine;

public class PlanetManager : MonoBehaviour
{
    private int _visibleIndex = 1;

    void Awake()
    {
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
            CinemachineCamera cam = GetComponentInChildren<CinemachineCamera>();
            if (cam != null)
            {
                cam.Priority = 2;
            }
            else
            {
                Debug.Log("[DEBUG]: Planet cam component is null");
            }
        }
        else
        {
            Debug.Log("[DEBUG]: Planet cam is null");
        }
    }
}
