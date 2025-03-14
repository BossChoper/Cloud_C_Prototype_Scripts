using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombineObjects : MonoBehaviour {
    public GameObject bigCubePrefab; // The larger object to spawn
    private bool hasCombined = false; // Prevent multiple combines

    void OnCollisionEnter(Collision collision) {
        // Check if we hit another small cube
        if (collision.gameObject.CompareTag("SmallCube") && !hasCombined) {
            // Mark both as combined
            hasCombined = true;
            CombineObjects otherScript = collision.gameObject.GetComponent<CombineObjects>();
            if (otherScript != null) otherScript.hasCombined = true;

            // Spawn the big cube at the midpoint
            Vector3 midPoint = (transform.position + collision.transform.position) / 2;
            Instantiate(bigCubePrefab, midPoint, Quaternion.identity);

            // Destroy both small cubes
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}