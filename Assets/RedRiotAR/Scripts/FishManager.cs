using System.Collections;
using UnityEngine;

public class FishManager : MonoBehaviour
{
    public FishData[] fishTypes;
    public MashReel mashReel;
    [SerializeField] private Canvas UI_Canvas;

    private void Awake()
    {
        if (mashReel != null)
        {
            mashReel.OnFishCaught += HandleFishingEnd;
            mashReel.OnFishLost += HandleFishingEnd;
        }
    }

    private void HandleFishingEnd()
    {
        if (UI_Canvas != null)
            UI_Canvas.enabled = false;
    }

    public void Castline()
    {
        if (UI_Canvas != null)
            UI_Canvas.enabled = true;
        StartCoroutine(WaitForBite());
    }

    private IEnumerator WaitForBite()
    {
        yield return new WaitForSeconds(Random.Range(2f, 5f));
        FishData selectedFish = GetRandomFish();

        if (selectedFish != null && mashReel != null)
        {
            mashReel.SetFish(selectedFish);
        }
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
