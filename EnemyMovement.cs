using UnityEngine;

public class EnemyMovement : MonoBehaviour {
    public float speed = 2f;
    public float distance = 5f;
    private Vector3 startPosition;
    private bool movingRight = true;
    private float currentSpeed;

    void Start() {
        startPosition = transform.position;
        currentSpeed = speed;
    }

    void Update() {
        float moveStep = currentSpeed * Time.deltaTime;
        if (movingRight) {
            transform.Translate(Vector3.right * moveStep);
        } else {
            transform.Translate(Vector3.left * moveStep);
        }

        float currentDistance = Vector3.Distance(startPosition, transform.position);
        if (currentDistance >= distance) {
            movingRight = !movingRight;
        }

        // Check for ink below
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1f)) {
            if (hit.collider.CompareTag("Ink")) {
                currentSpeed = speed * 0.5f; // Slow to half speed
            } else {
                currentSpeed = speed;
            }
        } else {
            currentSpeed = speed;
        }
    }
}

/* An enemy following script
using UnityEngine;

public class EnemyMovement : MonoBehaviour {
    public float speed = 2f;        // How fast it moves
    public GameObject player;       // The player to chase

    void Update() {
        // Move toward the player
        Vector3 direction = (player.transform.position - transform.position).normalized;
        transform.Translate(direction * speed * Time.deltaTime);
    }
}
*/
