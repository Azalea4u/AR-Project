using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MashReel : MonoBehaviour
{
    [Header("Fish Data")]
    [SerializeField] public FishData currentFishData;   // Reference to the current fish's data

    [SerializeField] Transform topPivot;                // Top boundary of fishing area
    [SerializeField] Transform bottomPivot;             // Bottom boundary of fishing area

    [SerializeField] Transform fish;                    // Fish transform to move

    float fishPosition;                                 // Current normalized position of the fish (0 = bottom, 1 = top)
    float fishDestination;                              // Target normalized position for the fish to move towards

    float fishTimer;                                    // Timer for when the fish picks a new destination
    float timerMultiplier = 3f;                         // How often the fish picks a new destination

    float fishSpeed;                                    // Used by SmoothDamp for fish movement
    float smoothMotion = 1f;                            // Smoothing factor for fish movement

    [Header("Hook Settings")]
    [SerializeField] Transform hook;                    // Hook transform
    float hookPosition;                                 // Current normalized position of the hook
    float hookSize = 0.2f;                              // Size of the hook zone (normalized)
    float hookPower = 1.0f;                             // Amount of progress gained per successful mash
    float hookProgress;                                 // Current progress towards catching the fish
    float hookProgressDegradationPower = 0.03f;         // How quickly progress degrades

    [SerializeField] Image hookImage;                   // UI image for the hook zone

    [SerializeField] Transform progressBarContainer;    // Container for progress bar UI
    [SerializeField] Slider progressBarSlider;          // Progress bar UI element

    bool pause = false;                                 // If true, disables game logic

    float failTimer = 10f;                               // Time before failing if progress is not made

    [Header("Input System")]
    [SerializeField] private InputActionAsset inputActions;
    private InputAction tapAction;
    private InputAction tapPositionAction;

    [Header("Audios")]
    [SerializeField] private AudioSource startFishing;
    [SerializeField] private AudioSource fishLost;

    private void Awake()
    {
        if (inputActions != null)
        {
            inputActions.Enable();
            tapAction = inputActions.FindAction("Tap");
            tapPositionAction = inputActions.FindAction("TapPosition");
        }
    }

    private void OnEnable()
    {
        if (tapAction != null)
            tapAction.performed += OnTapPerformed;
    }

    private void OnDisable()
    {
        if (tapAction != null)
            tapAction.performed -= OnTapPerformed;
    }

    private void Start()
    {
        // Initialize fish data and UI
        if (currentFishData != null)
        {
            ApplyFishData(currentFishData);
        }
        Resize();
        SetStaticHook();
    }

    // Sets the current fish and applies its data.
    public void SetFish(FishData fishData)
    {
        currentFishData = fishData;
        ApplyFishData(fishData);
        ResetFishing();
        if (startFishing != null)
            startFishing.Play();
    }

    // Applies fish-specific settings from FishData.
    private void ApplyFishData(FishData fishData)
    {
        timerMultiplier = fishData.timeLimit;
        smoothMotion = fishData.fishMoveSpeed;
        failTimer = fishData.timeLimit;

        // Randomize hook size and power within the fish's range
        hookSize = Random.Range(fishData.hookSizeMin, fishData.hookSizeMax);
        hookPower = Random.Range(fishData.hookPowerMin, fishData.hookPowerMax);

        Resize();
    }

    // Resets all gameplay variables for a new fishing attempt.
    private void ResetFishing()
    {
        fishPosition = 0f;
        fishDestination = 0f;
        fishTimer = 0f;
        fishSpeed = 0f;
        hookPosition = 0.5f;
        hookProgress = 0f;
        pause = false;
    }

    private void Update()
    {
        if (pause)
        {
            return; 
        }
        Fishing();
        ProgressCheck();
    }

    // Resizes the hook zone UI to match the fishing area and hook size.
    private void Resize()
    {
        float ySize = hookImage.rectTransform.rect.height;
        Vector3 ls = hook.localScale;
        float distance = Vector3.Distance(topPivot.position, bottomPivot.position);
        ls.y = (distance / ySize * hookSize);
        hook.localScale = ls;
    }

    // Sets the hook to the middle of the fishing zone.
    private void SetStaticHook()
    {
        hookPosition = 0.5f;
        hook.position = Vector3.Lerp(bottomPivot.position, topPivot.position, hookPosition);
    }

    // Called when the "Tap" action is performed.
    private void OnTapPerformed(InputAction.CallbackContext context)
    {
        // Optionally, get tap position if needed for future logic
        Vector2 touchPosition = Vector2.zero;
        if (tapPositionAction != null)
            touchPosition = tapPositionAction.ReadValue<Vector2>();

        float min = hookPosition - hookSize / 2;
        float max = hookPosition + hookSize / 2;

        // Only allow progress if fish is in the hook zone
        if (min < fishPosition && fishPosition < max)
        {
            hookProgress += (hookPower / 10);
        }
        else
        {
            // Penalize for mashing at the wrong time
            hookProgress -= hookProgressDegradationPower * 2;
        }
    }

    // Handles fish movement logic, picking new destinations at intervals.
    private void Fishing()
    {
        fishTimer -= Time.deltaTime;
        if (fishTimer < 0f)
        {
            // Pick a new timer interval for more frequent movement
            fishTimer = Random.Range(0.5f, 1.5f);
            fishDestination = Random.value;
        }

        // Smoothly move fish towards its destination
        fishPosition = Mathf.SmoothDamp(fishPosition, fishDestination, ref fishSpeed, smoothMotion);
        fish.position = Vector3.Lerp(bottomPivot.position, topPivot.position, fishPosition);
    }

    // Updates the progress bar and checks for catch/fail conditions.
    private void ProgressCheck()
    {
        progressBarSlider.value = hookProgress;

        // degrade progress if not mashing or mashing at the wrong time
        float min = hookPosition - hookSize / 2;
        float max = hookPosition + hookSize / 2;

        if (!(min < fishPosition && fishPosition < max))
        {
            hookProgress -= hookProgressDegradationPower * Time.deltaTime;

            failTimer -= Time.deltaTime;
            if (failTimer < 0f)
            {
                FishLost();
            }
        }

        if (hookProgress >= 1f)
        {
            FishCaught();
        }
        else if (hookProgress < 0f)
        {
            hookProgress = 0f;
        }
        hookProgress = Mathf.Clamp(hookProgress, 0, 1f);
    }

    public event System.Action OnFishCaught;
    public event System.Action OnFishLost;

    private void FishCaught()
    {
        pause = true;
        Debug.Log("FISH CAUGHT!");
        OnFishCaught?.Invoke();
    }

    private void FishLost()
    {
        fishLost.Play();
        pause = true;
        Debug.Log("FISH LOST!");
        OnFishLost?.Invoke();
    }

    public void Reset()
    {
        // Reset all variables to initial state
        ResetFishing();
        SetStaticHook();
        Resize();
    }
}
