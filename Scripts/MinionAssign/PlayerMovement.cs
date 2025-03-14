using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Movement
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveHorizontal, 0f, moveVertical).normalized;
        rb.velocity = movement * speed;

        // Assign Minions with mouse click
        if (Input.GetMouseButtonDown(0)) // Left-click
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("CarryObject"))
                {
                    CarryTask task = hit.collider.GetComponent<CarryTask>();
                    if (task != null)
                    {
                        task.AssignMinion();
                    }
                }
            }
        }

        // Recall Minions with 'R' key
        if (Input.GetKeyDown(KeyCode.R))
        {
            RecallMinions();
        }
    }

    private void RecallMinions()
    {
        GameObject[] carryObjects = GameObject.FindGameObjectsWithTag("CarryObject");
        foreach (GameObject carryObj in carryObjects)
        {
            CarryTask task = carryObj.GetComponent<CarryTask>();
            if (task != null)
            {
                task.RecallAssignedMinions(); // Call a new method to free Minions
            }
        }
    }
}