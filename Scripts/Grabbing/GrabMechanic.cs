using UnityEngine;

public class GrabMechanic : MonoBehaviour
{
    private GameObject target;              // The target to grab
    private GameObject catcher;             // The catcher to aim at
    private bool isGrabbing = false;        // Are we grabbing?
    public bool isLeaping = false;          // Public for DynamicCamera
    public float grabDistance = 2f;         // Distance to grab
    public float leapHeight = 5f;           // Height of the leap
    public float leapDuration = 1f;         // Time for leap up and down
    public float tossForce = 15f;           // Force to launch target toward catcher
    private float leapTimer = 0f;           // Timer for leap sequence
    private Vector3 startPosition;          // Starting position for leap
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        catcher = GameObject.FindWithTag("Catcher"); // Find the Catcher at start
    }

    void Update()
    {
        // Grab with E key
        if (Input.GetKeyDown(KeyCode.E) && !isGrabbing && !isLeaping && target != null)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance <= grabDistance)
            {
                StartGrabAndLeap();
            }
        }

        // Handle leap sequence
        if (isLeaping)
        {
            leapTimer += Time.deltaTime;
            float progress = leapTimer / leapDuration;

            if (progress < 0.5f) // Going up
            {
                float height = Mathf.Lerp(0f, leapHeight, progress * 2f);
                transform.position = new Vector3(transform.position.x, startPosition.y + height, transform.position.z);

                // Rotate toward Catcher while ascending
                if (catcher != null)
                {
                    Vector3 direction = (catcher.transform.position - transform.position).normalized;
                    direction.y = 0; // Keep rotation on XZ plane
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
                }
            }
            else if (progress < 1f) // Coming down
            {
                float height = Mathf.Lerp(leapHeight, 0f, (progress - 0.5f) * 2f);
                transform.position = new Vector3(transform.position.x, startPosition.y + height, transform.position.z);
            }
            else // Landed
            {
                EndLeapAndToss();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Grabbable"))
        {
            target = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Grabbable"))
        {
            target = null;
        }
    }

    void StartGrabAndLeap()
    {
        isGrabbing = true;
        isLeaping = true;
        leapTimer = 0f;
        startPosition = transform.position;

        // Attach target to player
        target.transform.parent = transform;
        target.GetComponent<Rigidbody>().isKinematic = true;
    }

    void EndLeapAndToss()
    {
        isLeaping = false;
        isGrabbing = false;

        // Detach and toss target toward Catcher
        Rigidbody targetRb = target.GetComponent<Rigidbody>();
        target.transform.parent = null;
        targetRb.isKinematic = false;

        if (catcher != null)
        {
            Vector3 tossDirection = (catcher.transform.position - transform.position).normalized;
            targetRb.AddForce(tossDirection * tossForce, ForceMode.Impulse);
        }
        else
        {
            // Fallback: toss forward if no Catcher
            targetRb.AddForce((transform.forward + Vector3.up) * tossForce, ForceMode.Impulse);
        }

        target = null;
    }
}