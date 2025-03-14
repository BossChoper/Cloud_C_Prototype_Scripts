using UnityEngine;

public class MinionAI : MonoBehaviour
{
    public Transform target; // The Player
    public float speed = 3f;
    public float followDistance = 2f;
    public float taskDistance = 5f;
    private GameObject currentTask; // Resource or CarryObject
    private bool isAssignedToCarry = false; // Is this Minion assigned to a CarryObject?
    private CarryTask carryTask; // Reference to the CarryTask script

    // Public property to check if the Minion is assigned to a carry task
    public bool IsAssignedToCarry => isAssignedToCarry;

    void Start()
    {
        if (target == null)
            target = GameObject.Find("Player").transform;
    }

    void Update()
    {
        if (isAssignedToCarry && carryTask != null)
        {
            HandleCarryTask();
        }
        else
        {
            // Find a Resource if not assigned to a CarryObject
            float playerDistance = Vector3.Distance(transform.position, target.position);
            if (currentTask == null)
            {
                GameObject[] resources = GameObject.FindGameObjectsWithTag("Resource");
                float closestDist = Mathf.Infinity;
                foreach (GameObject resource in resources)
                {
                    float dist = Vector3.Distance(transform.position, resource.transform.position);
                    if (dist < closestDist && dist < taskDistance)
                    {
                        closestDist = dist;
                        currentTask = resource;
                    }
                }
            }

            if (currentTask != null && playerDistance <= taskDistance)
            {
                Vector3 direction = (currentTask.transform.position - transform.position).normalized;
                transform.position += direction * speed * Time.deltaTime;
                if (Vector3.Distance(transform.position, currentTask.transform.position) < 0.5f)
                {
                    Destroy(currentTask);
                    currentTask = null;
                }
            }
            else if (playerDistance > followDistance)
            {
                Vector3 direction = (target.position - transform.position).normalized;
                transform.position += direction * speed * Time.deltaTime;
            }
        }
    }

    public void AssignToCarryTask(GameObject taskObject)
    {
        currentTask = taskObject;
        carryTask = taskObject.GetComponent<CarryTask>();
        isAssignedToCarry = true;
    }

    public void ResetTask()
    {
        isAssignedToCarry = false;
        currentTask = null;
        carryTask = null;
    }

    private void HandleCarryTask()
    {
        if (carryTask.IsBeingCarried())
        {
            // Move toward the Base
            Vector3 basePos = GameObject.Find("Base").transform.position;
            Vector3 direction = (basePos - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            // Check if close to Base
            if (Vector3.Distance(transform.position, basePos) < 0.5f)
            {
                carryTask.DropAtBase();
            }
        }
        else
        {
            // Move toward the CarryObject
            Vector3 direction = (currentTask.transform.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            // Check if close enough to start carrying
            if (Vector3.Distance(transform.position, currentTask.transform.position) < 0.5f)
            {
                carryTask.AttachMinion(this);
            }
        }
    }
}