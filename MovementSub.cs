
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////SETUP INSTRUCTIONS//////////
//Attach this script a RigidBody2D to the player GameObject
//Set Body type to Dynamic, Collision detection to continuous and Freeze Z rotation
//Add a 2D Collider (Any will do, but 2D box collider)
//Define the ground and wall mask layers (In the script and in the GameObjects)
//Adjust and play around with the other variables (Some require you to activate gizmos in order to visualize)


//make movement dependent on isAtk



public class MovementSub : MonoBehaviour {
    [Header("General")]
    public bool isAtk;

    [Header("Components")]
    private Rigidbody2D rb;
    //private Animator anim;

    [Header("Layer Masks")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask cornerCorrectLayer;

    [Header("Movement Variables")]
    [SerializeField] private float movementAcceleration = 70f;
    [SerializeField] private float maxMoveSpeed = 12f;
    [SerializeField] private float groundLinearDrag = 7f;
    private float horizontalDirection;
    private float verticalDirection;
    private bool changingDirection => (rb.velocity.x > 0f && horizontalDirection < 0f) || (rb.velocity.x < 0f && horizontalDirection > 0f);
    private bool facingRight = true;
    private bool canMove = true;

    [Header("Jump Variables")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float airLinearDrag = 2.5f;
    [SerializeField] private float fallMultiplier = 8f;
    [SerializeField] private float lowJumpFallMultiplier = 5f;
    [SerializeField] private float downMultiplier = 12f;
    [SerializeField] private int extraJumps = 1;
    [SerializeField] private float hangTime = .1f;
    [SerializeField] private float jumpBufferLength = .1f;
    private int extraJumpsValue;
    private float hangTimeCounter;
    private float jumpBufferCounter;
    private bool canJump => jumpBufferCounter > 0f && (hangTimeCounter > 0f || extraJumpsValue > 0 || onWall);
    private bool isJumping = false;

    [Header("Wall Movement Variables")]
    [SerializeField] private float wallSlideModifier = 0.5f;
    [SerializeField] private float wallJumpXVelocityHaltDelay = 0.2f;

    private bool wallSlide => onWall && !onGround && rb.velocity.y < 0f;


    [Header("Ground Collision Variables")]
    [SerializeField] private float groundRaycastLength;
    [SerializeField] private Vector3 groundRaycastOffset;
    private bool onGround;

    [Header("Wall Collision Variables")]
    [SerializeField] private float wallRaycastLength;
    [SerializeField] private bool onWall;
    [SerializeField] private bool onRightWall;

    [Header("Corner Correction Variables")]
    [SerializeField] private float topRaycastLength;
    [SerializeField] private Vector3 edgeRaycastOffset;
    [SerializeField] private Vector3 innerRaycastOffset;
    private bool canCornerCorrect;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        //anim = GetComponent<Animator>();
    }

    private void Update() {
        horizontalDirection = GetInput().x;
        verticalDirection = GetInput().y;
        if (Input.GetButtonDown("Jump")) {
            jumpBufferCounter = jumpBufferLength;
        } else {
            jumpBufferCounter -= Time.deltaTime;
        }

    }

    private void FixedUpdate() {
        CheckCollisions();


        if (canMove) MoveCharacter();
        else rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(horizontalDirection * maxMoveSpeed, rb.velocity.y)), .5f * Time.deltaTime);
        if (onGround) {
            ApplyGroundLinearDrag();
            extraJumpsValue = extraJumps;
            hangTimeCounter = hangTime;

        } else {
            ApplyAirLinearDrag();
            FallMultiplier();
            hangTimeCounter -= Time.fixedDeltaTime;
            if (!onWall || rb.velocity.y < 0f) isJumping = false;
        }
        if (canJump) {
            if (onWall && !onGround) {
                if (onRightWall && horizontalDirection > 0f || !onRightWall && horizontalDirection < 0f) {
                    StartCoroutine(NeutralWallJump());
                } else {
                    WallJump();
                }
                Flip();
            } else {
                Jump(Vector2.up);
            }
        }
        if (!isJumping) {
            if (wallSlide) WallSlide();
            if (onWall) StickToWall();
        }

        if (canCornerCorrect) CornerCorrect(rb.velocity.y);
    }

    private Vector2 GetInput() {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    private void MoveCharacter() {
        rb.AddForce(new Vector2(horizontalDirection, 0f) * movementAcceleration);

        if (Mathf.Abs(rb.velocity.x) > maxMoveSpeed)
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxMoveSpeed, rb.velocity.y);
    }

    private void ApplyGroundLinearDrag() {
        if (Mathf.Abs(horizontalDirection) < 0.4f || changingDirection) {
            rb.drag = groundLinearDrag;
        } else {
            rb.drag = 0f;
        }
    }

    private void ApplyAirLinearDrag() {
        rb.drag = airLinearDrag;
    }

    private void Jump(Vector2 direction) {
        if (!onGround && !onWall)
            extraJumpsValue--;

        ApplyAirLinearDrag();
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(direction * jumpForce, ForceMode2D.Impulse);
        hangTimeCounter = 0f;
        jumpBufferCounter = 0f;
        isJumping = true;
    }

    private void WallJump() {
        Vector2 jumpDirection = onRightWall ? Vector2.left : Vector2.right;
        Jump(Vector2.up + jumpDirection);
    }

    IEnumerator NeutralWallJump() {
        Vector2 jumpDirection = onRightWall ? Vector2.left : Vector2.right;
        Jump(Vector2.up + jumpDirection);
        yield return new WaitForSeconds(wallJumpXVelocityHaltDelay);
        rb.velocity = new Vector2(0f, rb.velocity.y);
    }

    private void FallMultiplier() {
        if (verticalDirection < 0f) {
            rb.gravityScale = downMultiplier;
        } else {
            if (rb.velocity.y < 0) {
                rb.gravityScale = fallMultiplier;
            } else if (rb.velocity.y > 0 && !Input.GetButton("Jump")) {
                rb.gravityScale = lowJumpFallMultiplier;
            } else {
                rb.gravityScale = 1f;
            }
        }
    }

    void WallSlide() {
        rb.velocity = new Vector2(rb.velocity.x, -maxMoveSpeed * wallSlideModifier);
    }



    void StickToWall() {
        //Push player torwards wall
        if (onRightWall && horizontalDirection >= 0f) {
            rb.velocity = new Vector2(1f, rb.velocity.y);
        } else if (!onRightWall && horizontalDirection <= 0f) {
            rb.velocity = new Vector2(-1f, rb.velocity.y);
        }

        //Face correct direction
        if (onRightWall && !facingRight) {
            Flip();
        } else if (!onRightWall && facingRight) {
            Flip();
        }
    }

    void Flip() {
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
    }


    /*
    void Animation() {
        if (isDashing) {
            anim.SetBool("isDashing", true);
            anim.SetBool("isGrounded", false);
            anim.SetBool("isFalling", false);
            anim.SetBool("WallGrab", false);
            anim.SetBool("isJumping", false);
            anim.SetFloat("horizontalDirection", 0f);
            anim.SetFloat("verticalDirection", 0f);
        } else {
            anim.SetBool("isDashing", false);

            if ((horizontalDirection < 0f && facingRight || horizontalDirection > 0f && !facingRight) && !wallGrab && !wallSlide) {
                Flip();
            }
            if (onGround) {
                anim.SetBool("isGrounded", true);
                anim.SetBool("isFalling", false);
                anim.SetBool("WallGrab", false);
                anim.SetFloat("horizontalDirection", Mathf.Abs(horizontalDirection));
            } else {
                anim.SetBool("isGrounded", false);
            }
            if (isJumping) {
                anim.SetBool("isJumping", true);
                anim.SetBool("isFalling", false);
                anim.SetBool("WallGrab", false);
                anim.SetFloat("verticalDirection", 0f);
            } else {
                anim.SetBool("isJumping", false);

                if (wallGrab || wallSlide) {
                    anim.SetBool("WallGrab", true);
                    anim.SetBool("isFalling", false);
                    anim.SetFloat("verticalDirection", 0f);
                } else if (rb.velocity.y < 0f) {
                    anim.SetBool("isFalling", true);
                    anim.SetBool("WallGrab", false);
                    anim.SetFloat("verticalDirection", 0f);
                }
                if (wallRun) {
                    anim.SetBool("isFalling", false);
                    anim.SetBool("WallGrab", false);
                    anim.SetFloat("verticalDirection", Mathf.Abs(verticalDirection));
                }
            }
        }
    }
    */
    void CornerCorrect(float Yvelocity) {
        //Push player to the right
        RaycastHit2D hit = Physics2D.Raycast(transform.position - innerRaycastOffset + Vector3.up * topRaycastLength, Vector3.left, topRaycastLength, cornerCorrectLayer);
        if (hit.collider != null) {
            float newPos = Vector3.Distance(new Vector3(hit.point.x, transform.position.y, 0f) + Vector3.up * topRaycastLength,
                transform.position - edgeRaycastOffset + Vector3.up * topRaycastLength);
            transform.position = new Vector3(transform.position.x + newPos, transform.position.y, transform.position.z);
            rb.velocity = new Vector2(rb.velocity.x, Yvelocity);
            return;
        }

        //Push player to the left
        hit = Physics2D.Raycast(transform.position + innerRaycastOffset + Vector3.up * topRaycastLength, Vector3.right, topRaycastLength, cornerCorrectLayer);
        if (hit.collider != null) {
            float newPos = Vector3.Distance(new Vector3(hit.point.x, transform.position.y, 0f) + Vector3.up * topRaycastLength,
                transform.position + edgeRaycastOffset + Vector3.up * topRaycastLength);
            transform.position = new Vector3(transform.position.x - newPos, transform.position.y, transform.position.z);
            rb.velocity = new Vector2(rb.velocity.x, Yvelocity);
        }
    }

    private void CheckCollisions() {


        //Ground Collisions
        onGround = Physics2D.Raycast(transform.position + groundRaycastOffset, Vector2.down, groundRaycastLength, groundLayer) ||
                   Physics2D.Raycast(transform.position - groundRaycastOffset, Vector2.down, groundRaycastLength, groundLayer);

        //Corner Collisions
        canCornerCorrect = Physics2D.Raycast(transform.position + edgeRaycastOffset, Vector2.up, topRaycastLength, cornerCorrectLayer) &&
                           !Physics2D.Raycast(transform.position + innerRaycastOffset, Vector2.up, topRaycastLength, cornerCorrectLayer) ||
                           Physics2D.Raycast(transform.position - edgeRaycastOffset, Vector2.up, topRaycastLength, cornerCorrectLayer) &&
                           !Physics2D.Raycast(transform.position - innerRaycastOffset, Vector2.up, topRaycastLength, cornerCorrectLayer);

        //Wall Collisions
        onWall = Physics2D.Raycast(transform.position, Vector2.right, wallRaycastLength, wallLayer) ||
                   Physics2D.Raycast(transform.position, Vector2.left, wallRaycastLength, wallLayer);
        onRightWall = Physics2D.Raycast(transform.position, Vector2.right, wallRaycastLength, wallLayer);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;

        //Ground Check
        Gizmos.DrawLine(transform.position + groundRaycastOffset, transform.position + groundRaycastOffset + Vector3.down * groundRaycastLength);
        Gizmos.DrawLine(transform.position - groundRaycastOffset, transform.position - groundRaycastOffset + Vector3.down * groundRaycastLength);

        //Corner Check
        Gizmos.DrawLine(transform.position + edgeRaycastOffset, transform.position + edgeRaycastOffset + Vector3.up * topRaycastLength);
        Gizmos.DrawLine(transform.position - edgeRaycastOffset, transform.position - edgeRaycastOffset + Vector3.up * topRaycastLength);
        Gizmos.DrawLine(transform.position + innerRaycastOffset, transform.position + innerRaycastOffset + Vector3.up * topRaycastLength);
        Gizmos.DrawLine(transform.position - innerRaycastOffset, transform.position - innerRaycastOffset + Vector3.up * topRaycastLength);

        //Corner Distance Check
        Gizmos.DrawLine(transform.position - innerRaycastOffset + Vector3.up * topRaycastLength,
                        transform.position - innerRaycastOffset + Vector3.up * topRaycastLength + Vector3.left * topRaycastLength);
        Gizmos.DrawLine(transform.position + innerRaycastOffset + Vector3.up * topRaycastLength,
                        transform.position + innerRaycastOffset + Vector3.up * topRaycastLength + Vector3.right * topRaycastLength);

        //Wall Check
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * wallRaycastLength);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.left * wallRaycastLength);
    }
}