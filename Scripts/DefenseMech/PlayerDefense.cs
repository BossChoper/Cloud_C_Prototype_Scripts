using UnityEngine;

public class PlayerDefense : MonoBehaviour
{
    // Block variables
    private int blockHitsRemaining = 3;
    private bool isBlocking = false;
    public GameObject blockEffectPrefab; // Optional: visual effect for blocking

    // Parry variables
    private bool isParrying = false;
    private float parryWindow = 0.5f;    // 0.5 second parry window
    private float parryTimer = 0f;
    public float parryForce = 10f;       // Force to reflect projectile
    public GameObject parryEffectPrefab; // Optional: visual effect for parrying

    // Input keys (customize as needed)
    public KeyCode blockKey = KeyCode.Mouse1;  // Right mouse button
    public KeyCode parryKey = KeyCode.F;

    void Update()
    {
        // Handle block input
        if (Input.GetKeyDown(blockKey) && blockHitsRemaining > 0)
        {
            StartBlocking();
        }
        if (Input.GetKeyUp(blockKey))
        {
            StopBlocking();
        }

        // Handle parry input
        if (Input.GetKeyDown(parryKey) && !isBlocking)
        {
            StartParry();
        }

        // Update parry timer
        if (isParrying)
        {
            parryTimer -= Time.deltaTime;
            if (parryTimer <= 0)
            {
                StopParry();
            }
        }
    }

    void StartBlocking()
    {
        isBlocking = true;
        // Add animation trigger here if you have one
        Debug.Log("Blocking started");
    }

    void StopBlocking()
    {
        isBlocking = false;
        // Reset animation if needed
        Debug.Log("Blocking stopped");
    }

    void StartParry()
    {
        isParrying = true;
        parryTimer = parryWindow;
        // Add animation trigger here if you have one
        Debug.Log("Parry started");
    }

    void StopParry()
    {
        isParrying = false;
        // Reset animation if needed
        Debug.Log("Parry ended");
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            if (isParrying)
            {
                ReflectProjectile(collision);
            }
            else if (isBlocking)
            {
                BlockProjectile(collision);
            }
            else
            {
                // Normal hit logic here if neither blocking nor parrying
                Debug.Log("Player hit by projectile!");
            }
        }
    }

    void BlockProjectile(Collision collision)
    {
        blockHitsRemaining--;
        
        // Spawn block effect if assigned
        if (blockEffectPrefab != null)
        {
            Instantiate(blockEffectPrefab, collision.contacts[0].point, Quaternion.identity);
        }

        // Destroy the projectile
        Destroy(collision.gameObject);

        Debug.Log($"Block successful! Hits remaining: {blockHitsRemaining}");

        if (blockHitsRemaining <= 0)
        {
            StopBlocking();
            Debug.Log("Block broken!");
        }
    }

    void ReflectProjectile(Collision collision)
    {
        // Get the projectile's Rigidbody
        Rigidbody projectileRb = collision.gameObject.GetComponent<Rigidbody>();
        if (projectileRb != null)
        {
            // Calculate reflection direction (back toward where it came from)
            Vector3 reflectionDirection = -collision.relativeVelocity.normalized;
            
            // Apply force to reflect
            projectileRb.velocity = reflectionDirection * parryForce;

            // Spawn parry effect if assigned
            if (parryEffectPrefab != null)
            {
                Instantiate(parryEffectPrefab, collision.contacts[0].point, Quaternion.identity);
            }

            Debug.Log("Projectile parried!");
        }
        
        StopParry();
    }

    // Call this to reset block (e.g., after a cooldown or level reset)
    public void ResetBlock()
    {
        blockHitsRemaining = 3;
    }
}