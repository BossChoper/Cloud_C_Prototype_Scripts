using UnityEngine;

public class CloudController : MonoBehaviour
{
    public GameObject cloudPrefab;
    public float cloudLaunchSpeed = 20f;
    private GameObject currentCloud;
    private PlayerMovement playerMovement;
    private Camera mainCamera;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        HandleCloudLaunch();
        HandleCursorInput();
        ValidateCurrentCloud();
    }

    public void SpawnCloudAboveEnemy(GameObject enemy)
    {
        currentCloud = InstantiateCloud(enemy.transform.position + Vector3.up * 3f);
        currentCloud.GetComponent<CloudBehavior>().target = enemy;
        currentCloud.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        Debug.Log("Cloud spawned above enemy!");
    }

    private GameObject InstantiateCloud(Vector3 position)
    {
        GameObject cloud = Instantiate(cloudPrefab, position, Quaternion.identity);
        Rigidbody cloudRb = cloud.AddComponent<Rigidbody>();
        cloudRb.isKinematic = true;
        cloudRb.useGravity = false;
        cloudRb.constraints = RigidbodyConstraints.FreezeRotation;
        cloud.tag = "Cloud";
        cloud.AddComponent<CloudBehavior>();
        return cloud;
    }

    private void HandleCloudLaunch()
    {
        if (Input.GetKeyDown(KeyCode.Q) && currentCloud != null)
        {
            CloudBehavior cloudBehavior = currentCloud.GetComponent<CloudBehavior>();
            if (cloudBehavior.target == gameObject)
            {
                Rigidbody cloudRb = currentCloud.GetComponent<Rigidbody>();
                cloudBehavior.PrepareForLaunch();
                cloudRb.isKinematic = false;
                cloudRb.useGravity = true;

                Vector3 launchDirection = playerMovement.GetRigidbody().velocity.normalized;
                if (launchDirection.magnitude < 0.1f) launchDirection = transform.forward;
                launchDirection += Vector3.up * 0.05f;
                cloudRb.velocity = launchDirection.normalized * cloudLaunchSpeed * 3f;

                cloudBehavior.Launch();
                currentCloud = null;
                Debug.Log("Cloud launched!");
            }
        }
    }

    private void HandleCursorInput()
    {
        // To be changed; raycast should be activated on mouse position, not click
        if (Input.GetMouseButtonDown(0) && mainCamera != null)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                CloudBehavior cloud = hit.collider.GetComponent<CloudBehavior>();
                if (cloud != null)
                {
                    cloud.ActivateCloud(2f);
                    Debug.Log($"Activated cloud: {hit.collider.gameObject.name}");
                }
            }
        }
    }

    private void ValidateCurrentCloud()
    {
        if (currentCloud != null && currentCloud.GetComponent<CloudBehavior>() == null)
        {
            currentCloud = null;
            Debug.Log("Cleared currentCloud due to missing CloudBehavior!");
        }
    }
}