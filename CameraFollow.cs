using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    public GameObject player; // The player to follow
    public Vector3 offset = new Vector3(0, 5, -10); // Distance from player

    void Update() {
        transform.position = player.transform.position + offset;
    }
}