using UnityEngine;

public class CloudInkSystem : MonoBehaviour
{
    public float rainInterval = 1f;
    public float burstRainInterval = 0.2f;

    private CloudCore cloudCore;
    private float rainTimer = 0f;
    private float burstDuration = 0f;
    private bool isBursting = false;

    void Start()
    {
        cloudCore = GetComponent<CloudCore>();
        rainTimer = rainInterval;
    }

    void Update()
    {
        if (cloudCore.IsActivated())
        {
            DropInk();
        }
        else
        {
            UpdateRainTimer();
        }
    }

    private void UpdateRainTimer()
    {
        rainTimer -= Time.deltaTime;
        if (rainTimer <= 0 && !cloudCore.IsActivated())
        {
            DropInk();
            rainTimer = isBursting ? burstRainInterval : rainInterval;
            Debug.Log($"Rain triggered (burst: {isBursting})");
        }
    }

    public void TriggerBurstRain()
    {
        isBursting = true;
        burstDuration = 2f;
        rainTimer = 0f;
        Debug.Log("Burst rain triggered!");
    }

    private void DropInk()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 10f))
        {
            if (hit.collider.gameObject.CompareTag("Ground") || hit.collider.gameObject.CompareTag("Ink"))
            {
                GameObject inkSpot = GameObject.CreatePrimitive(PrimitiveType.Plane);
                inkSpot.transform.position = hit.point + Vector3.up * 0.01f;
                float inkSize = isBursting ? 0.3f : 0.2f;
                inkSpot.transform.localScale = new Vector3(inkSize, inkSize, inkSize);
                inkSpot.GetComponent<Renderer>().material = cloudCore.inkMaterial;
                inkSpot.tag = "Ink";
                Debug.Log($"Ink dropped at: {inkSpot.transform.position}");
            }
        }
    }
}
