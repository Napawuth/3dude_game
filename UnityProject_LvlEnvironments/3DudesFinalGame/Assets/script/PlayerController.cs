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
    public GameObject cropAttackPrefab;
    public Transform attackSpawnPoint;

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
        if (isAttacking)
        {
            horizontalInput = 0f;
            return;
        }

        horizontalInput = 0f;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.leftArrowKey.isPressed)
            {
                horizontalInput = 1f;
                Debug.Log("Left arrow pressed");
            }
            else if (Keyboard.current.rightArrowKey.isPressed)
            {
                horizontalInput = -1f;
                Debug.Log("Right arrow pressed");
            }

            if (Keyboard.current.aKey.isPressed)
                Debug.Log("A key still being read!");

            if (Keyboard.current.slashKey.wasPressedThisFrame && isGrounded && !isAttacking)
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
        animator.SetBool("isWalking", false);
        animator.SetBool("isAttacking", true);

        if (cropAttackPrefab != null && attackSpawnPoint != null)
            Instantiate(cropAttackPrefab, attackSpawnPoint.position, Quaternion.identity);

        #if UNITY_2022_1_OR_NEWER
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        #else
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
        #endif

        yield return new WaitForSeconds(attackDuration);

        isAttacking = false;
        animator.SetBool("isAttacking", false);
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