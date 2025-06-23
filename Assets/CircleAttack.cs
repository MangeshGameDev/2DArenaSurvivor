using UnityEngine;

public class CircleAttack : MonoBehaviour
{
    public float attackPower = 5f;
    public LayerMask enemyLayer;
    public float attackCooldown = 2f;

    private float cooldownTimer = 0f;
    private CircleCollider2D circleCollider;

    void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        if (circleCollider == null)
        {
            Debug.LogError("CircleAttack requires a CircleCollider2D component.");
        }
    }

    void Update()
    {
        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0f)
        {
            PerformAttack();
            cooldownTimer = attackCooldown;
        }
    }

    // Damages all enemies within the collider's radius
    public void PerformAttack()
    {
        if (circleCollider == null) return;
        float attackRadius = circleCollider.radius * Mathf.Max(transform.localScale.x, transform.localScale.y);
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRadius, enemyLayer);
        foreach (var hit in hits)
        {
            SimpleEnemy enemy = hit.GetComponent<SimpleEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(attackPower);
            }
        }
    }

    // Increases the visual scale of the attack area
    public void IncreaseVisualScale(float amount)
    {
        transform.localScale += new Vector3(amount, amount, 0f);
    }

    // Changes the attack power
    public void ChangeAttackPower(float newPower)
    {
        attackPower += newPower;
    }

    private void OnDrawGizmosSelected()
    {
        CircleCollider2D col = GetComponent<CircleCollider2D>();
        if (col != null)
        {
            float attackRadius = col.radius * Mathf.Max(transform.localScale.x, transform.localScale.y);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRadius);
        }
    }
}
