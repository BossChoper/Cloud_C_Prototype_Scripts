using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Optional: If you want to keep the original movement method
    public float speed = 10f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // If using Rigidbody velocity from shooter, we don't need Update movement
        // Remove this if you're using the Shooter's velocity system
        // transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}