using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CarryTask : MonoBehaviour
{
    private List<MinionAI> assignedMinions = new List<MinionAI>();
    private bool isBeingCarried = false;
    private float carrySpeed = 1f;
    private Transform baseTransform;
    public TextMeshProUGUI minionCountText;

    void Start()
    {
        baseTransform = GameObject.Find("Base").transform;
        minionCountText = GetComponentInChildren<TextMeshProUGUI>();
        UpdateMinionCountText();
    }

    void Update()
    {
        if (isBeingCarried)
        {
            float effectiveSpeed = carrySpeed * (assignedMinions.Count >= 4 ? 1.5f : 1f);
            Vector3 direction = (baseTransform.position - transform.position).normalized;
            transform.position += direction * effectiveSpeed * Time.deltaTime;

            foreach (MinionAI minion in assignedMinions)
            {
                minion.transform.position = transform.position + (minion.transform.position - transform.position).normalized * 0.5f;
            }

            if (Vector3.Distance(transform.position, baseTransform.position) < 0.5f)
            {
                DropAtBase();
            }
        }
    }

    public void AssignMinion()
    {
        GameObject[] minions = GameObject.FindGameObjectsWithTag("Minion");
        foreach (GameObject minionObj in minions)
        {
            MinionAI minion = minionObj.GetComponent<MinionAI>();
            if (!minion.IsAssignedToCarry && assignedMinions.Count < 4)
            {
                minion.AssignToCarryTask(gameObject);
                assignedMinions.Add(minion);
                if (assignedMinions.Count >= 3)
                {
                    isBeingCarried = true;
                }
                UpdateMinionCountText();
                break;
            }
        }
    }

    public void AttachMinion(MinionAI minion)
    {
        // No changes needed here
    }

    public bool IsBeingCarried()
    {
        return isBeingCarried;
    }

    public void DropAtBase()
    {
        foreach (MinionAI minion in assignedMinions)
        {
            minion.ResetTask();
        }
        Destroy(gameObject);
    }

    public void RecallAssignedMinions()
    {
        foreach (MinionAI minion in assignedMinions)
        {
            minion.ResetTask();
        }
        assignedMinions.Clear();
        isBeingCarried = false; // Stop carrying if recalled
        UpdateMinionCountText();
    }

    private void UpdateMinionCountText()
    {
        if (minionCountText != null)
        {
            minionCountText.text = $"{assignedMinions.Count}/4";
        }
    }
}