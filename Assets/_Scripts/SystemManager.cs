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

    public void SaveNewHobby(HobbyManager hobby)
    {
        
        HobbyData hobbyData = hobby.GetHobbyData();
        // Show orbit ring
        hobby.GetOrbitContainer().gameObject.SetActive(true); 
        // Set rotation speed
        float rotationSpeed = -360f / (hobbyData.GetDaysInterval() * 24 * 60 * 60);
        hobby.SetRotationSpeed(rotationSpeed);
        // Insert into system considering order 
        InsertIntoList(hobby.gameObject);
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