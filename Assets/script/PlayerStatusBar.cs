using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerStatusBar : MonoBehaviour
{
    [Header("Health Bar")]
    [SerializeField] private RectTransform healthFill;
    private float maxHealthWidth = 325f;

    [Header("Attack Cooldown Bar")]
    [SerializeField] private RectTransform attackCooldownFill;
    private float maxAttackWidth = 145f;

    [Header("Super Icon")]
    [SerializeField] private Image superIcon;
    [SerializeField] private Sprite superReady;
    [SerializeField] private Sprite superCooldown;

    [Header("Utility Icon")]
    [SerializeField] private Image utilityIcon;
    [SerializeField] private Sprite utilityReady;
    [SerializeField] private Sprite utilityCooldown;

    // Called by BossHealth script when boss takes damage
    public void UpdateHealth(float currentHP, float maxHP)
    {
        float fillAmount = currentHP / maxHP;
        healthFill.sizeDelta = new Vector2(maxHealthWidth * fillAmount, healthFill.sizeDelta.y);
    }

    // Called when attack used, fills back up over cooldown duration
    public void UpdateAttackCooldown(float cooldownRemaing, float cooldownTotal)
    {
        float fillAmount = 1f - (cooldownRemaing / cooldownTotal);
        attackCooldownFill.sizeDelta = new Vector2(maxAttackWidth * fillAmount, attackCooldownFill.sizeDelta.y);
    }

    public void SetSuperReady(bool isReady)
    {
        superIcon.sprite = isReady ? superReady : superCooldown;
    }

    public void SetUtilityReady(bool isReady)
    {
        utilityIcon.sprite = isReady ? utilityReady : utilityCooldown;
    }

    public void TriggerAttackCooldown(float cooldownTime)
    {
        StartCoroutine(attackCooldownRoutine(cooldownTime));
    }

    private IEnumerator attackCooldownRoutine(float cooldownTime)
    {
        attackCooldownFill.localScale = new Vector3(0f, 1f, 1f);

        float elapsed = 0f;
        while (elapsed < cooldownTime)
        {
            elapsed += Time.deltaTime;
            float fillAmount = elapsed / cooldownTime;
            attackCooldownFill.localScale = new Vector3(fillAmount, 1f, 1f);
            yield return null;
        }

        attackCooldownFill.localScale = new Vector3(1f, 1f, 1f);
    }
}
