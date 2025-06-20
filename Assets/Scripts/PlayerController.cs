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
        
    }
}
