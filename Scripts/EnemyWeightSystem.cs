using UnityEngine;

public class EnemyWeightSystem : MonoBehaviour
{
    private float enemyWeight = 1f;
    private float weightReductionPerHit = 0.2f;
    private float minWeight = 0.1f;
    private float weightResetDelay = 3f;
    private float lastHitTimer = 0f;
    private int hitCount = 0;
    private Rigidbody enemyRb;
    private Renderer enemyRenderer;
    private Color originalColor;

    void Start()
    {
        enemyRb = GetComponent<Rigidbody>();
        enemyRenderer = GetComponent<Renderer>();
        if (enemyRenderer != null) originalColor = enemyRenderer.material.color;
    }

    void Update()
    {
        if (lastHitTimer > 0)
        {
            lastHitTimer -= Time.deltaTime;
            if (lastHitTimer <= 0 && enemyWeight < 1f) ResetWeight();
        }
    }

    public void HitEnemy()
    {
        hitCount++;
        ReduceWeight();
        FlashEnemy();
        lastHitTimer = weightResetDelay;
    }

    public void LaunchEnemy(float baseHeight)
    {
        float launchHeight = baseHeight / enemyWeight;
        enemyRb.velocity = new Vector3(0, launchHeight, 0);
        ReduceWeight();
        FlashEnemy();
    }

    public void ApplyAerialForce(float force)
    {
        enemyRb.velocity = new Vector3(enemyRb.velocity.x, force, enemyRb.velocity.z);
    }

    private void ReduceWeight()
    {
        enemyWeight = Mathf.Max(minWeight, enemyWeight - weightReductionPerHit);
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = Color.Lerp(Color.red, originalColor, enemyWeight);
        }
        Debug.Log($"Enemy weight reduced to: {enemyWeight}");
    }

    private void ResetWeight()
    {
        enemyWeight = 1f;
        if (enemyRenderer != null) enemyRenderer.material.color = originalColor;
        Debug.Log("Enemy weight reset to 1!");
    }

    private void FlashEnemy()
    {
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = Color.white;
            Invoke(nameof(ResetColor), 0.1f);
        }
    }

    private void ResetColor()
    {
        if (enemyRenderer != null) enemyRenderer.material.color = Color.Lerp(Color.red, originalColor, enemyWeight);
    }

    public float GetWeight() => enemyWeight;
    public int GetHitCount() => hitCount;
}