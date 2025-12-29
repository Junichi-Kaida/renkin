using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    [Header("Base Stats")]
    [SerializeField] protected int maxHealth = 2;
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] protected float knockbackResistance = 1f;

    [Header("Loot")]
    [SerializeField] protected GameObject pickupPrefab;
    [SerializeField] protected MaterialSO dropItem;
    [Range(0f, 1f)] [SerializeField] protected float dropChance = 1f;

    protected int currentHealth;
    protected Rigidbody2D rb;
    protected Transform player;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Force Physics Settings at Runtime to prevent floating bugs
        if (rb != null)
        {
            rb.gravityScale = 3f;
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    public virtual void TakeDamage(int damage, Vector2 knockback)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. HP: {currentHealth}");

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockback / knockbackResistance, ForceMode2D.Impulse);

        StartCoroutine(DamageFlash());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual IEnumerator DamageFlash()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color originalColor = sr.color;
            sr.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            sr.color = originalColor;
        }
    }

    protected virtual void Die()
    {
        TryDropItem();
        Destroy(gameObject);
    }

    protected void TryDropItem()
    {
        if (pickupPrefab != null && dropItem != null && Random.value <= dropChance)
        {
            GameObject pickupObj = Instantiate(pickupPrefab, transform.position, Quaternion.identity);
            ItemPickup pickup = pickupObj.GetComponent<ItemPickup>();
            if (pickup != null)
            {
                pickup.SetItem(dropItem, 1);
            }
        }
    }
}
