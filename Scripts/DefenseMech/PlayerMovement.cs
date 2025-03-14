using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Movement variables
    public float moveSpeed = 5f;
    public float rotationSpeed = 720f; // Degrees per second
    private CharacterController characterController;
    private Vector3 moveDirection;

    // Camera reference
    public Camera mainCamera;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        // Get input
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        // Calculate movement direction relative to camera
        Vector3 forward = mainCamera.transform.forward;
        Vector3 right = mainCamera.transform.right;
        
        // Keep movement on horizontal plane
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // Calculate move direction
        moveDirection = (forward * verticalInput + right * horizontalInput).normalized;

        // Move the player
        if (moveDirection != Vector3.zero)
        {
            // Rotate towards movement direction
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, 
                targetRotation, 
                rotationSpeed * Time.deltaTime
            );

            // Move using CharacterController
            characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
        }
    }
}