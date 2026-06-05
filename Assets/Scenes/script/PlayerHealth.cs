using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public float maxHP = 100f;
    private float currentHP;

    [SerializeField] private PlayerStatusBar statusBar;
    
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        currentHP = maxHP;
        spriteRenderer = GetComponent<SpriteRenderer>();
        statusBar.UpdateHealth(currentHP, maxHP);
    }

    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        currentHP = Mathf.Max(currentHP, 0f);

        statusBar.UpdateHealth(currentHP, maxHP);
        StartCoroutine(FlashRed());
        Debug.Log("Player took " + amount + " damage. HP remaining: " + currentHP);

        if (currentHP <= 0)
            Debug.Log("Player Defeated");
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        spriteRenderer.color = Color.white;
    }
}