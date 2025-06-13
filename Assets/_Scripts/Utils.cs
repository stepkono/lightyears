using UnityEngine;

public class Utils : MonoBehaviour
{
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

    /*public static Vector3 DynamicWorldToScreen(Vector3 screenPosition, Transform activeCameraPos)
    {
        if (_canvas == null)
        {
            _canvas = FindObjectOfType<Canvas>();
        }
        
        Vector3 cameraPosition = activeCameraPos.position;
        float canvasDistance = Vector3.Distance(cameraPosition, _canvas.transform.position);
        Debug.Log("Distance: " + canvasDistance);
        screenPosition.z = canvasDistance;
        
        return 
    }*/
}
