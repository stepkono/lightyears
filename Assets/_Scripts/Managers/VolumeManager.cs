using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VolumeManager : MonoBehaviour
{
    private Volume _volume; 
    
    private void Awake()
    {
        _volume = GetComponent<Volume>();
    }
    
    public void ActivateVolumeBlur()
    {
        if (_volume == null)
        {
            Debug.LogWarning("Global Volume is null.");
            return; 
        }

        if (_volume.profile.TryGet<DepthOfField>(out var depthOfField))
        {
            depthOfField.active = true;
            depthOfField.focalLength.value = 0f;
            depthOfField.focusDistance.value = 2f;
        }
        
        Debug.Log("[INFO]: Volume Blur activated.");
    }
}
    