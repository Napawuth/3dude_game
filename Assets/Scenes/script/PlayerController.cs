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

    // --- ADDED FOR AUDIO TEAMWORK ---
    [Header("Audio Settings")]
    [SerializeField] private PlayerAudioController audioController;
    // ---------------------------------

    [Header("Ability Settings")]
    public float utilityCooldown = 12f;
    public float superCooldown = 30f;
    public GameObject superProjectilePrefab;
    public Transform superSpawnPoint;

    [Header("Respawn Settings")]
    public Transform respawnPoint;
    public float fallThreshold = -20f;

    private bool isUtilityReady = false;
    private bool isSuperReady = false;
    private bool isOnUtilityCooldown = false;
    private bool isOnSuperCooldown = false;

    private PlayerHealth playerHealth;

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

        // --- ADDED FOR AUDIO TEAMWORK ---
        // Automatically attempts to find the sound controller if forgot to assign in inspector
        if (audioController == null)
        {
            audioController = GetComponent<PlayerAudioController>();
        }
        // ---------------------------------

        playerHealth = GetComponent<PlayerHealth>();
        StartCoroutine(InitialCooldowns());
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
            {
                jumpRequested = true;
                if (audioController != null) audioController.PlayJumpSound();
            }

            if (Keyboard.current.slashKey.wasPressedThisFrame && isGrounded && !isAttacking && !isCoolingDown && !spriteRenderer.flipX)
                StartCoroutine(PerformAttack());

            if (Keyboard.current.periodKey.wasPressedThisFrame && isUtilityReady && !isOnUtilityCooldown)
                StartCoroutine(PerformUtility());

            if (Keyboard.current.commaKey.wasPressedThisFrame && isSuperReady && !isOnSuperCooldown && !spriteRenderer.flipX)
                StartCoroutine(PerformSuper());

            if (transform.position.y < fallThreshold)
                Respawn();
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

   
        if (audioController != null) audioController.PlayAttackSound();


        if (formatAttackPrefab != null && attackSpawnPoint != null)
        {
            GameObject projectile = Instantiate(formatAttackPrefab, attackSpawnPoint.position, Quaternion.identity);
            FormatProjectile fp = projectile.GetComponent<FormatProjectile>();
            if (fp != null)
            {
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

    IEnumerator PerformUtility()
    {
        isUtilityReady = false;
        isOnUtilityCooldown = true;

        playerHealth.ActivateShield();
        statusBar.TriggerUtility(utilityCooldown);

        yield return new WaitForSeconds(utilityCooldown);

        isOnUtilityCooldown = false;
        isUtilityReady = true;
    }

    IEnumerator PerformSuper()
    {
        isSuperReady = false;
        isOnSuperCooldown = true;
        isAttacking = true;

        animator.SetBool("isAttacking", true);

        if (superProjectilePrefab != null && superSpawnPoint != null)
        {
            GameObject projectile = Instantiate(superProjectilePrefab, superSpawnPoint.position, Quaternion.identity);
            FormatProjectile fp = projectile.GetComponent<FormatProjectile>();
            Debug.Log("FormatProjectile found: " + (fp != null));
            if (fp != null)
            {
                fp.damage = 180f;
                int direction = spriteRenderer.flipX ? 1 : -1;
                fp.SetDirection(direction);
                Debug.Log("Super direction set to: " + direction);
            }
        }

        statusBar.TriggerSuper(superCooldown);

        yield return new WaitForSeconds(attackDuration);
        animator.SetBool("isAttacking", false);
        isAttacking = false;

        yield return new WaitForSeconds(superCooldown - attackDuration);
        isOnSuperCooldown = false;
        isSuperReady = true;
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

    private void Respawn()
    {
        rb.linearVelocity = Vector3.zero;
        transform.position = respawnPoint.position;
    }

    private IEnumerator InitialCooldowns()
    {
        // Both start no ready, count down from full cooldown
        statusBar.TriggerUtility(utilityCooldown);
        statusBar.TriggerSuper(superCooldown);

        yield return new WaitForSeconds(utilityCooldown);
        isUtilityReady = true;

        yield return new WaitForSeconds(superCooldown - utilityCooldown);
        isSuperReady = true;
    }
}