using UnityEngine;
using System.Collections.Generic;

public class SummonController : MonoBehaviour
{
    public GameObject summonPrefab; // The summon object to spawn
    private GameObject activeSummon; // Current summoned instance
    private SummonBehavior summonScript; // Reference to its behavior script
    private bool isSummoned = false;

    void Update()
    {
        // Hold Q to summon, release to despawn
        if (Input.GetKeyDown(KeyCode.Q) && !isSummoned)
        {
            SummonCharacter();
        }
        else if (Input.GetKeyUp(KeyCode.Q) && isSummoned)
        {
            DespawnCharacter();
        }

        // Add commands while summoned
        if (isSummoned)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) // Press 1 to move to a point
            {
                Vector3 targetPos = GetMouseWorldPosition();
                summonScript.AddCommand(new MoveCommand(targetPos));
            }
            if (Input.GetKeyDown(KeyCode.Alpha2)) // Press 2 to attack nearest enemy
            {
                summonScript.AddCommand(new AttackCommand());
            }
        }
    }

    void SummonCharacter()
    {
        activeSummon = Instantiate(summonPrefab, transform.position + Vector3.forward * 2, Quaternion.identity);
        summonScript = activeSummon.GetComponent<SummonBehavior>();
        isSummoned = true;
    }

    void DespawnCharacter()
    {
        if (activeSummon != null)
        {
            Destroy(activeSummon);
            isSummoned = false;
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.point;
        }
        return transform.position; // Fallback to player position
    }
}