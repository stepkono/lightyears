using UnityEngine;

public class Utils : MonoBehaviour
{
    public static Vector3 ScreenToWorld(Vector3 screenPosition, Camera camera)
    {
        screenPosition.z = camera.nearClipPlane;
        return camera.ScreenToWorldPoint(screenPosition);
    } 
}
