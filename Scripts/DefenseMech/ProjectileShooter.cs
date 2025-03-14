using UnityEngine;

public class ProjectileShooter : MonoBehaviour
{
    // Shooting variables
    public GameObject projectilePrefab;
    public Transform firePoint;         // Where projectiles spawn
    public float shootForce = 10f;
    public float shootCooldown = 0.5f;  // Time between shots
    private float shootTimer = 0f;

    // Input
    public KeyCode shootKey = KeyCode.Mouse0; // Left mouse button

    void Update()
    {
        // Update cooldown timer
        if (shootTimer > 0)
        {
            shootTimer -= Time.deltaTime;
        }

        // Handle shooting input
        if (Input.GetKeyDown(shootKey) && shootTimer <= 0)
        {
            Shoot();
            shootTimer = shootCooldown;
        }
    }

    void Shoot()
    {
        // Instantiate projectile
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        
        // Add force to projectile
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = firePoint.forward * shootForce;
        }

        // Optional: Destroy projectile after some time
        Destroy(projectile, 5f);
        
        Debug.Log("Projectile fired!");
    }
}