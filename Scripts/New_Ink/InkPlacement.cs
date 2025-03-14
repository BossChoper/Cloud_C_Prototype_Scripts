using UnityEngine;

public class InkPlacement : MonoBehaviour
{
    public GameObject inkPuddlePrefab;
    public float mergeDistance = 2f;
    public LayerMask groundLayer;
    public Camera mainCamera;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PlaceInk();
        }
    }

    void PlaceInk()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        //Check if ray hits the ground
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            Vector3 placementPosition = hit.point;

            //Check if there is already ink
            Collider[] nearbyObjects = Physics.OverlapSphere(placementPosition, inkPuddlePrefab.transform.localScale.x / 2);
            bool canPlace = true;

            foreach(Collider col in nearbyObjects)
            {
                if(col.CompareTag("Ink"))
                {
                    canPlace = false;
                    break;
                }
            }

            if (canPlace)
            {
                GameObject newPuddle = Instantiate(inkPuddlePrefab, placementPosition, Quaternion.identity);
                newPuddle.tag = "Ink";
                //Align the puddle with uneven terrain
                newPuddle.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                MergeNearbyPuddles(newPuddle);
            }
        }
    }

    void MergeNearbyPuddles(GameObject newPuddle)
    {
        Collider[] nearbyPuddles = Physics.OverlapSphere(newPuddle.transform.position, mergeDistance);
        foreach(Collider col in nearbyPuddles)
        {
            if(col.CompareTag("Ink") && col.gameObject != newPuddle)
            {
                //Simple merge: scale up the new puddle and destroy old one
                Vector3 newScale = newPuddle.transform.localScale;
                newScale.x += col.transform.localScale.x * 0.5f; // Increase size based on nearby puddle
                // newScale.y += col.transform.localScale.y * 0.5f; // Increases size of new puddle height; not needed
                newScale.z += col.transform.localScale.z * 0.5f;
                newPuddle.transform.localScale = newScale;
            }
        }
    }
}