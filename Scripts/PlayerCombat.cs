using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    public GameObject enemy;
    public float attackRange = 2f;
    public GameObject cloudPrefab;
    [HideInInspector] public GameObject currentCloud;
    public int comboCount = 0;
    public string styleRank = "D";
    public TextMeshProUGUI comboText;
    public float aerialLift = 2f;
    public float cloudLaunchSpeed = 20f;

    public float jumpForce = 10f;
    public float aerialBoost = 3f;
    public int maxAerialCombo = 3;
    public float slamForce = 15f;

    private float comboTimer = 0f;
    private float comboWindow = 2f;
    private bool lastWasSpace = false;
    private Rigidbody enemyRb;
    private Renderer enemyRenderer;
    private Color originalColor;
    private float flashDuration = 0.1f;
    private float flashTimer = 0f;
    private Rigidbody playerRb;
    private bool isGrounded = true;
    private int hitCount = 0;
    private int aerialHitCount = 0;
    private Camera mainCamera;

    private bool hasSuperpower = false;
    private float superpowerDuration = 10f;
    private float superpowerTimer = 0f;
    private Color superpowerColor = Color.red;
    private Renderer playerRenderer;

    // Enemy weight system
    private float enemyWeight = 1f; // Starting weight (1 = normal)
    private float weightReductionPerHit = 0.2f; // How much weight decreases per hit
    private float minWeight = 0.1f; // Minimum weight to avoid zero/negative
    private float weightResetDelay = 3f; // Time before weight resets
    private float lastHitTimer = 0f; // Tracks time since last hit

    void Start()
    {
        InitializeEnemy();
        playerRb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        playerRenderer = GetComponent<Renderer>();
        if (playerRenderer != null)
        {
            originalColor = playerRenderer.material.color;
        }
        else
        {
            Debug.LogWarning("Player Renderer not found! Superpower color change won’t work.");
        }
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found! Cursor system requires a camera.");
        }
    }

    private void InitializeEnemy()
    {
        if (enemy == null)
        {
            enemy = GameObject.FindWithTag("Enemy");
            if (enemy == null)
            {
                Debug.LogError("No GameObject tagged 'Enemy' found in the scene!");
                return;
            }
        }
        enemyRb = enemy.GetComponent<Rigidbody>();
        enemyRenderer = enemy.GetComponent<Renderer>();
        if (enemyRenderer != null) originalColor = enemyRenderer.material.color;
    }

    void Update()
    {
        UpdateTimers();
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1f);

        HandleJumpInput();
        HandleLaunchInput();
        HandleAttackInput();
        HandleSlamInput();
        HandleTestCloudInput();
        HandleCursorInput();
        HandleSuperpower();
        UpdateEnemyWeight(); // New method for weight management
        ValidateCurrentCloud();
    }

    private void UpdateTimers()
    {
        if (comboTimer > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0) ResetCombo();
        }
        if (flashTimer > 0)
        {
            flashTimer -= Time.deltaTime;
            if (flashTimer <= 0 && enemyRenderer != null) enemyRenderer.material.color = originalColor;
        }
        if (lastHitTimer > 0)
        {
            lastHitTimer -= Time.deltaTime;
            if (lastHitTimer <= 0 && enemyWeight < 1f)
            {
                ResetEnemyWeight();
            }
        }
    }

    private void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            playerRb.velocity = new Vector3(playerRb.velocity.x, jumpForce, playerRb.velocity.z);
            isGrounded = false;
            Debug.Log("Player jumped!");
        }
    }

    private void HandleLaunchInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && isGrounded && enemy != null)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < attackRange && enemyRb != null)
            {
                // Scale launch height inversely with weight (lighter = higher)
                float launchHeight = 5f / enemyWeight; // Base 5f, height increases as weight decreases
                enemyRb.velocity = new Vector3(0, launchHeight, 0);
                comboCount += 2;
                comboTimer = comboWindow;
                UpdateStyleRank();
                FlashEnemy();
                ReduceEnemyWeight(); // Reduce weight on launch
                Debug.Log($"Launched! Height: {launchHeight}, Weight: {enemyWeight}, Combo: {comboCount}");
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            LaunchCloud();
        }
    }

    private void HandleAttackInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && enemy != null)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < attackRange)
            {
                UpdateCombo(false);
                hitCount++;
                lastHitTimer = weightResetDelay; // Reset timer on hit
                ReduceEnemyWeight(); // Reduce weight on hit
                comboTimer = comboWindow;
                UpdateStyleRank();
                FlashEnemy();
                Debug.Log($"Combo: {comboCount} | Rank: {styleRank} | Weight: {enemyWeight}");

                if (!isGrounded)
                {
                    if (aerialHitCount < maxAerialCombo)
                    {
                        // Scale aerial boost inversely with weight
                        float adjustedBoost = aerialBoost / enemyWeight;
                        playerRb.velocity = new Vector3(playerRb.velocity.x, adjustedBoost, playerRb.velocity.z);
                        enemyRb.velocity = new Vector3(enemyRb.velocity.x, adjustedBoost, enemyRb.velocity.z);
                        aerialHitCount++;
                        comboCount += 1;
                        Debug.Log($"Aerial hit #{aerialHitCount}! Boost: {adjustedBoost}, Weight: {enemyWeight}");
                    }
                    else
                    {
                        playerRb.velocity = new Vector3(playerRb.velocity.x, aerialLift, playerRb.velocity.z);
                        comboCount += 1;
                        Debug.Log("Aerial attack! Extra combo point, max height reached.");
                    }
                }
                if (enemy.transform.position.y > 1.5f)
                {
                    comboCount += 1;
                    Debug.Log("Aerial hit! Extra combo point!");
                }

                if (hitCount == 2 && currentCloud == null)
                {
                    SpawnCloudAboveEnemy();
                }
                if (currentCloud != null)
                {
                    Vector3 currentScale = currentCloud.transform.localScale;
                    currentScale += new Vector3(0.1f, 0.1f, 0.1f);
                    currentCloud.transform.localScale = currentScale;
                }
                if (hitCount > 2)
                {
                    Destroy(enemy);
                    enemy = null;
                    hitCount = 0;
                    aerialHitCount = 0;
                    enemyWeight = 1f; // Reset weight on enemy death
                }
            }
        }
    }

    private void HandleSlamInput()
    {
        if (Input.GetKeyDown(KeyCode.S) && !isGrounded && enemy != null)
        {
            playerRb.velocity = new Vector3(playerRb.velocity.x, -slamForce, playerRb.velocity.z);
            enemyRb.velocity = new Vector3(enemyRb.velocity.x, -slamForce, enemyRb.velocity.z);
            Debug.Log("Slam! Player and enemy forced downward.");
            comboCount += 2;
            UpdateStyleRank();
            aerialHitCount = 0;
        }
    }

    private void HandleTestCloudInput()
    {
        if (Input.GetKeyDown(KeyCode.R) && enemy != null)
        {
            Debug.Log("Spawning test Cloud near Player, targeting enemy: " + enemy.name);
            GameObject testCloud = InstantiateCloud(transform.position + Vector3.up * 3f + Vector3.right * 2f);
            testCloud.GetComponent<CloudBehavior>().target = enemy;
            testCloud.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            Debug.Log("Test Cloud spawned!");
        }
    }

    private void ValidateCurrentCloud()
    {
        if (currentCloud != null && currentCloud.GetComponent<CloudBehavior>() == null)
        {
            Debug.Log("Clearing currentCloud because CloudBehavior is missing!");
            currentCloud = null;
        }
    }

    private void SpawnCloudAboveEnemy()
    {
        Debug.Log("Spawning Cloud above enemy: " + enemy.name + " (Position: " + enemy.transform.position + ")");
        currentCloud = InstantiateCloud(enemy.transform.position + Vector3.up * 3f);
        currentCloud.GetComponent<CloudBehavior>().target = enemy;
        currentCloud.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        Debug.Log("Cloud spawned and assigned to currentCloud!");
    }

    private GameObject InstantiateCloud(Vector3 position)
    {
        GameObject cloud = Instantiate(cloudPrefab, position, Quaternion.identity);
        Rigidbody cloudRb = cloud.AddComponent<Rigidbody>();
        cloudRb.isKinematic = true;
        cloudRb.useGravity = false;
        cloudRb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        cloud.tag = "Cloud";
        cloud.AddComponent<CloudBehavior>();
        return cloud;
    }

    private void HandleCursorInput()
    {
        if (Input.GetMouseButtonDown(0) && mainCamera != null)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                CloudBehavior cloud = hit.collider.GetComponent<CloudBehavior>();
                if (cloud != null)
                {
                    Debug.Log($"Activated cloud: {hit.collider.gameObject.name}");
                    cloud.ActivateCloud(2f);
                }
            }
        }
    }

    private void LaunchCloud()
    {
        Debug.Log("Q pressed!");
        if (currentCloud == null)
        {
            Debug.Log("No current Cloud!");
            return;
        }

        Debug.Log("Current Cloud exists: " + currentCloud.name);
        CloudBehavior cloudBehavior = currentCloud.GetComponent<CloudBehavior>();
        if (cloudBehavior == null)
        {
            Debug.LogWarning("No CloudBehavior component on currentCloud!");
            return;
        }

        Debug.Log("CloudBehavior found, Target: " + (cloudBehavior.target != null ? cloudBehavior.target.name : "None"));
        if (cloudBehavior.target != gameObject)
        {
            Debug.Log("Cloud not following Player! Target: " + (cloudBehavior.target != null ? cloudBehavior.target.name : "None"));
            return;
        }

        Debug.Log("Cloud is following Player!");
        Rigidbody cloudRb = currentCloud.GetComponent<Rigidbody>();
        if (cloudRb == null)
        {
            Debug.LogWarning("No Rigidbody on Cloud!");
            return;
        }

        cloudBehavior.PrepareForLaunch();
        cloudRb.isKinematic = false;
        cloudRb.useGravity = true;

        Vector3 launchDirection = playerRb.velocity.normalized;
        if (launchDirection.magnitude < 0.1f)
        {
            launchDirection = transform.forward.normalized;
        }
        launchDirection += Vector3.up * 0.05f;
        launchDirection = launchDirection.normalized;

        float adjustedLaunchSpeed = cloudLaunchSpeed * 3f;
        cloudRb.velocity = launchDirection * adjustedLaunchSpeed;

        Debug.Log("Launch Direction: " + launchDirection + ", Velocity set to: " + cloudRb.velocity);
        cloudBehavior.Launch();
        StartCoroutine(LogPositionAfterLaunch(currentCloud));
        currentCloud = null;
        Debug.Log("Cloud launched!");
    }

    private System.Collections.IEnumerator LogPositionAfterLaunch(GameObject cloud)
    {
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(0.5f);
            if (cloud != null)
            {
                Debug.Log("Cloud position after launch: " + cloud.transform.position);
            }
        }
    }

    private void HandleSuperpower()
    {
        if (hasSuperpower)
        {
            superpowerTimer -= Time.deltaTime;
            if (superpowerTimer <= 0)
            {
                DeactivateSuperpower();
            }
        }
    }

    private void ActivateSuperpower()
    {
        hasSuperpower = true;
        superpowerTimer = superpowerDuration;
        if (playerRenderer != null)
        {
            playerRenderer.material.color = superpowerColor;
        }
        Debug.Log("Superpower activated! Player color changed to red.");
    }

    private void DeactivateSuperpower()
    {
        hasSuperpower = false;
        superpowerTimer = 0f;
        if (playerRenderer != null)
        {
            playerRenderer.material.color = originalColor;
        }
        Debug.Log("Superpower deactivated! Player color reverted.");
    }

    private void UpdateEnemyWeight()
    {
        // Managed in UpdateTimers to reset weight after delay
    }

    private void ReduceEnemyWeight()
{
    enemyWeight = Mathf.Max(minWeight, enemyWeight - weightReductionPerHit);
    lastHitTimer = weightResetDelay;
    if (enemyRenderer != null)
    {
        enemyRenderer.material.color = Color.Lerp(Color.red, originalColor, enemyWeight); // Lighter = redder
    }
    Debug.Log($"Enemy weight reduced to: {enemyWeight}");
}

private void ResetEnemyWeight()
{
    enemyWeight = 1f;
    if (enemyRenderer != null)
    {
        enemyRenderer.material.color = originalColor;
    }
    Debug.Log("Enemy weight reset to 1!");
}

    void UpdateStyleRank()
    {
        if (comboCount >= 15) styleRank = "S";
        else if (comboCount >= 10) styleRank = "A";
        else if (comboCount >= 6) styleRank = "B";
        else if (comboCount >= 3) styleRank = "C";
        else styleRank = "D";
        comboText.text = "Combo: " + comboCount + " | " + styleRank;
    }

    void ResetCombo()
    {
        comboCount = 0;
        styleRank = "D";
        comboText.text = "Combo: 0 | D";
        aerialHitCount = 0;
        Debug.Log("Combo dropped!");
    }

    void UpdateCombo(bool isSpace)
    {
        if (isSpace != lastWasSpace) comboCount += 2;
        else comboCount++;
        lastWasSpace = isSpace;
    }

    void FlashEnemy()
    {
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = Color.white;
            flashTimer = flashDuration;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SuperpowerPickup"))
        {
            ActivateSuperpower();
            Destroy(other.gameObject);
            Debug.Log("Collected Superpower Pickup!");
        }
    }
}