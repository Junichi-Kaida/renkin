using UnityEngine;

/// <summary>
/// プレイヤーの攻撃判定（剣など）につけるスクリプトの例。
/// </summary>
public class PlayerAttackExample : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private int damage = 10;
    [SerializeField] private float knockbackPower = 10f;
    
    [Header("Impact Settings")]
    [SerializeField] private float hitStopDuration = 0.1f;  // ヒットストップの時間
    [SerializeField] private float screenShakeDuration = 0.15f; // シェイク時間
    [SerializeField] private float screenShakeMagnitude = 0.2f; // シェイクの強さ

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 相手が IDamageable を持っているか確認
        if (other.TryGetComponent<IDamageable>(out var target))
        {
            // 1. ノックバック方向の計算（自分 -> 相手）
            Vector2 direction = (other.transform.position - transform.position).normalized;
            Vector2 knockback = direction * knockbackPower;

            // 2. ダメージとノックバックを与える
            target.TakeDamage(damage, knockback);

            // 3. ヒットストップ演出 (staticメソッド呼び出し)
            HitStop.Freeze(hitStopDuration);

            // 4. カメラシェイク演出 (staticメソッド呼び出し)
            // シーン内に CameraShake が存在しなくてもエラーにならないようにnullチェック推奨だが、
            // 今回の CameraShake.Shake は内部でチェックしているのでそのままでOK
            CameraShake.Shake(screenShakeDuration, screenShakeMagnitude);
        }
    }
}
