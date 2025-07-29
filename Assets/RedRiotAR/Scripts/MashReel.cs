using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MashReel : MonoBehaviour
{
    [Header("Fish Data")]
    [SerializeField] private FishData currentFishData;

    [SerializeField] Transform topPivot;
    [SerializeField] Transform bottomPivot;

    [SerializeField] Transform fish;

    float fishPosition;
    float fishDestination;

    float fishTimer;
    float timerMultiplier = 3f;

    float fishSpeed;
    float smoothMotion = 1f;

    [Header("Hook Settings")]
    [SerializeField] Transform hook;
    float hookPosition;
    float hookSize = 0.2f;
    float hookPower = 1.0f;
    float hookProgress;
    float hookPullVelocity;
    float hookPullPower = 0.01f;
    float hookGravityPower = 0.005f;
    float hookProgressDegradationPower = 0.03f;

    [SerializeField] Image hookImage;

    [SerializeField] Transform progressBarContainer;
    [SerializeField] Slider progressBarSlider;

    bool pause = false;

    float failTimer = 10f;

    private void Start()
    {
        if (currentFishData != null)
        {
            ApplyFishData(currentFishData);
        }
        Resize();
        SetStaticHook();
    }

    private void OnEnable()
    {
        Resize();
        SetStaticHook();
    }

    public void SetFish(FishData fishData)
    {
        currentFishData = fishData;
        ApplyFishData(fishData);
        ResetFishing();
    }

    private void ApplyFishData(FishData fishData)
    {
        timerMultiplier = fishData.timeLimit;
        smoothMotion = fishData.fishMoveSpeed;
        failTimer = fishData.timeLimit;

        // Randomize hook size and power within the fish's range
        hookSize = Random.Range(fishData.hookSizeMin, fishData.hookSizeMax);
        hookPower = Random.Range(fishData.hookPowerMin, fishData.hookPowerMax);

        Resize();
        // Optionally, instantiate fishPrefab if needed
        // fish.gameObject = Instantiate(fishData.fishPrefab, fish.position, Quaternion.identity);
    }

    private void ResetFishing()
    {
        fishPosition = 0f;
        fishDestination = 0f;
        fishTimer = 0f;
        fishSpeed = 0f;
        hookPosition = 0.5f;
        hookProgress = 0f;
        hookPullVelocity = 0f;
        pause = false;
    }

    private void Update()
    {
        if (pause)
        {
            return; 
        }
        Fishing();
        MashInput();
        ProgressCheck();
    }

    private void Resize()
    {
        float ySize = hookImage.rectTransform.rect.height;
        Vector3 ls = hook.localScale;
        float distance = Vector3.Distance(topPivot.position, bottomPivot.position);
        ls.y = (distance / ySize * hookSize);
        hook.localScale = ls;
    }

    // Set the hook to the middle of the fishing zone and keep it there
    private void SetStaticHook()
    {
        hookPosition = 0.5f;
        hook.position = Vector3.Lerp(bottomPivot.position, topPivot.position, hookPosition);
    }

    private void MashInput()
    {
        bool mashPressed = false;

        // Mouse input (desktop)
        if (Input.GetMouseButtonDown(0))
            mashPressed = true;

        // Touch input (mobile/tablet)
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                if (Input.GetTouch(i).phase == TouchPhase.Began)
                {
                    mashPressed = true;
                    break;
                }
            }
        }

        if (mashPressed)
        {
            float min = hookPosition - hookSize / 2;
            float max = hookPosition + hookSize / 2;

            // Only allow progress if fish is in the hook zone
            if (min < fishPosition && fishPosition < max)
            {
                hookProgress += (hookPower / 10);
            }
            else
            {
                // Optional: penalty for mashing at the wrong time
                hookProgress -= hookProgressDegradationPower * 2;
            }
        }
    }

    private void Fishing()
    {
        fishTimer -= Time.deltaTime;
        if (fishTimer < 0f)
        {
            // Pick a new timer interval for more frequent movement
            fishTimer = Random.Range(0.5f, 1.5f);
            fishDestination = Random.value;
        }

        fishPosition = Mathf.SmoothDamp(fishPosition, fishDestination, ref fishSpeed, smoothMotion);
        fish.position = Vector3.Lerp(bottomPivot.position, topPivot.position, fishPosition);
    }

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
        pause = true;
        Debug.Log("FISH LOST!");
        OnFishLost?.Invoke();
    }
}
