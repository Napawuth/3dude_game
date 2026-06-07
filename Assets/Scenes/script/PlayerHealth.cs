using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public float maxHP = 100f;
    private float currentHP;
    private bool isShielded = false;

    [SerializeField] private PlayerStatusBar statusBar;

    [SerializeField] private GameManager gameManager;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        currentHP = maxHP;
        spriteRenderer = GetComponent<SpriteRenderer>();
        statusBar.UpdateHealth(currentHP, maxHP);
    }

    public void TakeDamage(float amount)
    {
        if (isShielded)
        {
            isShielded = false;
            Debug.Log("Shield Blocked the hit!");
            return;
        }

        currentHP -= amount;
        currentHP = Mathf.Max(currentHP, 0f);

        statusBar.UpdateHealth(currentHP, maxHP);
        StartCoroutine(FlashRed());
        Debug.Log("Player took " + amount + " damage. HP remaining: " + currentHP);

        if (currentHP <= 0)
        {
            Debug.Log("Player Defeated");
            gameManager.ShowGameOver();
        }
    }

    public void ActivateShield()
    {
        isShielded = true;
        StartCoroutine(FlashBlue());
        Debug.Log("Shield Activated");
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        spriteRenderer.color = Color.white;
    }

    private IEnumerator FlashBlue()
    {
        spriteRenderer.color = Color.blue;
        yield return new WaitForSeconds(0.3f);
        spriteRenderer.color = Color.white;
    }
}