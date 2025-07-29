using System.Collections;
using UnityEngine;

public class FishManager : MonoBehaviour
{
    public static FishManager instance; // Singleton instance for global access

    public FishData[] fishTypes;        // Array of available fish types
    public MashReel mashReel;           // Reference to the MashReel minigame
    [SerializeField] private Canvas UI_Canvas; // Main fishing UI canvas

    private void Awake()
    {
        // Subscribe to MashReel events for catch/loss
        if (mashReel != null)
        {
            mashReel.OnFishCaught += HandleFishingEnd;
            mashReel.OnFishLost += HandleFishingEnd;
        }

        // Singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Handles the end of a fishing attempt (caught or lost).
    /// Disables the fishing UI.
    /// </summary>
    private void HandleFishingEnd()
    {
        if (UI_Canvas != null)
            UI_Canvas.enabled = false;
    }

    /// <summary>
    /// Begins the fishing process by enabling the UI and waiting for a bite.
    /// </summary>
    public void Castline()
    {
        if (UI_Canvas != null)
            UI_Canvas.enabled = true;
        StartCoroutine(WaitForBite());
    }

    /// <summary>
    /// Coroutine that waits for a random time before selecting a fish and starting the minigame.
    /// </summary>
    private IEnumerator WaitForBite()
    {
        yield return new WaitForSeconds(Random.Range(2f, 5f));
        FishData selectedFish = GetRandomFish();

        if (selectedFish != null && mashReel != null)
        {
            mashReel.SetFish(selectedFish);
        }
    }

    /// <summary>
    /// Immediately starts fishing with a randomly selected fish.
    /// </summary>
    public void StartFishing()
    {
        FishData selectedFish = GetRandomFish();

        if (selectedFish != null && mashReel != null)
        {
            mashReel.SetFish(selectedFish);
        }
    }

    /// <summary>
    /// Returns a random FishData from the available fish types.
    /// </summary>
    /// <returns>A random FishData, or null if none are available.</returns>
    private FishData GetRandomFish()
    {
        if (fishTypes.Length == 0)
        {
            Debug.LogError("No fish types available!");
            return null;
        }
        int randomIndex = Random.Range(0, fishTypes.Length);
        return fishTypes[randomIndex];
    }
}
