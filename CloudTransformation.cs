using UnityEngine;

public class CloudTransformation : MonoBehaviour
{
    public GameObject stormCloudPrefab;
    public GameObject superpowerPickupPrefab;
    public float proximityDistance = 5f;
    private CloudCore cloudCore;
    private GameObject player;

    void Start()
    {
        cloudCore = GetComponent<CloudCore>();
        player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        if (cloudCore.target != null && cloudCore.target.CompareTag("Enemy"))
        {
            CheckEnemyProximity();
        }
    }

    private void CheckEnemyProximity()
    {
        if (cloudCore.target == null) return;

        float distance = Vector3.Distance(transform.position, cloudCore.target.transform.position);
        if (distance <= proximityDistance)
        {
            TransformCloudOnEnemyProximity();
        }
    }

    private void TransformCloudOnEnemyProximity()
    {
        if (stormCloudPrefab == null)
        {
            Debug.LogWarning("StormCloudPrefab not assigned! Cannot transform cloud.");
            return;
        }

        GameObject enemyToDestroy = cloudCore.target;
        cloudCore.target = null;
        if (enemyToDestroy != null)
        {
            Destroy(enemyToDestroy);
        }

        SpawnStormCloud();
        SpawnSuperpowerPickup();
        UpdatePlayerReference();
        
        Destroy(gameObject);
    }

    private void SpawnStormCloud()
    {
        Vector3 cloudPosition = transform.position;
        GameObject stormCloud = Instantiate(stormCloudPrefab, cloudPosition, transform.rotation);
        
        CloudCore stormCloudCore = stormCloud.GetComponent<CloudCore>();
        if (stormCloudCore != null)
        {
            stormCloudCore.target = null;
        }

        CloudInkSystem stormCloudInk = stormCloud.GetComponent<CloudInkSystem>();
        if (stormCloudInk != null)
        {
            stormCloudInk.rainInterval = 0.3f;
        }
    }

    private void SpawnSuperpowerPickup()
    {
        if (superpowerPickupPrefab == null)
        {
            Debug.LogWarning("SuperpowerPickupPrefab not assigned! No pickup spawned.");
            return;
        }

        Vector2 randomOffset = Random.insideUnitCircle * 2f;
        Vector3 pickupPosition = new Vector3(
            transform.position.x + randomOffset.x,
            transform.position.y + 1f,
            transform.position.z + randomOffset.y
        );
        
        Instantiate(superpowerPickupPrefab, pickupPosition, Quaternion.identity);
        Debug.Log($"Superpower pickup spawned at: {pickupPosition}");
    }

    private void UpdatePlayerReference()
    {
        if (player != null)
        {
            PlayerCombat playerCombat = player.GetComponent<PlayerCombat>();
            if (playerCombat != null && playerCombat.currentCloud == gameObject)
            {
                playerCombat.currentCloud = null;
            }
        }
    }
}
