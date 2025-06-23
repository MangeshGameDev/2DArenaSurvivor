using UnityEditor.Rendering;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // dependencies
    private UpgradeManager upgradeManager; 
    private SpawnManagerForPlayer spawnManagerForPlayer; 
    private PlayerController playerController; 

    public float speed = 10f; // Speed of the bullet
    public float lifetime = 5f; // Time before the bullet is destroyed
    public float damage = 50f; // Damage dealt by the bullet
    [HideInInspector] public Vector2 direction; // Direction of the bullet
   
    private Vector2 moveTarget; // The position to move towards
    private bool hasTarget = false;

    private void Awake()
    {
        upgradeManager  = UpgradeManager.instance; // Get the UpgradeManager instance
        spawnManagerForPlayer = SpawnManagerForPlayer.Instance; // Get the SpawnManagerForPlayer instance
        playerController = PlayerController.instance; // Get the PlayerController instance
    }

    private void OnEnable()
    {
       
        SeekAndSetTarget();
    }

    private void Update()
    {
        ShootWhenGotTarget();
        damage = upgradeManager.bulletDamage; // Update bullet damage from UpgradeManager
    }

    private void ShootWhenGotTarget()
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
        if (playerController == null)
        {
            spawnManagerForPlayer.DeactivatePooledObject(gameObject);
            gameObject.SetActive(false);
            return;
        }

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
            // No target found, deactivate bullet immediately
            spawnManagerForPlayer.DeactivatePooledObject(gameObject);
            gameObject.SetActive(false);
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
