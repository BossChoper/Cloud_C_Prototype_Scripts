using UnityEngine;

public class PlayerTeleport : MonoBehaviour
{
    public GameObject teleportSpherePrefab; // Assign the sphere prefab in the Inspector
    public float sphereSpeed = 10f; // Speed of the sphere
    public Transform sphereSpawnPoint; // Where the sphere spawns (e.g., in front of player)
    
    private GameObject currentSphere; // Track the current sphere
    private bool canTeleport = false; // Flag to enable teleportation
    private Vector3 teleportPosition; // Where to teleport

    void Update()
    {
        // Shoot sphere on key press (e.g., Spacebar)
        if (Input.GetKeyDown(KeyCode.Space) && !canTeleport)
        {
            ShootSphere();
        }

        // Teleport when sphere lands (e.g., on Mouse Click)
        if (canTeleport && Input.GetMouseButtonDown(0))
        {
            Teleport();
        }
    }

    void ShootSphere()
    {
        // If there's an existing sphere, destroy it
        if (currentSphere != null)
        {
            Destroy(currentSphere);
        }

        // Instantiate the sphere at the spawn point
        currentSphere = Instantiate(teleportSpherePrefab, sphereSpawnPoint.position, Quaternion.identity);
        Rigidbody rb = currentSphere.GetComponent<Rigidbody>();
        
        // Shoot the sphere forward
        Vector3 direction = sphereSpawnPoint.forward; // Use spawn point's forward direction
        rb.velocity = direction * sphereSpeed;
    }

    public void SetTeleportPosition(Vector3 position)
    {
        teleportPosition = position;
        canTeleport = true;
        currentSphere = null;
    }

    void Teleport()
    {
        transform.position = teleportPosition; // Move player to sphere's landing spot
        canTeleport = false; // Reset flag
    }
}