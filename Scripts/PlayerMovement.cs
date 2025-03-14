using UnityEngine;

public class PlayerMovement: MonoBehaviour
{
    public float jumpForce = 10f;
    public float slamForce = 15f;
    private Rigidbody playerRb;
    private bool isGrounded = true;

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1f);
        HandleJumpInput();
        HandleSlamInput();
    }

    private void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            playerRb.velocity = new Vector3(playerRb.velocity.x, jumpForce, playerRb.velocity.z);
            isGrounded = false;
            Debug.Log("Player jumped!");
        }
    }

    private void HandleSlamInput()
    {
        if (Input.GetKeyDown(KeyCode.S) && !isGrounded)
        {
            playerRb.velocity = new Vector3(playerRb.velocity.x, -slamForce, playerRb.velocity.z);
            Debug.Log("Slam! Player forced downward.");
        }
    }

    public bool IsGrounded() => isGrounded;
    public Rigidbody GetRigidbody() => playerRb;
}