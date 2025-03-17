using UnityEngine;
using System.Collections;

public class CloudController : MonoBehaviour
{
    public GameObject cloudPrefab;
    public float cloudLaunchSpeed = 20f;
    [HideInInspector] public GameObject currentCloud;
    private Camera mainCamera;
    private Rigidbody playerRb;

    void Start()
    {
        mainCamera = Camera.main;
        playerRb = GetComponent<Rigidbody>();
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found! Cloud system requires a camera.");
        }
    }

    void Update()
    {
        HandleTestCloudInput();
        HandleCursorInput();
        ValidateCurrentCloud();
    }

    public void SpawnCloudAboveTarget(GameObject target)
    {
        if (target == null) return;
        
        Debug.Log($"Spawning Cloud above target: {target.name} (Position: {target.transform.position})");
        currentCloud = InstantiateCloud(target.transform.position + Vector3.up * 3f);
        currentCloud.GetComponent<CloudBehavior>().target = target;
        currentCloud.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        Debug.Log("Cloud spawned and assigned to currentCloud!");
    }

    public void GrowCurrentCloud()
    {
        if (currentCloud != null)
        {
            Vector3 currentScale = currentCloud.transform.localScale;
            currentScale += new Vector3(0.1f, 0.1f, 0.1f);
            currentCloud.transform.localScale = currentScale;
        }
    }

    private void HandleTestCloudInput()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Spawning test Cloud near Player");
            GameObject testCloud = InstantiateCloud(transform.position + Vector3.up * 3f + Vector3.right * 2f);
            testCloud.GetComponent<CloudBehavior>().target = gameObject;
            testCloud.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            Debug.Log("Test Cloud spawned!");
        }
    }

    private void HandleCursorInput()
    {
        if (Input.GetMouseButtonDown(0) && mainCamera != null)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                CloudBehavior cloud = hit.collider.GetComponent<CloudBehavior>();
                if (cloud != null)
                {
                    Debug.Log($"Activated cloud: {hit.collider.gameObject.name}");
                    cloud.ActivateCloud(2f);
                }
            }
        }
    }

    public void LaunchCloud()
    {
        if (currentCloud == null)
        {
            Debug.Log("No current Cloud!");
            return;
        }

        CloudBehavior cloudBehavior = currentCloud.GetComponent<CloudBehavior>();
        if (cloudBehavior == null || cloudBehavior.target != gameObject)
        {
            Debug.LogWarning("Invalid cloud state for launch!");
            return;
        }

        Rigidbody cloudRb = currentCloud.GetComponent<Rigidbody>();
        if (cloudRb == null)
        {
            Debug.LogWarning("No Rigidbody on Cloud!");
            return;
        }

        cloudBehavior.PrepareForLaunch();
        cloudRb.isKinematic = false;
        cloudRb.useGravity = true;

        Vector3 launchDirection = playerRb.velocity.normalized;
        if (launchDirection.magnitude < 0.1f)
        {
            launchDirection = transform.forward.normalized;
        }
        launchDirection += Vector3.up * 0.05f;
        launchDirection = launchDirection.normalized;

        float adjustedLaunchSpeed = cloudLaunchSpeed * 3f;
        cloudRb.velocity = launchDirection * adjustedLaunchSpeed;

        Debug.Log($"Launch Direction: {launchDirection}, Velocity set to: {cloudRb.velocity}");
        cloudBehavior.Launch();
        StartCoroutine(LogPositionAfterLaunch(currentCloud));
        currentCloud = null;
    }

    private GameObject InstantiateCloud(Vector3 position)
    {
        GameObject cloud = Instantiate(cloudPrefab, position, Quaternion.identity);
        Rigidbody cloudRb = cloud.AddComponent<Rigidbody>();
        cloudRb.isKinematic = true;
        cloudRb.useGravity = false;
        cloudRb.constraints = RigidbodyConstraints.FreezeRotationX | 
                            RigidbodyConstraints.FreezeRotationY | 
                            RigidbodyConstraints.FreezeRotationZ;
        cloud.tag = "Cloud";
        cloud.AddComponent<CloudBehavior>();
        return cloud;
    }

    private void ValidateCurrentCloud()
    {
        if (currentCloud != null && currentCloud.GetComponent<CloudBehavior>() == null)
        {
            Debug.Log("Clearing currentCloud because CloudBehavior is missing!");
            currentCloud = null;
        }
    }

    private IEnumerator LogPositionAfterLaunch(GameObject cloud)
    {
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(0.5f);
            if (cloud != null)
            {
                Debug.Log("Cloud position after launch: " + cloud.transform.position);
            }
        }
    }
}
