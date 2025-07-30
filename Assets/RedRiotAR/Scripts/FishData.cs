using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "NewFish", menuName = "Fishing/FishData")]
public class FishData : ScriptableObject
{
    public string fishName;                // Display name of the fish
    public int MashMin;                    // Minimum mash actions required to catch
    public int MashMax;                    // Maximum mash actions required to catch
    public float timeLimit;                // Time limit to catch the fish (seconds)
    public float fishMoveSpeed;            // Speed at which the fish moves
    public GameObject fishPrefab;          // Prefab for the fish's visual representation

    [Header("Hook Settings")]
    public float hookSizeMin = 0.15f;      // Minimum hook zone size (normalized)
    public float hookSizeMax = 0.25f;      // Maximum hook zone size (normalized)
    public float hookPowerMin = 0.8f;      // Minimum progress per successful mash
    public float hookPowerMax = 1.2f;      // Maximum progress per successful mash
}
