using UnityEngine;

/// <summary>
/// シンプルなカメラシェイク。
/// CameraShake.Instance.Shake() で呼び出す。
/// メインカメラに直接アタッチするか、カメラの親オブジェクトにアタッチすることを推奨。
/// カメラが追従スクリプト等で動いている場合、このスクリプトはカメラ本体（子）につけ、親を動かす構成にすると競合しない。
/// </summary>
public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    private float _shakeTimeRemaining;
    private float _shakeMagnitude;
    
    // シェイク前の座標を保持（localPositionなら親がいる限り安全）
    private Vector3 _initialLocalPos;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        _initialLocalPos = transform.localPosition;
    }

    /// <summary>
    /// カメラを揺らす
    /// </summary>
    /// <param name="duration">揺れる時間（秒）</param>
    /// <param name="magnitude">揺れの強さ</param>
    public static void Shake(float duration, float magnitude)
    {
        if (Instance == null) return;
        Instance.StartShake(duration, magnitude);
    }

    public void StartShake(float duration, float magnitude)
    {
        _shakeTimeRemaining = duration;
        _shakeMagnitude = magnitude;
    }

    private void LateUpdate()
    {
        if (_shakeTimeRemaining > 0)
        {
            // ランダムな位置にずらす
            transform.localPosition = _initialLocalPos + (Vector3)(Random.insideUnitCircle * _shakeMagnitude);

            // HitStop中でも時間が進むように unscaledDeltaTime を使用
            _shakeTimeRemaining -= Time.unscaledDeltaTime;
        }
        else
        {
            // 揺れが終わったら位置を戻す
            // float比較の誤差対策などをせず単純に0以下でリセット
            if (transform.localPosition != _initialLocalPos)
            {
                transform.localPosition = _initialLocalPos;
            }
        }
    }
}
