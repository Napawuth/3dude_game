using UnityEngine;
using UnityEngine.UI;


public class BossStatusBar : MonoBehaviour
{
    [SerializeField] private RectTransform bossHealthFill;
    private float maxHealthWidth = 455f;

    public void UpdateHealth(float currentHP, float maxHP)
    {
        float fillAmount = currentHP / maxHP;
        bossHealthFill.sizeDelta = new Vector2(maxHealthWidth * fillAmount, bossHealthFill.sizeDelta.y);
    }
}