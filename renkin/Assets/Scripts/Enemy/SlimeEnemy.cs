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
        
        // Flip sprite based on direction
        // Sprite seems to be Left-Facing by default.
        // Move Right (> 0) -> Need Face Right -> Flip (Negative Scale)
        // Move Left (< 0) -> Need Face Left -> Default (Positive Scale)
        if (direction.x > 0.1f)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (direction.x < -0.1f)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
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
