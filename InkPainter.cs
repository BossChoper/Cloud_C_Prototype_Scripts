using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkPainter : MonoBehaviour {
    public Material inkMaterial; // The ink color

    void Update() {
    if (Input.GetKeyDown(KeyCode.E)) {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f)) {
            GameObject inkSpot = GameObject.CreatePrimitive(PrimitiveType.Plane);
            inkSpot.transform.position = hit.point + Vector3.up * 0.01f;
            inkSpot.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            inkSpot.GetComponent<Renderer>().material = inkMaterial;
            inkSpot.tag = "Ink"; // Add this line
        }
    }
}
}
