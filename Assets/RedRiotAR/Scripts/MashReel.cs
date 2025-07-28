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
    float hookSize = 0.1f;
    float hookPower = 5.0f;
    float hookProgress;
    float hookPullVelocity;
    float hookPullPower = 0.01f;
    float hookGravityPower = 0.005f;
    float hookProgressDegradationPower = 0.01f;

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
        Hook();
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

    private void Hook()
    {
        // Check for left mouse button or any touch
        if (Input.GetMouseButton(0) || Input.touchCount > 0)
        {
            hookPullVelocity += hookPullPower * Time.deltaTime;
        }
        
        hookPullVelocity -= hookGravityPower * Time.deltaTime;

        hookPosition += hookPullVelocity;

        if ((hookPosition - hookSize / 2) <= 0f && hookPullVelocity < 0f)
        {
            hookPullVelocity = 0f;
        }
        else if ((hookPosition + hookSize / 2) > 1f && hookPullVelocity > 0f)
        {
            hookPullVelocity = 0f;
        }

        hookPosition = Mathf.Clamp(hookPosition, hookSize / 2, 1f - hookSize / 2);
        hook.position = Vector3.Lerp(bottomPivot.position, topPivot.position, hookPosition);
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

        float min = hookPosition - hookSize / 2;
        float max = hookPosition + hookSize / 2;

        if (min < fishPosition && fishPosition < max)
        {
            hookProgress += (hookPower / 10) * Time.deltaTime;
        }
        else
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

        hookProgress = Mathf.Clamp(hookProgress, 0, 1f);
    }

    private void FishCaught()
    {
        pause = true;
        Debug.Log("FISH CAUGHT!");
    }

    private void FishLost()
    {
        pause = true;
        Debug.Log("FISH LOST!");
    }
}
