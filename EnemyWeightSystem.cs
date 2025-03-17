using UnityEngine;

public class EnemyWeightSystem : MonoBehaviour
{
    private float enemyWeight = 1f;
    private float weightReductionPerHit = 0.2f;
    private float minWeight = 0.1f;
    private float weightResetDelay = 3f;
    private float lastHitTimer = 0f;
    
    private Renderer enemyRenderer;
    private Color originalColor;

    void Start()
    {
        enemyRenderer = GetComponent<Renderer>();
        if (enemyRenderer != null)
        {
            originalColor = enemyRenderer.material.color;
        }
    }

    void Update()
    {
        if (lastHitTimer > 0)
        {
            lastHitTimer -= Time.deltaTime;
            if (lastHitTimer <= 0 && enemyWeight < 1f)
            {
                ResetWeight();
            }
        }
    }

    public float GetWeight()
    {
        return enemyWeight;
    }

    public void ReduceWeight()
    {
        enemyWeight = Mathf.Max(minWeight, enemyWeight - weightReductionPerHit);
        lastHitTimer = weightResetDelay;
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = Color.Lerp(Color.red, originalColor, enemyWeight);
        }
        Debug.Log($"Enemy weight reduced to: {enemyWeight}");
    }

    public void ResetWeight()
    {
        enemyWeight = 1f;
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = originalColor;
        }
        Debug.Log("Enemy weight reset to 1!");
    }

    public void FlashWhite()
    {
        if (enemyRenderer != null)
        {
            StartCoroutine(FlashEffect());
        }
    }

    private System.Collections.IEnumerator FlashEffect()
    {
        Color currentColor = enemyRenderer.material.color;
        enemyRenderer.material.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        enemyRenderer.material.color = currentColor;
    }
}
