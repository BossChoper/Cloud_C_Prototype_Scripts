using UnityEngine;
using System.Collections.Generic;

public class SummonBehavior : MonoBehaviour
{
    private Queue<Command> commandQueue = new Queue<Command>();
    private Command currentCommand;
    public float moveSpeed = 3f;
    public float attackRange = 1.5f;

    void Update()
    {
        if (currentCommand == null && commandQueue.Count > 0)
        {
            currentCommand = commandQueue.Dequeue();
        }

        if (currentCommand != null)
        {
            currentCommand.Execute(this);
            if (currentCommand.IsFinished)
            {
                currentCommand = null;
            }
        }
    }

    public void AddCommand(Command command)
    {
        commandQueue.Enqueue(command);
    }
}

// Abstract Command class
public abstract class Command
{
    public bool IsFinished { get; protected set; }
    public abstract void Execute(SummonBehavior summon);
}

// Move Command
public class MoveCommand : Command
{
    private Vector3 targetPosition;

    public MoveCommand(Vector3 target)
    {
        targetPosition = target;
        IsFinished = false;
    }

    public override void Execute(SummonBehavior summon)
    {
        Vector3 direction = (targetPosition - summon.transform.position).normalized;
        summon.transform.position += direction * summon.moveSpeed * Time.deltaTime;

        if (Vector3.Distance(summon.transform.position, targetPosition) < 0.1f)
        {
            IsFinished = true;
        }
    }
}

// Attack Command
public class AttackCommand : Command
{
    private GameObject target;

    public AttackCommand()
    {
        IsFinished = false;
    }

    public override void Execute(SummonBehavior summon)
    {
        if (target == null)
        {
            target = FindNearestEnemy(summon);
        }

        if (target != null)
        {
            Vector3 direction = (target.transform.position - summon.transform.position).normalized;
            summon.transform.position += direction * summon.moveSpeed * Time.deltaTime;

            if (Vector3.Distance(summon.transform.position, target.transform.position) < summon.attackRange)
            {
                Debug.Log("Attacking " + target.name); // Simulate attack
                IsFinished = true;
            }
        }
        else
        {
            IsFinished = true; // No enemy found
        }
    }

    GameObject FindNearestEnemy(SummonBehavior summon)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = null;
        float minDist = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector3.Distance(summon.transform.position, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = enemy;
            }
        }
        return closest;
    }
}