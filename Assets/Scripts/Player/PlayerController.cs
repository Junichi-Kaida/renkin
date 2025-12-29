using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Visuals")]
    [SerializeField] private Sprite frontSprite;
    [SerializeField] private Sprite sideSprite;
    [SerializeField] private Sprite[] runSprites; // New: Animation frames
    [SerializeField] private float animSpeed = 0.15f;
    private SpriteRenderer sr;
    private float animTimer;
    private int currentFrame;

    private Rigidbody2D rb;
    private bool isGrounded;
    private float horizontalInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (sr == null) return;

        if (Mathf.Abs(horizontalInput) > 0.1f)
        {
            // Animation Logic
            animTimer += Time.deltaTime;
            if (animTimer >= animSpeed)
            {
                animTimer = 0;
                currentFrame++;
                
                // Construct animation sequence: Side -> Run1 -> Side -> ...
                // If we assume sideSprite is "idle/stand" and runSprites has "step" frames.
                // Simple toggle for now if runSprites has 1 element: Side, Run, Side, Run
                
                List<Sprite> animSequence = new List<Sprite>();
                if (sideSprite != null) animSequence.Add(sideSprite);
                if (runSprites != null) animSequence.AddRange(runSprites);
                
                if (animSequence.Count > 0)
                {
                    sr.sprite = animSequence[currentFrame % animSequence.Count];
                }
            }
        }
        else
        {
            // Idle: Maintain side profile (facing direction is handled by localScale)
            if (sideSprite != null) sr.sprite = sideSprite;
            animTimer = 0;
            currentFrame = 0;
        }
    }

    private void FixedUpdate()
    {
        CheckGround();
        Move();
    }

    private void Move()
    {
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);

        if (horizontalInput > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (horizontalInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    private void CheckGround()
    {
        if (groundCheck == null) return;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
