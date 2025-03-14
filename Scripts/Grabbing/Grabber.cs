using UnityEngine;

public class Grabber : MonoBehaviour
{
    private GameObject heldObject = null; // The object being held
    private float grabDistance = 2f;      // Max distance to grab
    private float holdDistance = 1.5f;    // Distance to hold object in front
    private Camera cam;

    void Start()
    {
        cam = GetComponentInChildren<Camera>(); // Get the child camera
    }

    void Update()
    {
        // Press E to grab or drop
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObject == null) // Not holding anything, try to grab
            {
                TryGrab();
            }
            else // Holding something, drop it
            {
                Drop();
            }
        }

        // If holding an object, keep it in front of the camera
        if (heldObject != null)
        {
            HoldObject();
        }
    }

    void TryGrab()
    {
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)); // Center of screen
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, grabDistance))
        {
            if (hit.collider.gameObject.GetComponent<Rigidbody>() != null) // Check if it’s a physics object
            {
                heldObject = hit.collider.gameObject;
                heldObject.GetComponent<Rigidbody>().isKinematic = true; // Disable physics while held
            }
        }
    }

    void Drop()
    {
        heldObject.GetComponent<Rigidbody>().isKinematic = false; // Re-enable physics
        heldObject = null;
    }

    void HoldObject()
    {
        Vector3 holdPosition = cam.transform.position + cam.transform.forward * holdDistance;
        heldObject.transform.position = holdPosition;
    }
}