// EnemyFollow.cs
using UnityEngine;

/// <summary>
/// プレイヤーを追跡し、接触時にバトルを開始する敵の挙動スクリプト（ダンジョン内）
/// </summary>
public class EnemyFollow : MonoBehaviour
{
    [Header("行動パラメータ")]
    public float moveSpeed = 2f;           // 追跡移動速度
    public float chaseRange = 5f;          // 追跡を開始する距離

    private Transform target;              // 追跡対象（プレイヤー）

    void Start()
    {
        // タグでプレイヤーを探す
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
    }

    void Update()
    {
        if (target == null) return;

        // プレイヤーとの距離を測定
        float distance = Vector2.Distance(transform.position, target.position);

        if (distance <= chaseRange)
        {
            // プレイヤーに向かって移動
            Vector2 direction = (target.position - transform.position).normalized;
            transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// プレイヤーと接触したときにバトルシーンへ移行する
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            BattleManager.StartBattle(collision.gameObject, this.gameObject);
        }
    }
}
