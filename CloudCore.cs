using UnityEngine;

public class CloudCore : MonoBehaviour
{
    public GameObject target;
    public Material inkMaterial;
    public float lingerTime = 3f;
    
    private float lingerTimer = 0f;
    private bool enemyDead = false;
    private bool launched = false;
    private bool isPreparingLaunch = false;
    private Renderer cloudRenderer;
    private Color originalColor;
    private Rigidbody rb;

    private bool isActivated = false;
    private float activationTimer = 0f;
    private Color activatedColor = Color.yellow;

    void Start()
    {
        cloudRenderer = GetComponent<Renderer>();
        originalColor = cloudRenderer.material.color;
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    public void PrepareForLaunch()
    {
        isPreparingLaunch = true;
        target = null;
        Debug.Log("Cloud preparing for launch, stopping follow behavior!");
    }

    public void Launch()
    {
        launched = true;
        target = null;
        lingerTimer = lingerTime;
        isPreparingLaunch = false;
        Debug.Log("Cloud marked as launched!");
    }

    public void ActivateCloud(float duration)
    {
        isActivated = true;
        activationTimer = duration;
        cloudRenderer.material.color = activatedColor;
        Debug.Log($"Cloud {gameObject.name} activated for {duration} seconds!");
    }

    public void DeactivateCloud()
    {
        isActivated = false;
        activationTimer = 0f;
        cloudRenderer.material.color = originalColor;
        Debug.Log($"Cloud {gameObject.name} deactivated!");
    }

    public void UpdateLingerTimer()
    {
        lingerTimer -= Time.deltaTime;
        float alpha = lingerTimer / lingerTime;
        cloudRenderer.material.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
        if (lingerTimer <= 0)
        {
            Destroy(gameObject);
        }
    }

    public bool IsLaunched() => launched;
    public bool IsActivated() => isActivated;
    public bool IsPreparingLaunch() => isPreparingLaunch;
    public Rigidbody GetRigidbody() => rb;
    public void SetLingerTimer(float time) => lingerTimer = time;
    public void SetEnemyDead(bool dead) => enemyDead = dead;
    public bool IsEnemyDead() => enemyDead;
    public Color GetOriginalColor() => originalColor;
    public void SetColor(Color color) => cloudRenderer.material.color = color;
}
