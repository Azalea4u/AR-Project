using UnityEngine;
using UnityEngine.InputSystem;

public class DetectTap : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] InputActionAsset inputActions;

    private InputAction tapAction;
    private InputAction tapPositionAction;

    private void Awake()
    {
        inputActions.Enable();
        tapAction = inputActions.FindAction("Tap");
        tapPositionAction = inputActions.FindAction("TapPosition");
    }

    private void OnEnable()
    {
        tapAction.performed += OnTapPerformed;
    }

    private void OnDisable()
    {
        tapAction.performed -= OnTapPerformed;
    }

    private void OnTapPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Tap detected");
        // Check if the tap action was triggered
        if (context.performed)
        {
            // Get the touch position
            Vector2 touchPosition = tapPositionAction.ReadValue<Vector2>();

            // Create a ray from the touch position
            Ray ray = Camera.main.ScreenPointToRay(touchPosition);
            RaycastHit hit;

            // Perform a raycast
            if (Physics.Raycast(ray, out hit))
            {
                // Check if the ray hit a specific object by its tag or component
                if (hit.collider.CompareTag("Pond"))
                {
                    canvas.enabled = false;
                    Debug.Log("Tapped on AR Object: " + hit.collider.name);
                    // Add minigame logic here
                }
            }
        }
    }
}
