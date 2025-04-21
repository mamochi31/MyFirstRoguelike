using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleCharacter : MonoBehaviour
{
    [Header("ステータス")]
    public int maxHP = 10;
    public int currentHP = 10;

    [Header("UI要素")]
    public PlayerHealth playerHealth;
    public EnemyHealth enemyHealth;
    public Slider hpSlider;
    public TextMeshProUGUI hpText;

    void Awake()
    {
        if (playerHealth == null)
        {
            playerHealth = GetComponent<PlayerHealth>();
        }
        if (enemyHealth == null)
        {
            enemyHealth = GetComponent<EnemyHealth>();
        }
    }

    void Start()
    {
        if (playerHealth != null)
        {
            maxHP = playerHealth.maxHP;
            currentHP = playerHealth.currentHP;
        }
        if (enemyHealth != null)
        {
            maxHP = enemyHealth.maxHP;
            currentHP = enemyHealth.currentHP;
        }
        UpdateHPUI();
    }

    /// <summary>
    /// ダメージを受ける
    /// </summary>
    public void TakeDamage(int damage)
    {
        currentHP = Mathf.Max(currentHP - damage, 0);
        if (playerHealth != null) playerHealth.ApplyCurrentHP(currentHP);
        if (enemyHealth != null) enemyHealth.ApplyCurrentHP(currentHP);
        UpdateHPUI();
    }

    /// <summary>
    /// 回復する
    /// </summary>
    public void Heal(int amount)
    {
        currentHP = Mathf.Min(currentHP + amount, maxHP);
        UpdateHPUI();
    }

    /// <summary>
    /// HPバーとテキストを更新
    /// </summary>
    public void UpdateHPUI()
    {
        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHP;
            hpSlider.value = currentHP;
        }

        if (hpText != null)
        {
            hpText.text = $"{currentHP} / {maxHP}";
        }

    }
}
