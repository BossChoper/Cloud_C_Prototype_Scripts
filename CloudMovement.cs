using UnityEngine;

public class CloudMovement : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 3f, 0);
    public float followSpeed = 10f;
    public float verticalFollowSpeed = 2f;
    public float hoverForce = 9.8f;
    public float maxLaunchHeight = 5f;
    public float launchDistanceLimit = 10f;

    private CloudCore cloudCore;
    private Vector3 launchStartPosition;
    private float launchTargetLockoutDuration = 1f;
    private float timeSinceLaunch = 0f;
    private bool canReTarget = false;

    void Start()
    {
        cloudCore = GetComponent<CloudCore>();
        if (gameObject.name.Contains("BigCloud"))
        {
            followSpeed = 3f;
            verticalFollowSpeed = 1f;
        }
    }

    void Update()
    {
        if (cloudCore.IsLaunched())
        {
            HandleLaunchedState();
        }
        else if (cloudCore.target != null && !cloudCore.IsPreparingLaunch())
        {
            FollowTarget();
        }
    }

    private void HandleLaunchedState()
    {
        timeSinceLaunch += Time.deltaTime;

        if (timeSinceLaunch >= launchTargetLockoutDuration)
        {
            canReTarget = true;
        }

        ApplyHoverForce(0.5f);
        ConstrainLaunchTrajectory();

        if (canReTarget)
        {
            CheckForTargetBelow();
        }
    }

    public void InitializeLaunch(Vector3 startPos)
    {
        launchStartPosition = startPos;
        timeSinceLaunch = 0f;
        canReTarget = false;
    }

    private void ApplyHoverForce(float multiplier = 1f)
    {
        var rb = cloudCore.GetRigidbody();
        if (rb != null)
        {
            rb.AddForce(Vector3.up * hoverForce * multiplier, ForceMode.Acceleration);
            Vector3 velocity = rb.velocity;
            velocity.y *= 0.98f;
            rb.velocity = velocity;
        }
    }

    private void ConstrainLaunchTrajectory()
    {
        var rb = cloudCore.GetRigidbody();
        if (rb == null) return;

        if (transform.position.y > launchStartPosition.y + maxLaunchHeight)
        {
            Vector3 velocity = rb.velocity;
            velocity.y = Mathf.Min(velocity.y, 0);
            rb.velocity = velocity;
            transform.position = new Vector3(transform.position.x, 
                                          launchStartPosition.y + maxLaunchHeight, 
                                          transform.position.z);
        }

        float distanceTraveled = Vector3.Distance(launchStartPosition, transform.position);
        if (timeSinceLaunch > 1f || distanceTraveled > launchDistanceLimit)
        {
            Vector3 velocity = rb.velocity;
            Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);
            horizontalVelocity *= 0.7f;
            rb.velocity = new Vector3(horizontalVelocity.x, velocity.y, horizontalVelocity.z);
        }
    }

    private void FollowTarget()
    {
        Vector3 targetPos = cloudCore.target.transform.position;
        Vector3 desiredPos = targetPos + offset + new Vector3(0, Mathf.Sin(Time.time * 5f) * 0.2f, 0);
        Vector3 currentPos = transform.position;

        float horizontalLerpFactor = followSpeed * Time.deltaTime;
        float verticalLerpFactor = verticalFollowSpeed * Time.deltaTime;
        float newX = Mathf.Lerp(currentPos.x, desiredPos.x, horizontalLerpFactor);
        float newZ = Mathf.Lerp(currentPos.z, desiredPos.z, horizontalLerpFactor);
        float newY = Mathf.Lerp(currentPos.y, desiredPos.y, verticalLerpFactor);

        transform.position = new Vector3(newX, newY, newZ);
    }

    private void CheckForTargetBelow()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 3.5f))
        {
            GameObject hitObject = hit.collider.gameObject;
            if (hitObject.CompareTag("Player") || hitObject.CompareTag("Enemy"))
            {
                cloudCore.target = hitObject;
                cloudCore.SetLingerTimer(3f);
                cloudCore.SetColor(cloudCore.GetOriginalColor());
                Debug.Log($"Cloud now following {hitObject.tag}!");
            }
        }
    }
}
