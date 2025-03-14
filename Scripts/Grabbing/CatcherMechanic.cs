using UnityEngine;

public class CatcherMechanic : MonoBehaviour
{
    private GameObject caughtTarget = null;     // The target being caught
    public float catchDistance = 2f;            // Distance at which catching occurs

    void Update()
    {
        // If we’ve caught something, keep it attached
        if (caughtTarget != null)
        {
            caughtTarget.transform.position = transform.position + Vector3.up * 1.5f; // Hold above Catcher
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Grabbable") && caughtTarget == null)
        {
            float distance = Vector3.Distance(transform.position, other.transform.position);
            if (distance <= catchDistance)
            {
                CatchTarget(other.gameObject);
            }
        }
    }

    void CatchTarget(GameObject target)
    {
        caughtTarget = target;
        Rigidbody targetRb = target.GetComponent<Rigidbody>();
        targetRb.velocity = Vector3.zero;           // Stop its momentum
        targetRb.isKinematic = true;                // Disable physics
        target.transform.parent = transform;        // Attach to Catcher
    }
}