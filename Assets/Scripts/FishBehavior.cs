using System.Collections;
using UnityEngine;

public class FishBehavior : MonoBehaviour
{
    [SerializeField] private Vector3 origin;
    
    private Vector3 direction;
    private Vector3 rotation;

    private Rigidbody rb;

    [Range(0.1f, 3)]
    public float speed = 1;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        direction = transform.forward;
        rotation = Vector3.zero;

        StartCoroutine(Turn());
    }

    private void FixedUpdate()
    {
        rb.angularVelocity -= rb.angularVelocity * Time.fixedDeltaTime;
        rb.linearVelocity -= rb.linearVelocity * Time.fixedDeltaTime;
    }

    private IEnumerator Turn()
    {
        switch (Random.Range(-4, 4))
        {
            case -4: rotation = new Vector3(0, Mathf.Deg2Rad * -180, 0); break;
            case -3: rotation = new Vector3(0, Mathf.Deg2Rad * -135, 0); break;
            case -2: rotation = new Vector3(0, Mathf.Deg2Rad * -90, 0); break;
            case -1: rotation = new Vector3(0, Mathf.Deg2Rad * -45, 0); break;
            case 0: rotation = new Vector3(0, 0, 0); break;
            case 1: rotation = new Vector3(0, Mathf.Deg2Rad * 45, 0); break;
            case 2: rotation = new Vector3(0, Mathf.Deg2Rad * 90, 0); break;
            case 3: rotation = new Vector3(0, Mathf.Deg2Rad * 135, 0); break;
            case 4: rotation = new Vector3(0, Mathf.Deg2Rad * 180, 0); break;
            default: break;
        }

        rb.angularVelocity = rotation;

        yield return new WaitForSeconds(1.1f);
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        switch (Random.Range(1, 4))
        {
            case 1: direction = 0.5f * speed * transform.forward; break;
            case 2: direction =    1 * speed * transform.forward; break;
            case 3: direction =    2 * speed * transform.forward; break;
            case 4: direction =    3 * speed * transform.forward; break;
            default: break;
        }

        rb.linearVelocity = direction;

        yield return new WaitForSeconds(4 / speed);
        StartCoroutine(Turn());
    }
}
