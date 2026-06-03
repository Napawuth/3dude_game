using UnityEngine;
using UnityEngine.InputSystem; // Required for the New Input System
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    [Header("Ground Check Settings")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.2f;

    [Header("Attack Settings")]
    public float attackDuration = 0.3f; // Set this to the approximate length of your attack animation clip

    private Rigidbody rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private float horizontalInput;
    private bool isGrounded;
    private bool jumpRequested;
    private bool isAttacking;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
    }

    void Update()
    {
        // If we are currently attacking, lock movement inputs to keep the attack grounded
        if (isAttacking)
        {
            horizontalInput = 0f;
            return;
        }

        // 1. Read Movement Input (A/D or Arrows)
        horizontalInput = 0f;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            {
                horizontalInput = 1f; 
            }
            else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            {
                horizontalInput = -1f; 
            }

            // 2. Read Jump Input (Spacebar)
            if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
            {
                jumpRequested = true;
            }
        }

        // 3. Read Attack Input (Left Mouse Button)
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && isGrounded)
        {
            StartCoroutine(PerformAttack());
        }

        // 4. Flip Character Sprite visually based on direction
        if (horizontalInput < 0)
        {
            spriteRenderer.flipX = false; 
        }
        else if (horizontalInput > 0)
        {
            spriteRenderer.flipX = true;  
        }

        // 5. Update Animator States
        bool isWalking = Mathf.Abs(horizontalInput) > 0;
        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isGrounded", isGrounded);
    }

    // Coroutine to control the attack timing cleanly
    IEnumerator PerformAttack()
    {
        isAttacking = true;
        animator.SetBool("isWalking", false);
        animator.SetBool("isAttacking", true);

        // Stop horizontal physics momentum instantly when swinging
        #if UNITY_2022_1_OR_NEWER
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        #else
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
        #endif

        // Wait for the duration of the attack swing
        yield return new WaitForSeconds(attackDuration);

        isAttacking = false;
        animator.SetBool("isAttacking", false);
    }

    void FixedUpdate()
    {
        if (isAttacking) return;

        // 1. Perform Ground Check
        Vector3 checkPosition = groundCheck != null ? groundCheck.position : transform.position - new Vector3(0, 1f, 0);
        isGrounded = Physics.OverlapSphere(checkPosition, groundCheckRadius, groundLayer).Length > 0;

        // 2. Apply Horizontal Velocity
        #if UNITY_2022_1_OR_NEWER
            rb.linearVelocity = new Vector3(horizontalInput * moveSpeed, rb.linearVelocity.y, 0f);
        #else
            rb.velocity = new Vector3(horizontalInput * moveSpeed, rb.velocity.y, 0f);
        #endif

        // 3. Apply Jump Force
        if (jumpRequested)
        {
            #if UNITY_2022_1_OR_NEWER
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, 0f);
            #else
                rb.velocity = new Vector3(rb.velocity.x, jumpForce, 0f);
            #endif
            jumpRequested = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 checkPosition = groundCheck != null ? groundCheck.position : transform.position - new Vector3(0, 1f, 0);
        Gizmos.DrawWireSphere(checkPosition, groundCheckRadius);
    }
}