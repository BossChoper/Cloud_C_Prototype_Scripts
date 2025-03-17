using UnityEngine;

[RequireComponent(typeof(CloudCore))]
[RequireComponent(typeof(CloudMovement))]
[RequireComponent(typeof(CloudInkSystem))]
[RequireComponent(typeof(CloudCombiner))]
[RequireComponent(typeof(CloudTransformation))]

public class CloudBehavior : MonoBehaviour
{
    private CloudCore core;
    private CloudMovement movement;
    private CloudInkSystem inkSystem;
    private CloudCombiner combiner;
    private CloudTransformation transformation;

    void Start()
    {
        core = GetComponent<CloudCore>();
        movement = GetComponent<CloudMovement>();
        inkSystem = GetComponent<CloudInkSystem>();
        combiner = GetComponent<CloudCombiner>();
        transformation = GetComponent<CloudTransformation>();
    }

    public void PrepareForLaunch()
    {
        core.PrepareForLaunch();
    }

    public void Launch()
    {
        core.Launch();
        movement.InitializeLaunch(transform.position);
    }

    public void ActivateCloud(float duration)
    {
        core.ActivateCloud(duration);
    }

    public void TriggerBurstRain()
    {
        inkSystem.TriggerBurstRain();
    }
}