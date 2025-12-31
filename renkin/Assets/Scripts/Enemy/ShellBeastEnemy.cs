using UnityEngine;
using System.Collections;

public class ShellBeastEnemy : EnemyBase
{
    private enum State { Idle, Charging, Dashing, Staggered }

    [Header("ShellBeast Settings")]
    [SerializeField] private float detectionRange = 7f;
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float chargeDuration = 1.0f;
    [SerializeField] private float staggerDuration = 1.0f;
    [SerializeField] private int contactDamage = 1;

    private State currentState = State.Idle;
    private Vector2 dashDirection;

    private void Update()
    {
        if (player == null) return;

        if (currentState == State.Idle)
        {
            if (Vector2.Distance(transform.position, player.position) < detectionRange)
            {
                StartCoroutine(ChargeAndDash());
            }
        }
    }

    private IEnumerator ChargeAndDash()
    {
        currentState = State.Charging;
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(chargeDuration);

        currentState = State.Dashing;
        dashDirection = (player.position - transform.position).normalized;
        dashDirection.y = 0;
        dashDirection.Normalize();

        float dashTimer = 0;
        while (currentState == State.Dashing && dashTimer < 2.0f)
        {
            if(!IsStunned)
            {
                rb.linearVelocity = dashDirection * dashSpeed;
            }
            dashTimer += Time.deltaTime;
            yield return null;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentState == State.Dashing)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                IDamageable playerDamageable = collision.gameObject.GetComponent<IDamageable>();
                if (playerDamageable != null)
                {
                    playerDamageable.TakeDamage(contactDamage, dashDirection * 10f);
                }
            }
            else
            {
                StartCoroutine(Stagger());
            }
        }
    }

    private IEnumerator Stagger()
    {
        currentState = State.Staggered;
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(staggerDuration);
        currentState = State.Idle;
    }
}
