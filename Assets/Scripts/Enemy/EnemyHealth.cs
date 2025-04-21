using UnityEngine;

/// <summary>
/// 敵の体力と死亡処理を管理するクラス。
/// 死亡時に GameManager に通知して、クリア判定を行う。
/// </summary>
public class EnemyHealth : MonoBehaviour
{
    public int maxHP = 5;                  // 最大HP
    [SerializeField] public int currentHP;   // 現在のHP
    public int CurrentHP => currentHP;

    // ボスか
    public bool isBoss = false;

    void Start()
    {
        currentHP = maxHP;
    }

    /// <summary>
    /// 現在のHPを反映
    /// </summary>
    public void ApplyCurrentHP(int hp)
    {
        currentHP = hp;
    }

    /// <summary>
    /// HPが0になったときに呼ばれる死亡処理
    /// </summary>
    void Die()
    {
        // 敵撃破をGameManagerに通知
        GameManager.Instance.OnEnemyDefeated(isBoss);

        // 撃破された敵を消す
        Destroy(gameObject);
    }
}
