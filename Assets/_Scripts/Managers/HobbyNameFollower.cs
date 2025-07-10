using UnityEngine;

public class HobbyNameFollower : MonoBehaviour
{
    [SerializeField] private Transform planet;
    [SerializeField] private Camera mainCamera;

    private Quaternion initialLocalRotation;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        initialLocalRotation = transform.localRotation;
    }

    void LateUpdate()
    {
        // Halte Position direkt unter dem Planeten
        transform.position = planet.position + new Vector3(0, -1.5f, 0); // adjust Y-offset

        // Option 1: immer nach oben ausgerichtet, mit Blick zur Kamera
        Vector3 forward = mainCamera.transform.forward;
        forward.y = 0;
        transform.rotation = Quaternion.LookRotation(forward);

        // Optional: Beschränke Rotation nur auf Y
        Vector3 euler = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(0, euler.y, 0);
    }
}
