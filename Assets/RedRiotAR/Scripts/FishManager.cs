using System.Collections;
using UnityEngine;

public class FishManager : MonoBehaviour
{
    public static FishManager instance;             // Singleton instance for global access

    public FishData[] fishTypes;                    // Array of available fish types
    public MashReel mashReel;                       // Reference to the MashReel minigame
    [SerializeField] private GameObject FishPanel;  // Main fishing UI canvas

    private void Awake()
    {
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

    private void Start()
    {
        FishPanel.SetActive(false); // Initially hide the fishing UI
    }

    /// <summary>
    /// Handles the end of a fishing attempt (caught or lost).
    /// Disables the fishing UI.
    /// </summary>
    private void HandleFishingEnd()
    {
        if (FishPanel != null)
            FishPanel.SetActive(false);
    }

    public void StartFishing()
    {
        if (FishPanel != null)
            FishPanel.SetActive(true);
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

    public void EndFishing()
    {
        if (FishPanel != null)
        {
            FishPanel.SetActive(false);
        }
        mashReel.Reset(); // Reset the minigame state
    }
}
