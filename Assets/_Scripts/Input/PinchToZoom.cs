using UnityEngine;

public class PinchToZoom : MonoBehaviour
{
    public Camera cam;
    public float zoomSpeed = 0.1f;
    public float minZoom = 2f;
    public float maxZoom = 15f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            float prevMag = (t0.position - t0.deltaPosition - (t1.position - t1.deltaPosition)).magnitude;
            float currMag = (t0.position - t1.position).magnitude;
            float diff = prevMag - currMag;

            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize + diff * zoomSpeed, minZoom, maxZoom);
        }
    }
}
