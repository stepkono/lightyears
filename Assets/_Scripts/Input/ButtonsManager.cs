using UnityEngine;

public class ButtonsManager : MonoBehaviour
{
    public void CreateNewHobby()
    {
        Debug.Log("[DEBUG]: Clicked create new hobby.");
        var planetPrefab = Resources.Load<GameObject>("PlanetContainer");
        if (planetPrefab != null)
        {
            GameObject planet = Instantiate(planetPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            Debug.Log("[DEBUG]: Created new planet.");
        }
        else
        {
            Debug.Log("[ERROR]: Planet Prefab is null.");
        }
    }
}
