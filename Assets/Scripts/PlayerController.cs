using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{  
    public static PlayerController instance;
    // Player Movement Settings
   [SerializeField] private float moveSpeed = 5f;
   [SerializeField] private float sprintSpeed = 10f;

   //Player Health Settings
  [SerializeField]  private float maxHealth = 100f;
    public float currentHealth;
    public Slider playerSlider;

    //"Player Exp Settings"
    public float maxExp = 100f;
    public float currentExp = 0f;
    public Slider expSlider; 


    //"Player Attack Settings"
    public float attackRange = 3f;
    public LayerMask enemyLayer;
    private float attackCooldown = 0.5f;
    private float lastAttackTime = -Mathf.Infinity;

    // References to other managers
    public SpawnManagerForPlayer spawnManagerForPlayer; 
    public UpgradeManager upgradeManager;

    // References to components
    private Rigidbody2D rb; 
    [HideInInspector] public bool isDead = false;
    private void Awake()
    {
       Time.timeScale = 1f; // Ensure the game is running at normal speed
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
        // Get the Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();
        // boolean to check if the player is dead
        isDead = false;
    }

    private void Update()
    {
        Movement();
        AutoAttack();
        UpgradeWeapon();
        die(); 

    }

    public void Movement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveHorizontal, moveVertical, 0.0f);
        movement.Normalize(); // Normalize to prevent faster diagonal movement
        if (Input.GetKey(KeyCode.LeftShift))
        {
            rb.MovePosition(movement * sprintSpeed * Time.deltaTime);
        }
        else
        {
            transform.Translate(movement * moveSpeed * Time.deltaTime);
        }
    }

    public void Attack()
    {
        if(WaveSystem.Instance.enemyCount >= 1)
        {
            spawnManagerForPlayer.SpawnFromPool("Bullet", transform.position); // Spawn a bullet from the pools

        }
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

    public void UpdateMaxExp()
    {
        maxExp += WaveSystem.Instance.waveNumber * 2; // Increase maxExp based on the wave number
        expSlider.maxValue = maxExp; // Update the max value of the exp slider
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

    private void die()
    {
        if (currentHealth <= 0)
        {
            isDead = true; 
            Time.timeScale = 0f; 
        }
    }
}
