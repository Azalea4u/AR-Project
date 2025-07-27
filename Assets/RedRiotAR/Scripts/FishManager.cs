using System.Collections;
using UnityEngine;

public class FishManager : MonoBehaviour
{
    public FishData[] fishTypes;
    public MashReel mashReel;

    public void Castline()
    {
        StartCoroutine(WaitForBite());
    }

    private IEnumerator WaitForBite()
    {
        yield return new WaitForSeconds(Random.Range(2f, 5f));
        FishData selectedFish = GetRandomFish();

        //mashReel.StartFishing(selectedFish);
    }

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
