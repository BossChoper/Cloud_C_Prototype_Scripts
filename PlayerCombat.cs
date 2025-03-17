using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public GameObject enemy;
    public float attackRange = 2f;
    public float jumpForce = 10f;
    public float aerialBoost = 3f;
    public float aerialLift = 2f;
    public int maxAerialCombo = 3;
    public float slamForce = 15f;

    private Rigidbody playerRb;
    private bool isGrounded = true;
    private int hitCount = 0;
    private int aerialHitCount = 0;

    // Component references
    private ComboSystem comboSystem;
    private CloudController cloudController;
    private PowerupController powerupController;
    private EnemyWeightSystem enemyWeightSystem;

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        comboSystem = GetComponent<ComboSystem>();
        cloudController = GetComponent<CloudController>();
        powerupController = GetComponent<PowerupController>();
        InitializeEnemy();
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
        enemyWeightSystem = enemy.GetComponent<EnemyWeightSystem>();
        if (enemyWeightSystem == null)
        {
            enemyWeightSystem = enemy.AddComponent<EnemyWeightSystem>();
        }
    }

    void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1f);
        HandleJumpInput();
        HandleLaunchInput();
        HandleAttackInput();
        HandleSlamInput();
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
            if (distance < attackRange)
            {
                float weight = enemyWeightSystem.GetWeight();
                float launchHeight = 10f / weight;
                enemy.GetComponent<Rigidbody>().velocity = new Vector3(0, launchHeight, 0);
                comboSystem.AddCombo(2);
                enemyWeightSystem.ReduceWeight();
                enemyWeightSystem.FlashWhite();
                Debug.Log($"Launched! Height: {launchHeight}, Weight: {weight}");
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            cloudController.LaunchCloud();
        }
    }

    private void HandleAttackInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && enemy != null)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < attackRange)
            {
                ProcessAttackHit();
            }
        }
    }

    private void ProcessAttackHit()
    {
        comboSystem.UpdateCombo(false);
        hitCount++;
        enemyWeightSystem.ReduceWeight();
        enemyWeightSystem.FlashWhite();

        if (!isGrounded && aerialHitCount < maxAerialCombo)
        {
            ProcessAerialHit();
        }
        else if (!isGrounded)
        {
            playerRb.velocity = new Vector3(playerRb.velocity.x, aerialLift, playerRb.velocity.z);
            comboSystem.AddCombo(1, false);
        }

        if (enemy.transform.position.y > 1.5f)
        {
            comboSystem.AddCombo(1, false);
        }

        if (hitCount == 2 && cloudController.currentCloud == null)
        {
            cloudController.SpawnCloudAboveTarget(enemy);
        }
        if (cloudController.currentCloud != null)
        {
            cloudController.GrowCurrentCloud();
        }

        if (hitCount > 2)
        {
            Destroy(enemy);
            enemy = null;
            hitCount = 0;
            aerialHitCount = 0;
        }
    }

    private void ProcessAerialHit()
    {
        float weight = enemyWeightSystem.GetWeight();
        float adjustedBoost = aerialBoost / weight;
        Vector3 boostVelocity = new Vector3(playerRb.velocity.x, adjustedBoost, playerRb.velocity.z);
        
        playerRb.velocity = boostVelocity;
        enemy.GetComponent<Rigidbody>().velocity = boostVelocity;
        
        aerialHitCount++;
        comboSystem.AddCombo(1, false);
        Debug.Log($"Aerial hit #{aerialHitCount}! Boost: {adjustedBoost}, Weight: {weight}");
    }

    private void HandleSlamInput()
    {
        if (Input.GetKeyDown(KeyCode.S) && !isGrounded && enemy != null)
        {
            Vector3 slamVelocity = new Vector3(playerRb.velocity.x, -slamForce, playerRb.velocity.z);
            playerRb.velocity = slamVelocity;
            enemy.GetComponent<Rigidbody>().velocity = slamVelocity;
            
            comboSystem.AddCombo(2);
            aerialHitCount = 0;
            Debug.Log("Slam! Player and enemy forced downward.");
        }
    }
}