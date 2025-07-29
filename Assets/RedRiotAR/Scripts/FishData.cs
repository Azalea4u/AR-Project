using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "NewFish", menuName = "Fishing/FishData")]
public class FishData : ScriptableObject
{
    public string fishName;
    public int MashMin;
    public int MashMax;
    public float timeLimit;
    public float fishMoveSpeed;
    public GameObject fishPrefab;

    [Header("Hook Settings")]
    public float hookSizeMin = 0.15f;
    public float hookSizeMax = 0.25f;
    public float hookPowerMin = 0.8f;
    public float hookPowerMax = 1.2f;
}
