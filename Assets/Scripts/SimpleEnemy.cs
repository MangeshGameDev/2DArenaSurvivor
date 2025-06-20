using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

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
    private float currentHealth;


    private float lastAttackTime = -Mathf.Infinity; // Track last attack time

    private void Awake()
    {
        playerGameObject = GameObject.FindGameObjectWithTag("Player");
        playerController = playerGameObject.GetComponent<PlayerController>();
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
        Debug.Log("Chasing the playerGameObject...");
        transform.position = Vector3.MoveTowards(transform.position, playerGameObject.transform.position, moveSpeed * Time.deltaTime);
    }

    private void AttackPlayer()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            playerController.UpdateHealth(-attackpower);
            lastAttackTime = Time.time;
            Debug.Log("Enemy attacked the player!");
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"Enemy took {damage} damage! Current health: {currentHealth}");
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    public void Die()
    {
        Debug.Log("Enemy has died!");
        Destroy(gameObject); // Destroy the enemy GameObject
    }

}
