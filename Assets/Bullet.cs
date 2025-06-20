using UnityEditor.Rendering;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float speed = 10f; // Speed of the bullet
    public float lifetime = 5f; // Time before the bullet is destroyed
    private float damage = 100f; // Damage dealt by the bullet
    [HideInInspector] public Vector2 direction; // Direction of the bullet
    private string targetTag = "Enemy"; // Tag to seek
    private SpawnManagerForPlayer spawnManagerForPlayer; // Reference to the SpawnManager for player
    private PlayerController playerController; // Reference to the PlayerController

    private Vector2 moveTarget; // The position to move towards
    private bool hasTarget = false;

    private void Awake()
    {
        spawnManagerForPlayer = GameObject.FindFirstObjectByType<SpawnManagerForPlayer>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        SeekAndSetTarget();
    }

    private void Update()
    {
        if (hasTarget)
        {
            // Move towards the target position
            transform.position = Vector2.MoveTowards(transform.position, moveTarget, speed * Time.deltaTime);

            // Optionally, deactivate if reached target (for non-random movement)
            if ((Vector2)transform.position == moveTarget)
            {
                spawnManagerForPlayer.DeactivatePooledObject(gameObject);
                gameObject.SetActive(false);
            }
        }
        else
        {
            // Move in a random direction if no target
            transform.position += (Vector3)direction * speed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            SimpleEnemy enemy = collision.gameObject.GetComponent<SimpleEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            spawnManagerForPlayer.DeactivatePooledObject(gameObject);
            gameObject.SetActive(false);
        }
    }

    public void SeekAndSetTarget()
    {
        hasTarget = false;
        if (playerController == null) return;

        Vector2 currentPos = transform.position;
        float attackRange = playerController.attackRange;
        LayerMask enemyLayer = playerController.enemyLayer;

        Collider2D[] hits = Physics2D.OverlapCircleAll(currentPos, attackRange, enemyLayer);

        GameObject nearest = null;
        float minDist = Mathf.Infinity;

        foreach (var hit in hits)
        {
            float dist = Vector2.Distance(currentPos, hit.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = hit.gameObject;
            }
        }

        if (nearest != null)
        {
            moveTarget = nearest.transform.position;
            hasTarget = true;
        }
        else
        {
            // Move in a random direction if no enemy is found in range
            float angle = Random.Range(0f, 2f * Mathf.PI);
            direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
            hasTarget = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (playerController != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, playerController.attackRange);
        }

        #if UNITY_EDITOR
        if (playerController != null)
        {
            Vector2 currentPos = transform.position;
            float attackRange = playerController.attackRange;
            LayerMask enemyLayer = playerController.enemyLayer;
            Collider2D[] hits = Physics2D.OverlapCircleAll(currentPos, attackRange, enemyLayer);

            GameObject nearest = null;
            float minDist = Mathf.Infinity;
            foreach (var hit in hits)
            {
                float dist = Vector2.Distance(currentPos, hit.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = hit.gameObject;
                }
            }
            if (nearest != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, nearest.transform.position);
            }
        }
        #endif
    }
}
