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
    private Rigidbody2D rb; // Reference to the Rigidbody2D component
    private PlayerController playerController; // Reference to the PlayerController

    private void Awake()
    {
        spawnManagerForPlayer = GameObject.FindFirstObjectByType<SpawnManagerForPlayer>(); // Find the SpawnManagerForPlayer in the scene
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>(); // Find the PlayerController
    }
    private void Start()
    {
        SeekAndMoveToNearest();
    }

    private void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log(Equals(collision.gameObject.name, "Enemy"));
            // Assuming the enemy has a method to take damage
            SimpleEnemy enemy = collision.gameObject.GetComponent<SimpleEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage); // Adjust damage value as needed
            }
            spawnManagerForPlayer.DeactivatePooledObject(gameObject); // Deactivate the bullet object instead of destroying it
            gameObject.SetActive(false); // Deactivate the bullet
        }
    }

    public void SeekAndMoveToNearest()
    {
        if (playerController == null) return;

        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);
        GameObject nearest = null;
        float minDist = Mathf.Infinity;
        Vector2 currentPos = transform.position;
        float attackRange = playerController.attackRange;

        foreach (GameObject obj in targets)
        {
            float dist = Vector2.Distance(currentPos, obj.transform.position);
            if (dist < minDist && dist <= attackRange)
            {
                minDist = dist;
                nearest = obj;
            }
        }

        Vector2 dir;
        if (nearest != null)
        {
            dir = ((Vector2)nearest.transform.position - currentPos).normalized;
        }
        else
        {
            // Move in a random direction if no enemy is found in range
            float angle = Random.Range(0f, 2f * Mathf.PI);
            dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
        }
        // Set the direction of the bullet
        rb.AddForce(dir * speed, ForceMode2D.Impulse);
    }
}
