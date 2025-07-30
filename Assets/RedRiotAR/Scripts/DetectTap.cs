using UnityEngine;
using UnityEngine.InputSystem;

public class DetectTap : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] InputActionAsset inputActions;
    [SerializeField] FishManager fishManager;

    private InputAction tapAction;
    private InputAction tapPositionAction;
    private bool minigameActive = false;

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
        if (context.performed && !minigameActive)
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
                    Debug.Log("Tapped on AR Object: " + hit.collider.name);
                    // Add minigame logic here
                    CastLine();

                }
            }
        }
    }

    public void CastLine()
    {
        minigameActive = true;
        canvas.enabled = false;
        FishManager.instance.StartFishing();
    }

    public void Reset()
    {
        minigameActive = false;
        canvas.enabled = true;
    }
}
