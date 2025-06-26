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

    // Animator component for animations
    private Animator animator;
    public AnimationClip walkAnimation; // Walk animation clip
    public AnimationClip attackAnimation; // Attack animation clip
    private Transform sprite;
   
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
        animator = GetComponentInChildren<Animator>(); // Get the Animator component from the child GameObject
        sprite = GetComponentInChildren<Transform>(); // Get the sprite GameObject
        healthBarCanvas.SetActive(false);
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
        AnimationState(1); // Play walk animation
    }

    private void AttackPlayer()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            playerController.UpdateHealth(-attackpower);
            lastAttackTime = Time.time;
          //  Debug.Log("Enemy attacked the player!");
        }
        AnimationState(2); // Play attack animation
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
        // 50% chance to spawn an experience coin
        if (Random.value < 0.5f)
        {
            spawnManager.SpawnFromPool("ExpCoin", this.transform.position);
        }
       
    }

    private  void AnimationState( int AnimationIndex)
    {
        // 1 = Walk, 2 = Attack
        if ( AnimationIndex == 1)
        {
           animator.Play(walkAnimation.name);

        }
        if (AnimationIndex == 2)
        {
            animator.Play(attackAnimation.name);
        }
       FacePlayer(); // Face the player while animating
    }
    private void FacePlayer()
    {
        Vector3 direction = playerGameObject.transform.position - transform.position;
        if (direction.x > 0)
        {
            // Face right
            sprite.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (direction.x < 0)
        {
            // Face left (flip 180 on Y)
            sprite.localRotation = Quaternion.Euler(0, 180, 0);
        }
    }
}
