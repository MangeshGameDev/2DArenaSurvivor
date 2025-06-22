using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance; // Singleton instance for easy access
    private float moveSpeed = 5f;
    private float sprintSpeed = 10f;

    [Header("Player Health Settings")]
    private float maxHealth = 100f;
    public float currentHealth;
    public Slider playerSlider;
    [Header("Player Exp Settings")]
    public float maxExp = 100f;
    public float currentExp = 0f;
    public Slider expSlider; // Slider to represent experience visually


    [Header("Player Attack Settings")]
    public float attackRange = 5f;
    public LayerMask enemyLayer;
    private float attackCooldown = 0.5f;
    private float lastAttackTime = -Mathf.Infinity;

    private SpawnManagerForPlayer spawnManagerForPlayer; // Reference to the SpawnManager for player
    private UpgradeManager upgradeManager; // Reference to the UpgradeManager script
    private void Awake()
    {
        spawnManagerForPlayer = SpawnManagerForPlayer.Instance; // Find the SpawnManagerForPlayer in the scene
        upgradeManager = UpgradeManager.instance; 
    }

   
    private void Start()
    {
        #region Singleton Pattern
        if (instance == null)
        {
            instance = this; // Set the singleton instance
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
       #endregion
        // Initialize health and health bar
        currentHealth = maxHealth;
        playerSlider.maxValue = maxHealth;
        playerSlider.value = currentHealth;
        // Initialize experience and experience bar
        expSlider.maxValue = maxExp;
        expSlider.value = currentExp;


    }

    private void Update()
    {
        Movement();
        AutoAttack();
        UpgradeWeapon();
    }

    public void Movement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveHorizontal, moveVertical, 0.0f);
        movement.Normalize(); // Normalize to prevent faster diagonal movement
        if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.Translate(movement * sprintSpeed * Time.deltaTime);
        }
        else
        {
            transform.Translate(movement * moveSpeed * Time.deltaTime);
        }
    }

    public void Attack()
    {
        spawnManagerForPlayer.SpawnFromPool("Bullet", transform.position); // Spawn a bullet from the pools

    }

    public void UpdateHealth(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (playerSlider != null)
        {
            playerSlider.value = currentHealth;
        }
    }
    public void UpdateExp(float amount)
    {
        currentExp += amount;
        currentExp = Mathf.Clamp(currentExp, 0, maxExp);
        if (expSlider != null)
        {
            expSlider.value = currentExp;
        }
    }

    private void AutoAttack()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time; // Update the last attack time
        }
          
    }
    private void UpgradeWeapon()
    {
        if(currentExp >= maxExp) // Check if the player has enough experience to upgrade
        {
            upgradeManager.EnableUIPanel(); // Open the upgrade panel UI
            UpdateExp(-maxExp);
        }
    }
}
