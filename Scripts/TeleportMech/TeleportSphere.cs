using UnityEngine;

public class TeleportSphere : MonoBehaviour
{
    private PlayerTeleport playerTeleport;

    void Start()
    {
        playerTeleport = FindObjectOfType<PlayerTeleport>();
    }

    void OnCollisionEnter(Collision collision)
    {
        // Ignore collisions with the player
        if (collision.gameObject.GetComponent<PlayerTeleport>() == null)
        {
            playerTeleport.SetTeleportPosition(transform.position);
            Destroy(gameObject);
        }
    }
} 