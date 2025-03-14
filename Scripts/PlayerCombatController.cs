using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    public GameObject enemy;
    public float attackRange = 2f;
    public float aerialLift = 2f;
    public float aerialBoost = 3f;
    public int maxAerialCombo = 3;

    private PlayerMovement playerMovement;
    private EnemyWeightSystem enemyWeightSystem;
    private ComboSystem comboSystem;
    private CloudController cloudController;
    private int aerialHitCount = 0;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        comboSystem = GetComponent<ComboSystem>();
        cloudController = GetComponent<CloudController>();
        InitializeEnemy();
    }

    void Update()
    {
        HandleLaunchInput();
        HandleAttackInput();
    }

    private void InitializeEnemy()
    {
        if (enemy == null)
        {
            enemy = GameObject.FindWithTag("Enemy");
            if (enemy == null) Debug.LogError("No GameObject tagged 'Enemy' found!");
        }
        enemyWeightSystem = enemy?.GetComponent<EnemyWeightSystem>();
    }

    private void HandleLaunchInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && playerMovement.IsGrounded() && enemy != null)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < attackRange && enemyWeightSystem != null)
            {
                enemyWeightSystem.LaunchEnemy(5f);
                comboSystem.AddCombo(2);
                Debug.Log($"Launched! Combo: {comboSystem.GetComboCount()}");
            }
        }
    }

    private void HandleAttackInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && enemy != null)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < attackRange)
            {
                enemyWeightSystem.HitEnemy();
                comboSystem.AddCombo(1);
                Debug.Log($"Combo: {comboSystem.GetComboCount()}");

                if (!playerMovement.IsGrounded())
                {
                    HandleAerialAttack();
                }

                int hitCount = enemyWeightSystem.GetHitCount();
                if (hitCount == 2) cloudController.SpawnCloudAboveEnemy(enemy);
                if (hitCount > 2)
                {
                    Destroy(enemy);
                    enemy = null;
                    aerialHitCount = 0;
                }
            }
        }
    }

    private void HandleAerialAttack()
    {
        Rigidbody playerRb = playerMovement.GetRigidbody();
        if (aerialHitCount < maxAerialCombo)
        {
            float adjustedBoost = aerialBoost / enemyWeightSystem.GetWeight();
            playerRb.velocity = new Vector3(playerRb.velocity.x, adjustedBoost, playerRb.velocity.z);
            enemyWeightSystem.ApplyAerialForce(adjustedBoost);
            aerialHitCount++;
            comboSystem.AddCombo(1);
            Debug.Log($"Aerial hit #{aerialHitCount}!");
        }
        else
        {
            playerRb.velocity = new Vector3(playerRb.velocity.x, aerialLift, playerRb.velocity.z);
            comboSystem.AddCombo(1);
            Debug.Log("Aerial attack! Max height reached.");
        }
    }
}