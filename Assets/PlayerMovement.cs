using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float acceleration = 10f;
    public float deceleration = 10f;

    [Header("Jumping")]
    public float jumpForce = 12f;
    public int maxJumps = 2;
    private int jumpsLeft;

    [Header("Dash")]
    public float dashSpeed = 20f;
    public float dashTime = 0.2f;
    private bool isDashing;

    [Header("Wall Jumping")]
    public float wallJumpForce = 10f;
    public Vector2 wallJumpDirection = new Vector2(1, 1);

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool canDash = true;
    private bool isWallSliding;
    private float moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpsLeft = maxJumps;
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (isDashing) return;

        // Hareket
        Move();

        // Z�plama
        if (Input.GetKeyDown(KeyCode.Space))
            Jump();

        // Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
            StartCoroutine(Dash());

        // Duvar Kayma
        WallSlide();
    }

    void Move()
    {
        float targetSpeed = moveInput * moveSpeed;
        float speedDiff = targetSpeed - rb.linearVelocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        float movement = Mathf.Pow(Mathf.Abs(speedDiff) * accelRate, 0.9f) * Mathf.Sign(speedDiff);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x + movement * Time.deltaTime, rb.linearVelocity.y);
    }

    void Jump()
    {
        if (isGrounded || jumpsLeft > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpsLeft--;
        }
        else if (isTouchingWall)
        {
            rb.linearVelocity = new Vector2(-Mathf.Sign(moveInput) * wallJumpForce, jumpForce);
        }
    }

    System.Collections.IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;
        rb.linearVelocity = new Vector2(moveInput * dashSpeed, 0);
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = originalGravity;
        isDashing = false;
        canDash = true;
    }

    void WallSlide()
    {
        if (isTouchingWall && !isGrounded)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -2);
        }
        else
        {
            isWallSliding = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            jumpsLeft = maxJumps;
        }
        if (collision.gameObject.CompareTag("Wall"))
        {
            isTouchingWall = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
        if (collision.gameObject.CompareTag("Wall"))
        {
            isTouchingWall = false;
        }
    }
}
