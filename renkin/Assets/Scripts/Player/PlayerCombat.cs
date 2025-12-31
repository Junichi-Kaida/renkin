using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private WeaponSO currentWeapon;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private SpriteRenderer weaponRenderer;

    private float lastAttackTime;

    private void Start()
    {
        UpdateWeaponVisual();
    }

    public void SetWeapon(WeaponSO newWeapon)
    {
        currentWeapon = newWeapon;
        UpdateWeaponVisual();
        SaveManager.Instance?.SaveGame();
    }

    private void UpdateWeaponVisual()
    {
        if (weaponRenderer != null)
        {
            weaponRenderer.sprite = currentWeapon?.weaponSprite;
            weaponRenderer.gameObject.SetActive(false); // Hide by default
        }
    }

    public WeaponSO GetCurrentWeapon() => currentWeapon;

    private void Update()
    {
        if (Time.timeScale == 0 || GetComponent<PlayerHealth>() == null) return;

        if (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.J))
        {
            TryAttack();
        }
    }

    private void TryAttack()
    {
        if (currentWeapon == null)
        {
            Debug.LogWarning("No weapon assigned to PlayerCombat! Please assign a WeaponSO in the Inspector.");
            return;
        }
        if (Time.time < lastAttackTime + currentWeapon.attackInterval) return;

        Attack();
        lastAttackTime = Time.time;
    }

    private void Attack()
    {
        Debug.Log($"Attacking with {currentWeapon.weaponName}!");
        StartCoroutine(AttackVisualFeedback());

        // Lunge forward slightly
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 lungeDir = new Vector2(transform.localScale.x, 0);
            rb.AddForce(lungeDir * 3f, ForceMode2D.Impulse);
        }

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, currentWeapon.attackRange, enemyLayer);
        Debug.Log($"Attack hit {hitEnemies.Length} objects on layer {enemyLayer.value}");

        bool hitAny = false;
        foreach (Collider2D enemy in hitEnemies)
        {
            IDamageable damageable = enemy.GetComponent<IDamageable>();
            if (damageable != null)
            {
                hitAny = true;
                Vector2 knockbackDir = (enemy.transform.position - transform.position).normalized;
                
                // ジャンプしない程度に、ほんの少しだけ浮かせて摩擦を無効化する (0.2f)
                // これがないと地面との摩擦で一瞬で止まってしまう
                knockbackDir.y = 0.2f; 
                if (knockbackDir.x == 0) knockbackDir.x = (enemy.transform.position.x > transform.position.x) ? 1 : -1;
                knockbackDir = knockbackDir.normalized;
                
                // 力の強さ
                Vector2 finalKnockback = knockbackDir * 10f; 
                damageable.TakeDamage(currentWeapon.damage, finalKnockback);
            }
        }

        // --- 攻撃ヒット演出 ---
        if (hitAny)
        {
            // ヒットストップ（0.1秒止まる）
            HitStop.Freeze(0.1f);
            // カメラシェイク（0.15秒間、強さ0.2で揺れる）
            CameraShake.Shake(0.15f, 0.2f);
        }
        // --------------------
    }

    private System.Collections.IEnumerator AttackVisualFeedback()
    {
        if (weaponRenderer != null)
        {
            weaponRenderer.gameObject.SetActive(true);
            // Swing from 45 degrees to -45 degrees relative to facing
            float startAngle = 45f;
            float endAngle = -45f;
            float duration = 0.1f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float currentAngle = Mathf.Lerp(startAngle, endAngle, elapsed / duration);
                weaponRenderer.transform.localEulerAngles = new Vector3(0, 0, currentAngle);
                elapsed += Time.deltaTime;
                yield return null;
            }
            weaponRenderer.transform.localEulerAngles = new Vector3(0, 0, endAngle);
            yield return new WaitForSeconds(0.05f);
            weaponRenderer.gameObject.SetActive(false);
        }
        else
        {
            // Fallback for visual feedback if no renderer is assigned
            transform.localEulerAngles = new Vector3(0, 0, transform.localScale.x * -20f);
            yield return new WaitForSeconds(0.1f);
            transform.localEulerAngles = Vector3.zero;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null && currentWeapon != null)
        {
            Gizmos.color = Color.yellow;
            // Draw a wireframe sphere to represent the attack hit area
            Gizmos.DrawWireSphere(attackPoint.position, currentWeapon.attackRange);
            
            // Draw a line from the player to the attack center
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, attackPoint.position);
        }
    }
}
