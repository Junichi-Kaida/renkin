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

    // --- 新しいノックバック実装 ---
    private bool isKnockedBack;
    // 子クラスが移動処理を行う前にこのフラグを見て、trueなら移動処理をスキップさせること
    protected bool IsKnockedBack => isKnockedBack;

    public virtual void TakeDamage(int damage, Vector2 knockback)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. HP: {currentHealth}");

        // ダメージ点滅
        StartCoroutine(DamageFlash());

        // --- 強制ノックバック：Transform式 ---
        // 物理演算（Rigidbody）が効かないため、座標を直接動かして吹き飛ばす
        float duration = 0.25f; // 少しだけ時間を延ばす
        
        // 抵抗値を考慮して、飛ばす距離を計算
        // 10fくらいの力が来たら、5.0f くらい動くように強化 ( * 0.5f )
        Vector2 moveVector = knockback * 0.5f / knockbackResistance;
        
        TransformKnockback.Apply(transform, moveVector, duration);

        // ノックバック中はAI移動を止めるためにスタン時間をセット
        stunEndTime = Time.time + duration + 0.1f;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // IsStunned は TransformKnockback の状態も見に行く
    protected bool IsStunned
    {
        get
        {
            if (Time.time < stunEndTime) return true;
            // 念のため TransformKnockback コンポーネントの状態も確認
            var tk = GetComponent<TransformKnockback>();
            return tk != null && tk.IsKnockingBack;
        }
    }
    private float stunEndTime;

    // 旧PerformKnockbackは削除



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
