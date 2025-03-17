using UnityEngine;

public class WrestlingMove : MonoBehaviour
{
    public Animator wrestler1Animator; // Animator for the slamming character
    public Animator wrestler2Animator; // Animator for the slammed character
    public Transform wrestler2; // The slammed character's transform
    public float liftHeight = 2f; // Height to lift before slamming
    public float slamSpeed = 5f; // Speed of the slam

    private bool isSlamming = false;
    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = wrestler2.position; // Store starting position
    }

    void Update()
    {
        // Press "Space" to start the wrestling move
        if (Input.GetKeyDown(KeyCode.Space) && !isSlamming)
        {
            StartSlam();
        }

        // Handle the slam motion
        if (isSlamming)
        {
            MoveCharacters();
        }
    }

    void StartSlam()
    {
        isSlamming = true;
        wrestler1Animator.SetTrigger("StartSlam"); // Trigger slam animation
        wrestler2Animator.SetTrigger("StartSlam"); // Trigger fall animation
    }

    void MoveCharacters()
    {
        // Lift wrestler2 into the air
        if (wrestler2.position.y < originalPosition.y + liftHeight)
        {
            wrestler2.position += Vector3.up * slamSpeed * Time.deltaTime;
        }
        // Slam wrestler2 back down
        else if (wrestler2.position.y > originalPosition.y)
        {
            wrestler2.position -= Vector3.up * slamSpeed * Time.deltaTime;
        }
        else
        {
            isSlamming = false; // Reset when done
        }
    }
}