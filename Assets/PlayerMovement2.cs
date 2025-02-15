using UnityEngine;

public class PlayerMovement2 : MonoBehaviour
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
    public float wallJumpForceX = 8f;
    public float wallJumpForceY = 14f;
    public float wallJumpLockTime = 0.2f;
    private bool isWallSliding;
    private bool canMove = true;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool canDash = true;
    private float moveInput;
    private int lastWallDir = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpsLeft = maxJumps;
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (isDashing) return;

        // Duvar Z�plama Kontrol�
        if (Input.GetKeyDown(KeyCode.Space) && isWallSliding)
        {
            WallJump();
        }

        if (canMove)
        {
            // Hareket
            Move();

            // Normal Z�plama
            if (Input.GetKeyDown(KeyCode.Space) && (isGrounded || jumpsLeft > 0))
            {
                Jump();
            }
        }

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
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        jumpsLeft--;
    }

    void WallJump()
    {
        canMove = false; // Ge�ici olarak hareketi kapat
        int wallDir = isTouchingWall ? -lastWallDir : 0; // Duvar�n tersi y�n�
        rb.linearVelocity = new Vector2(wallDir * wallJumpForceX * -1.5f, wallDir * wallJumpForceY * -3f) ;
        Invoke(nameof(EnableMovement), wallJumpLockTime); // K�sa s�re sonra hareketi a�
    }

    void EnableMovement()
    {
        canMove = true;
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
            lastWallDir = (int)-Mathf.Sign(moveInput);
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
