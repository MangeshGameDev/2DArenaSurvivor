using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 50f; // Damage dealt by the bullet

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag ("Enemy"))
        {
            // Assuming the enemy has a method to take damage
            SimpleEnemy enemy = collision.gameObject.GetComponent<SimpleEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject); // Destroy the bullet on impact
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject); // Destroy the bullet when it hits a wall
        }
    }
}
