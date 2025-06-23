using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class SimpleEnemy : MonoBehaviour
{
    
    private GameObject playerGameObject;
    private PlayerController playerController;
    [Header("Enemy Settings")]
    private float moveSpeed = 1f;
    private float attackRange = 1f;
    private float attackCooldown = 1f;
    private float attackpower = 10f;
    [Header("Enemy Health Settings")]
    private float maxHealth = 100f;
    public float currentHealth;
    public GameObject healthBarCanvas;
    public Slider healthBarSlider; // Slider to represent health visually

    private float lastAttackTime = -Mathf.Infinity; // Track last attack time

    private SpawnManager spawnManager; // Reference to the SpawnManager
    private WaveSystem waveSystem; // Reference to the WaveSystem
    private void Awake()
    {
        // Initialize health and health bar
        currentHealth = maxHealth;
        healthBarSlider.maxValue = maxHealth;
        healthBarCanvas.SetActive(false); // Hide health bar canvas initially
        playerGameObject = GameObject.FindGameObjectWithTag("Player");
        playerController = playerGameObject.GetComponent<PlayerController>();
        spawnManager = SpawnManager.Instance; // Get the SpawnManager instance
        waveSystem = WaveSystem.Instance;
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
            spawnManager.SpawnFromPool("ExpCoin", transform.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            Debug.Log(other.name + " hit the enemy!");
        }
        
    }
}
