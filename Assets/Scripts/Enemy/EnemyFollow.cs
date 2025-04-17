using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyFollow : MonoBehaviour
{
    [Header("追跡対象")]
    public Transform target;               // プレイヤーのTransform（自動設定される）

    [Header("移動設定")]
    public float moveSpeed = 2f;           // 敵の移動速度
    public float chaseRange = 5f;          // プレイヤーを見つける距離（これより遠いと無視）

    [Header("攻撃設定")]
    public float attackRange = 1f;         // 攻撃できる距離
    public float attackCooldown = 1.5f;    // 攻撃間隔（秒）
    private float lastAttackTime = 0f;     // 最後に攻撃した時間

    private PlayerHealth playerHealth;     // PlayerHealthを初期化


    void Start()
    {
        // プレイヤーを自動で探して設定
        if (target == null)
        {
            GameObject playerObj = GameManager.Instance.player;
            if (playerObj != null)
            {
                target = playerObj.transform;
                playerHealth = playerObj.GetComponent<PlayerHealth>();
            }
            else
            {
                Debug.LogWarning("EnemyFollow: プレイヤーが見つかりません（Tag: Player）");
            }
        }
    }

    void Update()
    {
        if (target == null) return;

        // プレイヤーとの距離を計算
        float distanceSqr = (target.position - transform.position).sqrMagnitude;
        float chaseRangeSqr = chaseRange * chaseRange;
        float attackRangeSqr = attackRange * attackRange;

        // プレイヤーが追跡範囲内にいるとき
        if (distanceSqr <= chaseRangeSqr)
        {
            if (distanceSqr > attackRangeSqr)
            {
                // ▶️ 移動処理（プレイヤー方向へ移動）
                Vector2 direction = (target.position - transform.position).normalized;
                transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
            }
            else
            {
                // 💥 攻撃処理（一定間隔で）
                if (Time.time - lastAttackTime >= attackCooldown)
                {
                    AttackPlayer();
                    lastAttackTime = Time.time;
                }
            }
        }
    }

    // 攻撃処理（プレイヤーのHPを減らす）
    void AttackPlayer()
    {
        Debug.Log("敵がプレイヤーに攻撃！");
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(1); // 1ダメージ
        }
    }

    // エンカウント判定
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            BattleManager.StartBattle(collision.gameObject, this.gameObject);
        }
    }


}
