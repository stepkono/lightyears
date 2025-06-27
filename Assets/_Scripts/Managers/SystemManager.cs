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
        float degSecond = -360f / (hobbyData.GetDaysInterval() * 24 * 60 * 60);
        hobby.SetRotationSpeed(degSecond);
        // Insert into system considering order 
        InsertIntoList(hobby.gameObject);
        Debug.Log("[DEBUG]: SystemManger: New hobby has been saved.");
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
            }

            if (currentPlanetRotationSpeed < newPlanetRotationSpeed)
            {
                HobbyPlanets.AddBefore(currentNode, hobbyPlanet);    
            }
            else
            {
                HobbyPlanets.AddLast(hobbyPlanet);
            }
            
            UpdateOrbits();
        }
    }

    private void UpdateOrbits()
    {
        int rang = 0; 
        Debug.Log("[DEBUG]: SystemManger: Updating orbits...");
        Debug.Log("------------------------------------------------------------------------------");
        foreach (GameObject hobbyPlanet in HobbyPlanets)
        {
            Debug.Log("[DEBUG]: SystemManager: HOBBY NAME: " + hobbyPlanet.GetComponent<HobbyManager>().GetHobbyData().GetName() + " RANG :" + rang);
            Debug.Log("[DEBUG]: SystemManager: HOBBY SPEED: " + hobbyPlanet.GetComponent<HobbyManager>().GetRotationSpeed());
            hobbyPlanet.GetComponent<HobbyManager>().UpdateRang(rang);
            ++rang; 
        }
        Debug.Log("[------------------------------------------------------------------------------");
    }
}