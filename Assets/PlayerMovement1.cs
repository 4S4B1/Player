using UnityEngine;

public class PlayerMovement1 : MonoBehaviour
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

    [Header("Wall Jumping & Sliding")]
    public float wallSlideSpeed = 2f;
    public float wallJumpForceX = 10f;
    public float wallJumpForceY = 12f;
    public float wallJumpCooldown = 0.2f;
    private bool isWallSliding;
    private bool canWallJump = true;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool canDash = true;
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

        // Duvar Kayma & Z�plama
        WallSlide();
        if (Input.GetKeyDown(KeyCode.Space) && isWallSliding && canWallJump)
            WallJump();
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
    }

    void WallJump()
    {
        canWallJump = false;
        float direction = -Mathf.Sign(moveInput); // Duvar�n ters y�n�ne z�plamak i�in
        rb.linearVelocity = new Vector2(direction * wallJumpForceX, wallJumpForceY);
        Invoke(nameof(ResetWallJump), wallJumpCooldown);
    }

    void ResetWallJump()
    {
        canWallJump = true;
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
        if (isTouchingWall && !isGrounded && moveInput != 0)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -wallSlideSpeed);
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
