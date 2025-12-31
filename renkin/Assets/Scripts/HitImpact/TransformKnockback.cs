using UnityEngine;
using System.Collections;

/// <summary>
/// Rigidbodyを持たない、またはKinematicな敵を
/// Transform操作で無理やりノックバックさせるためのスクリプト。
/// IDamageableを持ったオブジェクトにアタッチして使うか、
/// IDamageableの実装側から呼び出して使う。
/// </summary>
public class TransformKnockback : MonoBehaviour
{
    private bool _isKnockingBack = false;

    /// <summary>
    /// 外部から呼ぶためのエントリーポイント
    /// </summary>
    /// <param name="target">動かす対象のTransform</param>
    /// <param name="forceVector">飛ばす方向と強さ</param>
    /// <param name="duration">吹き飛ぶ時間（秒）</param>
    public static void Apply(Transform target, Vector2 forceVector, float duration = 0.2f)
    {
        var component = target.GetComponent<TransformKnockback>();
        if (component == null) component = target.gameObject.AddComponent<TransformKnockback>();
        
        component.StartKnockback(forceVector, duration);
    }

    public void StartKnockback(Vector2 forceVector, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(DoKnockback(forceVector, duration));
    }

    private IEnumerator DoKnockback(Vector2 forceVector, float duration)
    {
        _isKnockingBack = true;
        float elapsed = 0f;
        
        // 初速を計算（1フレームあたりの移動量に換算するために調整）
        // forceVector は "Force" なので、ここでは距離に変換する係数を適当にかける
        // 例: 10の力なら、0.2秒で2Unit動く、など
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + (Vector3)forceVector.normalized * (forceVector.magnitude * 0.1f); 

        // 障害物チェックなどは簡易的に省略（必要ならRaycastを追加）

        while (elapsed < duration)
        {
            // Lerpで移動させる（イージングを入れると気持ちいい）
            float t = elapsed / duration;
            // EaseOutCubic: 最初早くて最後ゆっくり
            t = 1f - Mathf.Pow(1f - t, 3);
            
            transform.position = Vector3.Lerp(startPos, targetPos, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        _isKnockingBack = false;
    }

    // 敵AIが移動処理をする前にチェックするためのプロパティ
    public bool IsKnockingBack => _isKnockingBack;
}
