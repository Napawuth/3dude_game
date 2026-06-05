using UnityEngine;
using UnityEngine.InputSystem;
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
    public float attackDuration = 0.3f;
    public float attackCooldown = 1.5f;
    public GameObject formatAttackPrefab;
    public Transform attackSpawnPoint;

    [Header("UI")]
    [SerializeField] private PlayerStatusBar statusBar;

    private Rigidbody rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private float horizontalInput;
    private bool isGrounded;
    private bool jumpRequested;
    private bool isAttacking;
    private bool isCoolingDown;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
    }

    void Update()
    {
        if (isAttacking)
        {
            horizontalInput = 0f;
            return;
        }

        horizontalInput = 0f;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.leftArrowKey.isPressed)
                horizontalInput = 1f;
            else if (Keyboard.current.rightArrowKey.isPressed)
                horizontalInput = -1f;

            if (Keyboard.current.upArrowKey.wasPressedThisFrame && isGrounded)
                jumpRequested = true;

            if (Keyboard.current.slashKey.wasPressedThisFrame && isGrounded && !isAttacking && !isCoolingDown)
                StartCoroutine(PerformAttack());
        }

        if (horizontalInput < 0)
            spriteRenderer.flipX = false;
        else if (horizontalInput > 0)
            spriteRenderer.flipX = true;

        bool isWalking = Mathf.Abs(horizontalInput) > 0;
        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isGrounded", isGrounded);
    }

    IEnumerator PerformAttack()
    {
        isAttacking = true;
        isCoolingDown = true;
        animator.SetBool("isWalking", false);
        animator.SetBool("isAttacking", true);

        if (formatAttackPrefab != null && attackSpawnPoint != null)
        {
            GameObject projectile = Instantiate(formatAttackPrefab, attackSpawnPoint.position, Quaternion.identity);
            FormatProjectile fp = projectile.GetComponent<FormatProjectile>();
            if (fp != null)
            {
                // spriteRenderer.flipX = true means facing left which = -1, otherwise = 1
                int direction = spriteRenderer.flipX ? 1 : -1;
                fp.SetDirection(direction);
            }
        }
        statusBar.TriggerAttackCooldown(attackCooldown);

        yield return new WaitForSeconds(attackDuration);
        animator.SetBool("isAttacking", false);
        isAttacking = false;

        yield return new WaitForSeconds(attackCooldown - attackDuration);
        isCoolingDown = false;
    }

    void FixedUpdate()
    {
        if (isAttacking) return;

        Vector3 checkPosition = groundCheck != null ? groundCheck.position : transform.position - new Vector3(0, 1f, 0);
        isGrounded = Physics.OverlapSphere(checkPosition, groundCheckRadius, groundLayer).Length > 0;

        #if UNITY_2022_1_OR_NEWER
            rb.linearVelocity = new Vector3(horizontalInput * moveSpeed, rb.linearVelocity.y, 0f);
        #else
            rb.velocity = new Vector3(horizontalInput * moveSpeed, rb.velocity.y, 0f);
        #endif

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