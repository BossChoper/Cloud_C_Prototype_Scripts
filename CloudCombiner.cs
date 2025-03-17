using UnityEngine;

public class CloudCombiner : MonoBehaviour
{
    public GameObject bigCloudPrefab;
    public float combineRadius = 2f;
    private bool hasCombined = false;
    private CloudCore cloudCore;
    private GameObject player;

    void Start()
    {
        cloudCore = GetComponent<CloudCore>();
        player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        if (cloudCore.target == player && !hasCombined)
        {
            CombineWithNearbyCloud();
        }
    }

    private void CombineWithNearbyCloud()
    {
        Collider[] nearbyClouds = Physics.OverlapSphere(transform.position, combineRadius);
        foreach (Collider col in nearbyClouds)
        {
            if (col.gameObject != gameObject && col.CompareTag("Cloud"))
            {
                CloudCombiner otherCloud = col.GetComponent<CloudCombiner>();
                if (otherCloud != null && !otherCloud.hasCombined && 
                    otherCloud.GetComponent<CloudCore>().target != player)
                {
                    CreateBigCloud(otherCloud);
                    return;
                }
            }
        }
    }

    private void CreateBigCloud(CloudCombiner otherCloud)
    {
        hasCombined = true;
        otherCloud.hasCombined = true;

        Vector3 midPoint = (transform.position + otherCloud.transform.position) / 2;
        GameObject bigCloud = Instantiate(bigCloudPrefab, midPoint, Quaternion.identity);
        
        CloudCore bigCloudCore = bigCloud.GetComponent<CloudCore>();
        if (bigCloudCore != null)
        {
            bigCloudCore.target = player;
        }

        CloudInkSystem bigCloudInk = bigCloud.GetComponent<CloudInkSystem>();
        if (bigCloudInk != null)
        {
            bigCloudInk.rainInterval = 0.5f;
        }

        AssignToPlayer(bigCloud);
        Destroy(otherCloud.gameObject);
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Cloud") && !hasCombined)
        {
            CloudCombiner otherScript = collision.gameObject.GetComponent<CloudCombiner>();
            if (otherScript != null)
            {
                CreateBigCloud(otherScript);
            }
        }
    }

    private void AssignToPlayer(GameObject cloud)
    {
        PlayerCombat playerCombat = player.GetComponent<PlayerCombat>();
        if (playerCombat != null)
        {
            playerCombat.currentCloud = cloud;
            Debug.Log("Big Cloud assigned to Player!");
        }
    }

    public bool HasCombined() => hasCombined;
}
