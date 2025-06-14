using System;
using System.Collections.Generic;
using UnityEngine;

public class SystemManager : MonoBehaviour
{
    private LinkedList<GameObject> _hobbyPlanets;

    private void Awake()
    {
        _hobbyPlanets = new LinkedList<GameObject>();
    }

    public void SaveNewHobby(HobbyData hobbyData)
    {
        var hobbyPrefab = Resources.Load<GameObject>("HobbyContainer");
        if (hobbyPrefab != null)
        {
            float rotationSpeed = -360f / (hobbyData.GetFrequency() * 24 * 60 * 60);

            GameObject hobbyPlanet = Instantiate(hobbyPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            hobbyPlanet.GetComponent<HobbyManager>().SetRotationSpeed(rotationSpeed);
            InsertIntoList(hobbyPlanet);

            Debug.Log("[DEBUG]: Created new hobby planet.");
        }
        else
        {
            Debug.LogError("[ERROR]: HobbyContainer prefab not found.");
        }
    }

    private void InsertIntoList(GameObject hobbyPlanet)
    {
        if (_hobbyPlanets.Count == 0)
        {
            _hobbyPlanets.AddFirst(hobbyPlanet);
            Debug.Log("[DEBUG]: SystemManager: List empty. Inserting first planet.");
        }
        else
        {
            LinkedListNode<GameObject> currentNode = _hobbyPlanets.First;
            float currentPlanetRotationSpeed = currentNode.Value.GetComponent<HobbyManager>().GetRotationSpeed();
            float newPlanetRotationSpeed = hobbyPlanet.GetComponent<HobbyManager>().GetRotationSpeed();

            while ((currentPlanetRotationSpeed >= newPlanetRotationSpeed) && currentNode.Next != null)
            {
                currentNode = currentNode.Next;
                currentPlanetRotationSpeed = currentNode.Value.GetComponent<HobbyManager>().GetRotationSpeed();
                newPlanetRotationSpeed = hobbyPlanet.GetComponent<HobbyManager>().GetRotationSpeed();
            }
            
            if (currentNode.Next == null)
            {
                _hobbyPlanets.AddLast(hobbyPlanet);
            }
            else
            {
                _hobbyPlanets.AddBefore(currentNode, hobbyPlanet);
            }
            
            UpdateOrbits();
        }
    }

    private void UpdateOrbits()
    {
        int rang = 0; 
        
        foreach (GameObject hobbyPlanet in _hobbyPlanets)
        {
            hobbyPlanet.GetComponent<HobbyManager>().UpdateRang(rang);
            ++rang; 
        }
    }
}