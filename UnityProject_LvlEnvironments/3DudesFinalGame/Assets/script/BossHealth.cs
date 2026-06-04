using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyHealth : MonoBehaviour
{
    public float maxHP = 500f;

    [Header("Phase Sprites")]
    public Sprite phase1Sprite;
    public Sprite phase2Sprite;
    public Sprite phase3Sprite;

    [Header("Phase Positions")]
    public Transform phase1Position;
    public Transform phase2Position;
    public Transform phase3Position;

    private float currentHP;
    private int currentPhase = 1;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        currentHP = maxHP;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = phase1Sprite;
        TeleportTo(phase1Position);
    }

    void Update()
    {
        // Temporarily: press T to deal 50 damage to the boss
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            TakeDamage(50f);
        }
    }

    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        Debug.Log("Boss took " + amount + " damage. HP remaining: " + currentHP);

        if (currentHP <= 0)
        {
            currentHP = 0;
            Debug.Log("Design Complete");
        }

        CheckPhaseTransition();
    }

    private void CheckPhaseTransition()
    {
        float hpPercent = currentHP / maxHP;

        if (currentPhase == 1 && hpPercent <= 0.75f)
        {
            currentPhase = 2;
            spriteRenderer.sprite = phase2Sprite;
            TeleportTo(phase2Position);
            Debug.Log("Phase 2 - some colour added");
        }
        else if (currentPhase == 2 && hpPercent <= 0.50f)
        {
            currentPhase = 3;
            spriteRenderer.sprite = phase3Sprite;
            TeleportTo(phase3Position);
            Debug.Log("Phase 3 - more colour added");
        }
    }

    private void TeleportTo(Transform target)
    {
        if (target == null)
        {
            Debug.LogWarning("Phase position not assigned in Inspector!");
            return;
        }
        transform.position = target.position;
    }
}
