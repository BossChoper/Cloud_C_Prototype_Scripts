using UnityEngine;

public class SuperpowerSystem : MonoBehaviour
{
    private bool hasSuperpower = false;
    private float superpowerDuration = 10f;
    private float superpowerTimer = 0f;
    private Color superpowerColor = Color.red;
    private Renderer playerRenderer;
    private Color originalColor;

    void Start()
    {
        playerRenderer = GetComponent<Renderer>();
        if (playerRenderer != null) originalColor = playerRenderer.material.color;
    }

    void Update()
    {
        if (hasSuperpower)
        {
            superpowerTimer -= Time.deltaTime;
            if (superpowerTimer <= 0) DeactivateSuperpower();
        }
    }

    public void ActivateSuperpower()
    {
        hasSuperpower = true;
        superpowerTimer = superpowerDuration;
        if (playerRenderer != null) playerRenderer.material.color = superpowerColor;
        Debug.Log("Superpower activated!");
    }

    private void DeactivateSuperpower()
    {
        hasSuperpower = false;
        if (playerRenderer != null) playerRenderer.material.color = originalColor;
        Debug.Log("Superpower deactivated!");
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