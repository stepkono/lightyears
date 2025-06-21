using System;
using System.Collections.Generic;
using UnityEngine;

public class SystemManager : MonoBehaviour
{
    public LinkedList<GameObject> HobbyPlanets { get; private set; }

    private void Awake()
    {
        HobbyPlanets = new LinkedList<GameObject>();
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
        if (HobbyPlanets.Count == 0)
        {
            HobbyPlanets.AddFirst(hobbyPlanet);
            Debug.Log("[DEBUG]: SystemManager: List empty. Inserting first planet.");
        }
        else
        {
            LinkedListNode<GameObject> currentNode = HobbyPlanets.First;
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
                HobbyPlanets.AddLast(hobbyPlanet);
            }
            else
            {
                HobbyPlanets.AddBefore(currentNode, hobbyPlanet);
            }
            
            UpdateOrbits();
        }
    }

    private void UpdateOrbits()
    {
        int rang = 0; 
        
        foreach (GameObject hobbyPlanet in HobbyPlanets)
        {
            hobbyPlanet.GetComponent<HobbyManager>().UpdateRang(rang);
            ++rang; 
        }
    }
}