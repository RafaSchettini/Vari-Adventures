using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    public LevelRestart levelRestart;
    
    private Rigidbody2D rb2d;
    
    private Animator anim;

    public TextMeshProUGUI dashCoolDownText;

    private float movementInputDirection;
    private float jumpTimer;
    private float turnTimer;
    private float wallJumpTimer;
    private float dashTimeLeft;
    private float lastImageXpos;
    private float lastDash;

    private float dashCoolDownUI;

    private int amountOfJumpsLeft;
    private int facingDirection = 1;
    private int lastWallJumpDirection;

    private bool isFacingRight = true;
    private bool isGrounded;
    private bool canNormalJump;
    private bool canWallJump;
    private bool isWalking;
    private bool isWallSliding;
    private bool isTouchingWall;
    private bool isAttemptingToJump;
    private bool checkJumpMultiplier;
    private bool canMove;
    private bool canFlip;
    private bool hasWallJumped;
    
    public bool isDashing;
    public bool canDash;
    
    public int amountOfJumps = 2;

    public float movementSpeed;
    public float jumpForce;
    public float groundCheckRadius;
    public float wallCheckDistance;
    public float wallSlideSpeed;
    public float movementForceInAir;
    public float airDragMultiplier = 0.95f;
    public float wallHopForce;
    public float wallJumpForce;
    public float variableJumpHeightMultiplier = 0.5f;
    public float jumpTimerSet = 0.15f;
    public float turnTimerSet = 0.1f;
    public float wallJumpTimerSet = 0.5f;
    public float dashTime;
    public float dashSpeed;
    public float distanceBetweenImages;
    public float dashCoolDown;

    public Vector2 wallHopDirection;
    public Vector2 wallJumpDirection;

    public AudioSource footstepsSFX;

    public ParticleSystem dust;
    public ParticleSystem ps;

    public Transform groundCheck;
    public Transform wallCheck;
    
    public LayerMask whatIsGround;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        footstepsSFX = GetComponent<AudioSource>();

        amountOfJumpsLeft = amountOfJumps;
        
        wallHopDirection.Normalize();
        wallJumpDirection.Normalize();

        var em = ps.emission;
        em.enabled = false;

        dashCoolDownUI = dashCoolDown - Time.time;

    }

    // Update is called once per frame
    void Update()
    {
        //RestartLevelDebug();
        CheckInput();
        CheckMovementDirection();
        CheckIfCanJump();
        UpdateAnimations();
        CheckIfWallSliding();
        CheckJump();
        CheckDash();
        
        dashCoolDownText.text = dashCoolDownUI.ToString();

    }

    private void FixedUpdate()
    {
        ApplyMovement();
        CheckSurroundings();
    }

    private void CheckIfWallSliding()
    {
        if (isTouchingWall && movementInputDirection == facingDirection && rb2d.velocity.y < 0)
        {
            isWallSliding = true;
        }

        else
        {
            isWallSliding = false;
        }
    }

    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);
    }

    private void CheckIfCanJump()
    {
        if (isGrounded && rb2d.velocity.y <= 0.01f)
        {
            amountOfJumpsLeft = amountOfJumps;
        }

        if (isTouchingWall)
        {
            checkJumpMultiplier = false;
            canWallJump = true;
        }

        if (amountOfJumpsLeft <= 0)
        {
            canNormalJump = false;
        }

        else
        {
            canNormalJump = true;
        }
    }

    private void CheckMovementDirection()
    {
        if (isFacingRight && movementInputDirection < 0)
        {
            Flip();
        }

        else if (!isFacingRight && movementInputDirection > 0)
        {
            Flip();
        }

        if (rb2d.velocity.x != 0)
        {
            isWalking = true;
        }

        else
        {
            isWalking = false;
        }
    }

    private void UpdateAnimations()
    {
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb2d.velocity.y);
        anim.SetBool("isWallSliding", isWallSliding);
    }

    private void CheckInput()
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded || (amountOfJumpsLeft > 0 && isTouchingWall))
            {
                NormalJump();
            }

            else 
            {
                jumpTimer = jumpTimerSet;
                isAttemptingToJump = true;
            }
        }

        if (Input.GetButtonDown("Horizontal") && isTouchingWall)
        {
            if (!isGrounded && movementInputDirection != facingDirection)
            {
                canMove = false;
                canFlip = false;

                turnTimer = turnTimerSet;
            }
        }

        if (!canMove)
        {
            turnTimer -= Time.deltaTime;

            if (turnTimer <= 0)
            {
                canMove = true;
                canFlip = true;
            }
        }

        if (checkJumpMultiplier && !Input.GetButton("Jump"))
        {
            checkJumpMultiplier = false;
            rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y * variableJumpHeightMultiplier); 
        } 

        if (Input.GetButtonDown("Dash"))
        {
            if(Time.time >= (lastDash + dashCoolDown))
            {
                AttemptToDash();
            }
            
        }
    }

    public void AttemptToDash()
    {
        if (canDash)
        {
            CreateDust();
            isDashing = true;
            dashTimeLeft = dashTime;
            lastDash = Time.time;

            PlayerAfterImagePool.Instance.GetFromPool();
            lastImageXpos = transform.position.x;
        }
        
    }

    private void CheckDash()
    {
        if (isDashing)
        {

            if (dashTimeLeft > 0)
            {
                canMove = false;
                canFlip = false;
                rb2d.velocity = new Vector2(dashSpeed * facingDirection, 0);
                dashTimeLeft -= Time.deltaTime;

                if (Mathf.Abs(transform.position.x - lastImageXpos) > distanceBetweenImages)
                {
                    PlayerAfterImagePool.Instance.GetFromPool();
                    lastImageXpos = transform.position.x;
                }
            }

            if (dashTimeLeft <= 0 || isTouchingWall)
            {
                isDashing = false;
                canMove = true;
                canFlip = true;
            }

        }
    }

    private void CheckJump()
    {
        if (jumpTimer > 0)
        {
            if (!isGrounded && isTouchingWall && movementInputDirection != 0 && movementInputDirection != facingDirection)
            {
                WallJump();
            }

            else if (isGrounded)
            {
                NormalJump();
            }
        }

        if (isAttemptingToJump)
        {
            jumpTimer -= Time.deltaTime;
        }

        if (wallJumpTimer > 0)
        {
            if(hasWallJumped && movementInputDirection == -lastWallJumpDirection)
            {
                rb2d.velocity = new Vector2(rb2d.velocity.x, 0.0f);
                hasWallJumped = false;
            }
            
            else if (wallJumpTimer <= 0)
            {
                hasWallJumped = false;
            }
            
            else
            {
                wallJumpTimer -= Time.deltaTime;
            }
        }
    }

    private void NormalJump()
    {
        if (canNormalJump)
        {
            CreateDust();
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpForce);
            amountOfJumpsLeft--;
            jumpTimer = 0;
            isAttemptingToJump = false;
            checkJumpMultiplier = true;
        }
    }

    private void WallJump()
    {
        if (canWallJump)
        {
            CreateDust();
            rb2d.velocity = new Vector2 (rb2d.velocity.x, 0.0f);
            isWallSliding = false;
            amountOfJumpsLeft = amountOfJumps;
            amountOfJumpsLeft--;
            Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x * movementInputDirection, wallJumpForce * wallJumpDirection.y);
            rb2d.AddForce(forceToAdd, ForceMode2D.Impulse);
            jumpTimer = 0;
            isAttemptingToJump = false;
            checkJumpMultiplier = true;
            turnTimer = 0;
            canMove = true;
            canFlip = true;
            hasWallJumped = true;
            wallJumpTimer = wallJumpTimerSet;
            lastWallJumpDirection = -facingDirection;
        }
    }

    private void ApplyMovement()
    {

        if (!isGrounded && !isWallSliding && movementInputDirection == 0)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x * airDragMultiplier, rb2d.velocity.y);
        }

        else if (canMove)
        {
            rb2d.velocity = new Vector2(movementSpeed * movementInputDirection, rb2d.velocity.y);
        }

        if (isWallSliding)
        {
            if (rb2d.velocity.y < -wallSlideSpeed)
            {
                rb2d.velocity = new Vector2(rb2d.velocity.x, -wallSlideSpeed);
            }
        } 
    }

    private void Flip()
    {
        if (!isWallSliding && canFlip)
        {
            CreateDust();
            facingDirection *= -1;
            isFacingRight = !isFacingRight;
            transform.Rotate(0.0f, 180f, 0.0f);
        }
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
    }

    private void CreateDust()
    {
        dust.Play();
    }

    private void Footsteps()
    {
        footstepsSFX.Play();
    }

    /*LEMBRAR DE TIRAR NA BUILD FINAL DE JOGO, ~FUNÇÃO DE DESENVOLVEDOR~
    public void RestartLevelDebug()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("Tutorial");
        }
    }
    */

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Spikes"))
        {

            levelRestart.RestartScene();

            Destroy(gameObject);
            
            var em = ps.emission;
            em.enabled = true;

            Instantiate(ps, transform.position, Quaternion.identity);


        }
    }

}
