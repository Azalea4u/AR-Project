using System.Collections;
using UnityEngine;

public class FishCatchCelebrator : MonoBehaviour
{
    [SerializeField] private Canvas TitleUI;

    private MashReel mashReel;
    private GameObject shownFish;

    private void Start()
    {
        // This may not be the best way to do this, but it'll work for now!
        mashReel = FishManager.instance.mashReel;
        mashReel.OnFishCaught += Celebrate;

        if (TitleUI != null) TitleUI.enabled = false;
    }

    private void Celebrate()
    {
        if (TitleUI != null) TitleUI.enabled = true;  // Show celebratory text

        // Uncomment if currentFishData is made public
        // Otherwise, will need to find another way
        GameObject fish = mashReel.currentFishData.fishPrefab;  // Get caught fish gameObject
        
        // GameObject fish = null;

        if (fish != null)
        {
            shownFish = Instantiate(fish);  // Instantiate fish

            shownFish.transform.position = 7 * Camera.main.transform.forward + Camera.main.transform.position;  // Place fish in front of camera

            shownFish.GetComponent<Animator>().enabled = false;  // Disable swimming animation
            shownFish.GetComponent<FishBehavior>().caught = true;  // Turn off normal fish behavior

            shownFish.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, Mathf.Deg2Rad * 420, 0);
        }

        StartCoroutine(Stop());
    }

    private IEnumerator Stop()
    {
        yield return new WaitForSeconds(4f);  // Wait 5 seconds

        if (TitleUI != null) TitleUI.enabled = false;  // Hide text
        if (shownFish != null) Destroy(shownFish);  // Destroy fish
    }
}
