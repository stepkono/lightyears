using UnityEngine;
using UnityEngine.InputSystem;

public class TouchManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction deltaTouch; 
    
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        deltaTouch = playerInput.actions["DragView"];
    }

    private void OnEnable()
    {
        deltaTouch.performed += ScrollView;
    }

    private void OnDisable()
    {
        deltaTouch.performed -= ScrollView;
    }

    private void ScrollView(InputAction.CallbackContext context)
    {
        
    }
}
