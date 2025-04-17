using UnityEngine;
using UnityEngine.Events;
using System;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 5;
    public int currentHP;

    public UnityEvent OnPlayerDeath = new();

    // HPが変更されたときのイベント（UIと連携できる）
    public event Action<int, int> OnHPChanged;

    void Start()
    {
        currentHP = maxHP;
        OnHPChanged?.Invoke(currentHP, maxHP);
    }

    // public void TakeDamage(int damage)
    // {
    //     currentHP -= damage;
    //     currentHP = Mathf.Max(0, currentHP);
    //     OnHPChanged?.Invoke(currentHP, maxHP); // ← UIに通知

    //     if (currentHP == 0)
    //     {
    //         // GameManager にプレイヤー撃破を通知
    //         if (GameManager.Instance != null)
    //         {
    //             GameManager.Instance.TriggerGameOver();
    //         }
    //     }
    // }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            OnPlayerDeath.Invoke(); // 🔥 GameManager などへ通知
        }
    }

    public void SetHP(int newHP)
    {
        currentHP = Mathf.Clamp(newHP, 0, maxHP);
        OnHPChanged?.Invoke(currentHP, maxHP);
    }

}
