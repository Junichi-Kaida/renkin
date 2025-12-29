using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private float invincibilityDuration = 0.5f;

    private int currentHealth;
    private bool isInvincible;
    private bool isDead;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage, Vector2 knockback)
    {
        if (isInvincible || isDead) return;

        currentHealth -= damage;
        Debug.Log($"Player hit! HP: {currentHealth}");

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockback, ForceMode2D.Impulse);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(BecomeInvincible());
        }
    }

    private IEnumerator BecomeInvincible()
    {
        isInvincible = true;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        float timer = 0;
        while (timer < invincibilityDuration && !isDead)
        {
            if (sr != null) sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(0.1f);
            timer += 0.1f;
        }
        if (sr != null) sr.enabled = true;
        isInvincible = false;
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;
        Debug.Log("Player Died!");
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
