using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private float moveSpeed = 5f;
    private float sprintSpeed = 10f;

    [Header("Player Health Settings")]
    private float maxHealth = 100f;
    private float currentHealth;
    public Slider playerSlider;

    [Header("Player Attack Settings")]
    public float attackRange = 100f;
    public LayerMask enemyLayer;
    private float attackCooldown = 1f;
    private float lastAttackTime = -Mathf.Infinity;

    [Header("Throwable Settings")]
    public GameObject throwablePrefab; // Assign your projectile prefab in the Inspector
    public float throwForce = 10f;

    private void Awake()
    {
    }

    private void Start()
    {
        currentHealth = maxHealth;
        playerSlider.maxValue = maxHealth;
        playerSlider.value = currentHealth;
    }

    private void Update()
    {
        Movement();
        AutoAttack();
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

    public void Attack(GameObject target)
    {
        Debug.Log("Player is throwing a projectile!");

        if (throwablePrefab != null && target != null)
        {
            // Instantiate projectile at player's position
            GameObject projectile = Instantiate(throwablePrefab, transform.position, Quaternion.identity);

            // Calculate direction to target
            Vector3 direction = (target.transform.position - transform.position).normalized;

            // Apply force to projectile
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(direction * throwForce, ForceMode.VelocityChange);
            }
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

    private void AutoAttack()
    {
        if (Time.time - lastAttackTime < attackCooldown)
            return;

        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);
        if (hitEnemies.Length > 0)
        {
            // Find the closest enemy
            GameObject closestEnemy = null;
            float minDist = Mathf.Infinity;
            foreach (var enemy in hitEnemies)
            {
                float dist = (enemy.transform.position - transform.position).sqrMagnitude;
                if (dist < minDist)
                {
                    minDist = dist;
                    closestEnemy = enemy.gameObject;
                }
            }

            if (closestEnemy != null)
            {
                Attack(closestEnemy);
                lastAttackTime = Time.time;
            }
        }
    }
}
