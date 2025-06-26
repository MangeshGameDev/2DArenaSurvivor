using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{  
    public static PlayerController instance;
    // Player Movement Settings
   [SerializeField] private float moveSpeed = 5f;
  

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
    private CircleCollider2D CircleCollider2D; // CircleCollider2D for attack range
    public LayerMask enemyLayer;
    private float attackCooldown = 0.5f;
    private float lastAttackTime = -Mathf.Infinity;

    // References to other managers
    public SpawnManagerForPlayer spawnManagerForPlayer; 
    public UpgradeManager upgradeManager;

    // References to components
    private Rigidbody2D rb; 
    [HideInInspector] public bool isDead = false;
    private bool EnemyInRange = false; // Check if an enemy is in range for attack

    // 
    public Transform sprite; // Reference to the sprite transform for animations
    private Animator animator; // Animator component for animations
    public AnimationClip walkAnimation; // Walk animation clip
    public AnimationClip attackAnimation; // Attack animation clip
    public AnimationClip idleAnimation;
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
        // Get the CircleCollider2D component for attack range
        CircleCollider2D = GetComponent<CircleCollider2D>();
        CircleCollider2D.radius = attackRange; // Set the radius of the attack range collider
        // boolean to check if the player is dead
        isDead = false;
        // Get the Animator component for animations
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        Movement();
        AutoAttack();
        UpgradeWeapon();
        die(); 

    }
    private void FixedUpdate()
    {
      
    }

    public void Movement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveHorizontal, moveVertical, 0);
        movement.Normalize(); // Normalize to prevent faster diagonal movement
        if (movement != Vector3.zero)
        {
            AnimationState(1); // Play walk animation when moving
            rb.linearVelocity = movement * moveSpeed; // Set the velocity for movement
        }
        else if (movement == Vector3.zero )
        {
            if(EnemyInRange == false)
            {
                AnimationState(3);
            }    
            else
            {
                AnimationState(2);
            }
            rb.linearVelocity = Vector3.zero; // Stop the player if no input is detected
        }
        transform.position = new Vector3( Mathf.Clamp(transform.position.x, -8f, 8f), Mathf.Clamp(transform.position.y, -4.5f, 4.5f), transform.position.z); // Clamp the player's position within the screen bounds
    }

    public void Attack()
    {
        if(WaveSystem.Instance.enemyCount >= 1 && EnemyInRange)
        {
            AnimationState(2); // Play attack animation
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

    private void AnimationState( int AnimationIndex)
    {
        // 1 = Walk Animation, 2 = Attack Animation, 3 = Idle Animation
        if ( AnimationIndex == 1)
        {
            animator.Play(walkAnimation.name); // Play walk animation
          Input.GetAxis("Horizontal"); // Get horizontal input for movement
            if (Input.GetAxis("Horizontal") < 0) // If moving left
            {
                sprite.localScale = new Vector3(-1, 1, 1); // Flip the sprite to face left
            }
            else if (Input.GetAxis("Horizontal") > 0) // If moving right
            {
                sprite.localScale = new Vector3(1, 1, 1); // Reset the sprite to face right
            }
        }
        if (AnimationIndex == 2)
        {
            animator.Play(attackAnimation.name); // Play attack animation
        }
        else if(AnimationIndex != 1&& AnimationIndex!=2)
        {
            animator.Play(idleAnimation.name); // Play idle animation
        }
    }

  
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyInRange = true; // Ensure the flag remains true while an enemy is in range
            
        }
        else
        {
            EnemyInRange = false;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
           // EnemyInRange = false; // Set the flag to false when no enemies are in range
        }
    }

}
