using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class SystemManager : MonoBehaviour
{
    public LinkedList<GameObject> HobbyPlanets { get; private set; }
    [SerializeField] private CinemachineCamera topDownCamera;

    private void Awake()
    {
        HobbyPlanets = new LinkedList<GameObject>();
        topDownCamera = topDownCamera.GetComponent<CinemachineCamera>();
    }

    /**
     * Saves a new hobby by setting up its orbit, calculating rotation speed and inserting it into the system list in the right order.
     */
    public void SaveNewHobby(HobbyManager hobby)
    {
        HobbyData hobbyData = hobby.GetHobbyData();
        // Show orbit ring
        hobby.GetOrbitContainer().gameObject.SetActive(true); 
        // Set rotation speed
        float degSecond; 
        if (hobbyData.GetDaysInterval() == 0)
        {
            degSecond = 0; // Prevent null division 
        }
        else
        {
            degSecond = -360f / (hobbyData.GetDaysInterval() * 24 * 60 * 60);
        }
        hobby.SetRotationSpeed(degSecond);
        // Insert into system considering order 
        InsertIntoList(hobby.gameObject);
        Debug.Log("[DEBUG]: SystemManger: New hobby has been saved.");
    }

    /**
     * Inserts a hobby planet into the linked list ordered by rotation speed (orbit time).
     * Also updates the top-down camera position and all orbits when planet count increases.
     */
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
            topDownCamera.transform.Translate(0, 15.2f, 0, Space.World);
            UpdateOrbits();
        }
    }

    /**
     * Updates all orbits by incrementally increasing their range.
     */
    private void UpdateOrbits()
    {
        int rang = 0; 
        Debug.Log("[INFO]: SystemManger: Updating orbits...");
        Debug.Log("------------------------------------------------------------------------------");
        foreach (GameObject hobbyPlanet in HobbyPlanets)
        {
            Debug.Log("[INFO]: SystemManager: HOBBY NAME: " + hobbyPlanet.GetComponent<HobbyManager>().GetHobbyData().GetName() + " RANG :" + rang);
            Debug.Log("[INFO]: SystemManager: HOBBY SPEED: " + hobbyPlanet.GetComponent<HobbyManager>().GetRotationSpeed());
            hobbyPlanet.GetComponent<HobbyManager>().UpdateRang(rang);
            ++rang; 
        }
        Debug.Log("[------------------------------------------------------------------------------");
    }
}