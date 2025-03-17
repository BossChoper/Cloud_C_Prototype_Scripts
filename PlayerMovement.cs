using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour {
    public float speed = 5f;
    public float inkSpeedBoost = 2f;
    private float currentSpeed;
    public TextMeshProUGUI comboText;
    public float jumpForce = 5f;       // Jump strength
    private Rigidbody rb;              // Player’s Rigidbody
    private bool isGrounded = true;    // Check if on ground

    void Start() {
        currentSpeed = speed;
        rb = GetComponent<Rigidbody>();
    }

    void Update() {
        // Horizontal movement
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 horizontalMovement = new Vector3(moveX, 0, moveZ).normalized * currentSpeed;
        rb.velocity = new Vector3(horizontalMovement.x, rb.velocity.y, horizontalMovement.z);

        // Jumping
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded) {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            isGrounded = false;
        }

        // Ink detection
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1f)) {
            isGrounded = true; // Grounded if close to floor
            if (hit.collider.CompareTag("Ink")) {
                currentSpeed = speed + inkSpeedBoost;
                comboText.text = "Combo: " + GetComponent<PlayerCombat>().comboCount + 
                                 " | " + GetComponent<PlayerCombat>().styleRank + 
                                 " | On Ink!";
            } else {
                currentSpeed = speed;
                comboText.text = "Combo: " + GetComponent<PlayerCombat>().comboCount + 
                                 " | " + GetComponent<PlayerCombat>().styleRank;
            }
        } else {
            currentSpeed = speed;
            isGrounded = false; // In air if no ground below
        }
    }
}