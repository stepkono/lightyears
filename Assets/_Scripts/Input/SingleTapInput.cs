using UnityEngine;

public class SingleTapInput : MonoBehaviour
{
    public Camera currentCamera;  
    void Start()
    {
        //currentCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentCamera != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    var planet = hit.collider.GetComponent<PlanetManager>();
                    if (planet != null)
                    {
                        Debug.Log("Planet hit.");
                        planet.OnTouch();
                    }
                }
            }
        }
        else
        {
            Debug.Log("No main camera found");
        }
    }
}
