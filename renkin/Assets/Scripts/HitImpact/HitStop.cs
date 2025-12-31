using UnityEngine;
using System.Collections;

/// <summary>
/// ヒットストップを管理するクラス。
/// 攻撃が当たった瞬間に HitStop.Freeze(0.1f) のように呼び出す。
/// </summary>
public class HitStop : MonoBehaviour
{
    // どこからでも呼べるようにシングルトン化
    private static HitStop _instance;

    private void Awake()
    {
        // 簡易的な重複防止
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    /// <summary>
    /// 指定秒数だけ時間を止める
    /// </summary>
    /// <param name="duration">停止時間（秒・Realtime）</param>
    public static void Freeze(float duration)
    {
        if (_instance == null)
        {
            Debug.LogWarning("HitStop instance not found in scene.");
            return;
        }

        // 連続ヒットした場合は前の処理をキャンセルして上書きする
        _instance.StopAllCoroutines();
        _instance.StartCoroutine(_instance.DoFreeze(duration));
    }

    private IEnumerator DoFreeze(float duration)
    {
        // 時間を止める
        Time.timeScale = 0f;

        // duration秒待つ（Time.timeScaleの影響を受けない WaitForSecondsRealtime を使う）
        yield return new WaitForSecondsRealtime(duration);

        // 時間を戻す
        Time.timeScale = 1f;
    }
}
