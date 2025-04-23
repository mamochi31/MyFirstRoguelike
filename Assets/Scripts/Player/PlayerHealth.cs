using UnityEngine;
using UnityEngine.Events;
using System;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 100;
    public int currentHP;

    public UnityEvent OnPlayerDeath = new();

    // HPが変更されたときのイベント（UIと連携できる）
    public event Action<int, int> OnHPChanged;

    void Start()
    {
        currentHP = maxHP;
        OnHPChanged?.Invoke(currentHP, maxHP);
    }

    public void ApplyCurrentHP(int hp)
    {
        currentHP = hp;
    }

    public void SetHP(int newHP)
    {
        currentHP = Mathf.Clamp(newHP, 0, maxHP);
        OnHPChanged?.Invoke(currentHP, maxHP);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            BattleManager.StartBattle(this.gameObject, collision.gameObject);
        }
    }
}
