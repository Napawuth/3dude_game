using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyHealth : MonoBehaviour
{
    public float maxHP = 500f;

    [Header("Phase Positions")]
    public Transform phase1Position;
    public Transform phase2Position;
    public Transform phase3Position;

    [Header("Phase Animator Controllers")]
    [SerializeField] private RuntimeAnimatorController controller100;
    [SerializeField] private RuntimeAnimatorController controller75;
    [SerializeField] private RuntimeAnimatorController controller50;

    [Header("UI")]
    [SerializeField] private BossStatusBar bossStatusBar;
    private Animator anim;

    private float currentHP;
    private int currentPhase = 1;

    void Start()
    {
        currentHP = maxHP;
        TeleportTo(phase1Position);
        bossStatusBar.UpdateHealth(currentHP, maxHP);

        anim = GetComponent<Animator>();
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
        currentHP = Mathf.Max(currentHP, 0f);
        bossStatusBar.UpdateHealth(currentHP, maxHP);
        Debug.Log("Boss took " + amount + " damage. HP remaining: " + currentHP);

        if (currentHP <= 0)
            Debug.Log("Design Complete");

        CheckPhaseTransition();
    }

    private void CheckPhaseTransition()
    {
        Debug.Log("HP Percent: " + (currentHP / maxHP));
        float hpPercent = currentHP / maxHP;

        if (currentPhase == 1 && hpPercent <= 0.75f)
        {
            currentPhase = 2;
            anim.runtimeAnimatorController = controller75;
            TeleportTo(phase2Position);
            Debug.Log("Phase 2 - some colour added");
        }
        else if (currentPhase == 2 && hpPercent <= 0.50f)
        {
            currentPhase = 3;
            anim.runtimeAnimatorController = controller50;
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
