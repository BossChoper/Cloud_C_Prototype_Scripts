using UnityEngine;

public class PickupBobbing : MonoBehaviour
{
    public float bobHeight = 0.5f; // Height of up-and-down motion
    public float bobSpeed = 2f; // Speed of bobbing

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}