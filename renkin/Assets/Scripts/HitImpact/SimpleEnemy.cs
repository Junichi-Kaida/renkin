using UnityEngine;

/// <summary>
/// 敵キャラクターの実装例。
/// Rigidbody2Dが必須。
/// </summary>
[RequireComponent(typeof(Rigidbody2D))] // ノックバックに必須
public class SimpleEnemy : MonoBehaviour, IDamageable
{
    [SerializeField] private int hp = 100;
    
    // ダメージ後の無敵時間（連続ヒットしすぎ防止用）
    [SerializeField] private float invincibilityDuration = 0.2f;
    private float _lastDamageTime;

    private Rigidbody2D _rb;
    private SpriteRenderer _sprite; // 被弾時の点滅用（任意）

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sprite = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int damage, Vector2 knockbackForce)
    {
        // 無敵時間チェック
        if (Time.time < _lastDamageTime + invincibilityDuration) return;
        _lastDamageTime = Time.time;

        // HPを減らす
        hp -= damage;

        // ノックバック処理
        // 1. 一旦停止させてから力を加えると「ドンッ」という重みが出る
        _rb.linearVelocity = Vector2.zero; 
        // 2. 力を加える (Impulse = 瞬間的な力)
        _rb.AddForce(knockbackForce, ForceMode2D.Impulse);

        // デバッグログ
        Debug.Log($"{name} took {damage} damage! HP: {hp}");

        // 死亡判定
        if (hp <= 0)
        {
            Die();
        }
        else
        {
            // 被弾演出（白く光るなど）
            StartCoroutine(FlashColor());
        }
    }

    private void Die()
    {
        // 死亡演出（パーティクル生成など）
        Destroy(gameObject);
    }

    // おまけ：被弾時に一瞬赤くする演出
    private System.Collections.IEnumerator FlashColor()
    {
        if (_sprite == null) yield break;
        Color original = _sprite.color;
        _sprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        _sprite.color = original;
    }
}
