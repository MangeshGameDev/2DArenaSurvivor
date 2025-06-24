using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class SimpleEnemy : MonoBehaviour
{
    
    private GameObject playerGameObject;
    private PlayerController playerController;
    private SimpleEnemyValues simpleEnemyValues; // Reference to the SimpleEnemyValues scriptable object
    // Enemy Settings
    private float moveSpeed ;
    private float attackRange ;
    private float attackCooldown;
    private float attackpower ;
    // Health Settings
    private float maxHealth;
    public float currentHealth;
    public GameObject healthBarCanvas;
    public Slider healthBarSlider; // Slider to represent health visually

    private float lastAttackTime = -Mathf.Infinity; // Track last attack time

    private SpawnManager spawnManager; // Reference to the SpawnManager
    private WaveSystem waveSystem; // Reference to the WaveSystem
    private void Awake()
    {
        InitializeEnemy();
    }

    private void InitializeEnemy()
    {
        // Initialize health and health bar
        currentHealth = maxHealth;
        healthBarSlider.maxValue = maxHealth;
        // Hide health bar canvas initially
        healthBarCanvas.SetActive(false); 
        playerGameObject = GameObject.FindGameObjectWithTag("Player");
        playerController = playerGameObject.GetComponent<PlayerController>();
        // Get the SpawnManager instance
        spawnManager = SpawnManager.Instance;
        waveSystem = WaveSystem.Instance;
        
        simpleEnemyValues = GetComponent<SimpleEnemyValues>();
    }
    private void Start()
    {
        AssignValuesFromConfig();
    }

    private void AssignValuesFromConfig()
    {
        moveSpeed = simpleEnemyValues.moveSpeed; // Get move speed from the SimpleEnemyValues scriptable object
        attackRange = simpleEnemyValues.attackRange; // Get attack range from the SimpleEnemyValues scriptable object
        attackCooldown = simpleEnemyValues.attackCooldown; // Get attack cooldown from the SimpleEnemyValues scriptable object
        attackpower = simpleEnemyValues.attackpower; // Get attack power from the SimpleEnemyValues scriptable object
        maxHealth = simpleEnemyValues.maxHealth; // Get max health from the SimpleEnemyValues scriptable object
        currentHealth = maxHealth; // Initialize current health
        healthBarSlider.maxValue = maxHealth;
    }

    void Update()
    {
        StateMachine();    
    }
    
    public void StateMachine()
    {
        Vector3 playerDistance = playerGameObject.transform.position - gameObject.transform.position;
        if (playerDistance.magnitude < attackRange)
        {
            AttackPlayer();
        }
        else
        {
            ChasePlayer();
        }
    }
    
    private void ChasePlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, playerGameObject.transform.position, moveSpeed * Time.deltaTime);
    }

    private void AttackPlayer()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            playerController.UpdateHealth(-attackpower);
            lastAttackTime = Time.time;
          //  Debug.Log("Enemy attacked the player!");
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        
        if (currentHealth <= 0)
        {
            Die();
        }
        // Update health bar
        healthBarSlider.value = currentHealth;
        healthBarCanvas.SetActive(true); // Show health bar canvas when taking damage
    }
    public void Die()
    {
        waveSystem.OnEnemyDied(); // Notify the WaveSystem that an enemy has died
        spawnManager.DeactivatePooledObject(gameObject); // Deactivate the enemy object instead of destroying it
        currentHealth = maxHealth; // Reset health for next spawn
        healthBarCanvas.SetActive(false); // Hide health bar canvas when enemy dies
        // 30% chance to spawn an experience coin
        if (Random.value < 0.3f)
        {
            spawnManager.SpawnFromPool("ExpCoin", this.transform.position);
        }
    }
}
