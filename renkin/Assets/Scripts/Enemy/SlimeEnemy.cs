using UnityEngine;

public class SlimeEnemy : EnemyBase
{
    [Header("Slime Settings")]
    [SerializeField] private int contactDamage = 1;

    private void FixedUpdate()
    {
        if (player != null)
        {
            MoveTowardsPlayer();
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            IDamageable playerDamageable = collision.gameObject.GetComponent<IDamageable>();
            if (playerDamageable != null)
            {
                Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;
                playerDamageable.TakeDamage(contactDamage, knockbackDir * 5f);
            }
        }
    }
}
