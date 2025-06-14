using UnityEngine;

public class Utils : MonoBehaviour
{
    public static float TIMESCALER = 1; 
    private static Canvas _canvas; 
    
    public static Vector3 ScreenToWorld(Vector3 screenPosition, Camera camera)
    {
        if (_canvas == null)
        {
            _canvas = FindObjectOfType<Canvas>();
        }
        
        float canvasDistance = Vector3.Distance(camera.transform.position, _canvas.transform.position);
        Debug.Log("Distance: " + canvasDistance);
        screenPosition.z = canvasDistance;

        return camera.ScreenToWorldPoint(screenPosition);
    }
}
