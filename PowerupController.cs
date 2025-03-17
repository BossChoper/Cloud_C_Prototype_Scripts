using UnityEngine;

public class PowerupController : MonoBehaviour
{
    public float superpowerDuration = 10f;
    private bool hasSuperpower = false;
    private float superpowerTimer = 0f;
    private Color superpowerColor = Color.red;
    private Color originalColor;
    private Renderer playerRenderer;

    void Start()
    {
        playerRenderer = GetComponent<Renderer>();
        if (playerRenderer != null)
        {
            originalColor = playerRenderer.material.color;
        }
        else
        {
            Debug.LogWarning("Player Renderer not found! Superpower color change won't work.");
        }
    }

    void Update()
    {
        if (hasSuperpower)
        {
            superpowerTimer -= Time.deltaTime;
            if (superpowerTimer <= 0)
            {
                DeactivateSuperpower();
            }
        }
    }

    public void ActivateSuperpower()
    {
        hasSuperpower = true;
        superpowerTimer = superpowerDuration;
        if (playerRenderer != null)
        {
            playerRenderer.material.color = superpowerColor;
        }
        Debug.Log("Superpower activated! Player color changed to red.");
    }

    private void DeactivateSuperpower()
    {
        hasSuperpower = false;
        superpowerTimer = 0f;
        if (playerRenderer != null)
        {
            playerRenderer.material.color = originalColor;
        }
        Debug.Log("Superpower deactivated! Player color reverted.");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SuperpowerPickup"))
        {
            ActivateSuperpower();
            Destroy(other.gameObject);
            Debug.Log("Collected Superpower Pickup!");
        }
    }
}
