using UnityEngine;

public class ButtonsManager : MonoBehaviour
{
    public void CreateNewHobby()
    {
        Debug.Log("[DEBUG]: Clicked create new hobby.");
        var planetPrefab = Resources.Load<GameObject>("PlanetContainer");
        if (planetPrefab != null)
        {
            GameObject planet = Instantiate(planetPrefab, new Vector3(415.34f, 1687.06f, -1696.6f), Quaternion.identity);
            planet.transform.localScale = new Vector3(4f, 4f, 4f);
            Debug.Log("[DEBUG]: Created new planet.");
        }
        else
        {
            Debug.Log("[ERROR]: Planet Prefab is null.");
        }
    }
}
