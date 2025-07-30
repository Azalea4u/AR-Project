using System.Collections;
using TMPro;
using UnityEngine;

public class FishCatchCelebrator : MonoBehaviour
{
    [SerializeField] private TMP_Text CelebrationText;
    [SerializeField] private TMP_Text DespairText;

    [SerializeField] private AudioSource fishCaught;
    [SerializeField] private AudioSource fishLost;

    private MashReel mashReel;
    private GameObject shownFish;

    private void Start()
    {
        // This may not be the best way to do this, but it'll work for now!
        mashReel = FishManager.instance.mashReel;

        mashReel.OnFishCaught += Celebrate;
        mashReel.OnFishLost += Despair;

        if (CelebrationText != null) CelebrationText.enabled = false;
        if (DespairText != null) DespairText.enabled = false;
    }

    private void Celebrate()
    {
        if (CelebrationText != null) CelebrationText.enabled = true;  // Show celebratory text
        fishCaught.Play();

        // Uncomment if currentFishData is made public
        // Otherwise, will need to find another way
        GameObject fish = mashReel.currentFishData.fishPrefab;  // Get caught fish gameObject
        
        // GameObject fish = null;

        if (fish != null)
        {
            shownFish = Instantiate(fish);  // Instantiate fish

            shownFish.transform.position = 7 * Camera.main.transform.forward + Camera.main.transform.position;  // Place fish in front of camera

            try
            {
                shownFish.GetComponent<Animator>().enabled = false;  // Disable swimming animation
                shownFish.GetComponent<FishBehavior>().caught = true;  // Turn off normal fish behavior
                shownFish.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, Mathf.Deg2Rad * 420, 0);
            }
            catch { }

        }

        StartCoroutine(StopCelebrating());
    }

    private void Despair()
    {
        if (DespairText != null) DespairText.enabled = true;  // Show despairing text
        fishLost.Play();

        StartCoroutine(StopDespairing());
    }

    private IEnumerator StopCelebrating()
    {
        yield return new WaitForSeconds(4f);  // Wait 5 seconds

        if (CelebrationText != null) CelebrationText.enabled = false;  // Hide text
        if (shownFish != null) Destroy(shownFish);  // Destroy fish
    }

    private IEnumerator StopDespairing()
    {
        yield return new WaitForSeconds(4f);  // Wait 5 seconds

        if (DespairText != null) DespairText.enabled = false;  // Hide text
    }
}
