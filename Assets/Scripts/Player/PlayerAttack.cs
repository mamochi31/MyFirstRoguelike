using UnityEngine;
using UnityEngine.Audio; // AudioMixer に対応するために必要

public class PlayerAttack : MonoBehaviour
{
    [Header("攻撃設定")]
    public float attackRange = 1.5f;        // 攻撃が届く距離
    public int attackDamage = 1;            // 攻撃ダメージ
    public float attackCooldown = 0.5f;     // 攻撃のクールタイム（連打防止）
    public LayerMask enemyLayer;           // 攻撃対象のレイヤー（Enemyなど）

    [Header("演出設定")]
    public GameObject hitEffectPrefab;     // ヒット時のエフェクト
    public AudioClip hitSound;             // ヒット時の効果音
    public AudioMixerGroup sfxMixerGroup;  // 効果音を流すミキサーグループ（任意）

    [Range(0f, 1f)]
    public float sfxVolume = 0.4f;         // 効果音の音量

    private AudioSource audioSource;       // 効果音再生用
    private float lastAttackTime = -999f;  // 最後に攻撃した時刻（初期値は十分過去）

    void Start()
    {
        // AudioSource がなければ追加
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // AudioMixerGroup を設定（あれば）
        if (sfxMixerGroup != null)
        {
            audioSource.outputAudioMixerGroup = sfxMixerGroup;
        }
    }

    void Update()
    {
        // スペースキーが押され、かつクールタイムが過ぎていれば攻撃
        if (Input.GetKeyDown(KeyCode.Space) && Time.time - lastAttackTime >= attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    void Attack()
    {
        // 範囲内の敵をすべて取得
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);
        bool hitSomething = false;

        foreach (Collider2D hit in hits)
        {
            // 敵に EnemyHealth コンポーネントがついているか確認
            EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                // ダメージを与える
                enemy.TakeDamage(attackDamage);

                // 最初の命中にだけエフェクトを再生
                if (!hitSomething && hitEffectPrefab != null)
                {
                    Instantiate(hitEffectPrefab, hit.transform.position, Quaternion.identity);
                }

                hitSomething = true;
            }
        }

        // 音も1回だけ鳴らす
        if (hitSomething && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound, sfxVolume);
        }
    }

    // 攻撃範囲を Scene 上で可視化
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
