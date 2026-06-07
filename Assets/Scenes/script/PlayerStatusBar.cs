using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class PlayerStatusBar : MonoBehaviour
{
    [Header("Health Bar")]
    [SerializeField] private RectTransform healthFill;
    private float maxHealthWidth = 395f;

    [Header("Attack Cooldown Bar")]
    [SerializeField] private RectTransform attackCooldownFill;
    private float maxAttackWidth = 170f;

    [Header("Super Icon")]
    [SerializeField] private Image superIcon;
    [SerializeField] private Sprite superReady;
    [SerializeField] private Sprite superCooldown;
    [SerializeField] private TextMeshProUGUI superCooldownText;

    [Header("Utility Icon")]
    [SerializeField] private Image utilityIcon;
    [SerializeField] private Sprite utilityReady;
    [SerializeField] private Sprite utilityCooldown;
    [SerializeField] private TextMeshProUGUI utilityCooldownText;

    void Start()
    {
        attackCooldownFill.localScale = new Vector3(1f, 1f, 1f);
    }

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
        attackCooldownFill.localScale = new Vector3(fillAmount, 1f, 1f);
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

    public void SetSuperReady(bool isReady)
    {
        superIcon.sprite = isReady ? superReady : superCooldown;
        if (isReady)
            superCooldownText.text = "";
    }

    public void SetUtilityReady(bool isReady)
    {
        utilityIcon.sprite = isReady ? utilityReady : utilityCooldown;
        if (isReady)
            utilityCooldownText.text = "";
    }

    public void TriggerSuper(float cooldownTime)
    {
        StartCoroutine(SuperCooldownRoutine(cooldownTime));
    }

    public void TriggerUtility(float cooldownTime)
    {
        StartCoroutine(UtilityCooldownRoutine(cooldownTime));
    }

    private IEnumerator SuperCooldownRoutine(float cooldownTime)
    {
        SetSuperReady(false);
        float remaining = cooldownTime;

        while (remaining > 0)
        {
            remaining -= Time.deltaTime;
            superCooldownText.text = Mathf.CeilToInt(remaining).ToString(); // CeilToInt used to keep numbers whole
            yield return null;
        }

        superCooldownText.text = "";
        SetSuperReady(true);
    }

    private IEnumerator UtilityCooldownRoutine(float cooldownTime)
    {
        Debug.Log("Utility cooldown started");
        SetUtilityReady(false);
        float remaining = cooldownTime;

        while (remaining > 0)
        {
            remaining -= Time.deltaTime;
            utilityCooldownText.text = Mathf.CeilToInt(remaining).ToString();
            yield return null;
        }

        Debug.Log("Utility cooldown finished");
        utilityCooldownText.text = "";
        SetUtilityReady(true);
    }
}
