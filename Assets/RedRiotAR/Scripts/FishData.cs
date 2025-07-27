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
}
