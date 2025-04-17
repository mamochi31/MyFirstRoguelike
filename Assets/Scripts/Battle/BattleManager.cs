using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    private GameObject player;
    private GameObject enemy;

    public float attackInterval = 1.5f;  // 攻撃の間隔
    private float timer = 0f;

    public static BattleManager Instance { get; private set; }

    public static GameObject PlayerInBattle;
    public static GameObject EnemyInBattle;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // プレイヤーと敵を生成（位置は固定）
        player = Instantiate(playerPrefab, new Vector3(-2, 0, 0), Quaternion.identity);
        enemy = Instantiate(enemyPrefab, new Vector3(2, 0, 0), Quaternion.identity);
    }

    void Update()
    {
        if (player == null || enemy == null) return;

        timer += Time.deltaTime;
        if (timer >= attackInterval)
        {
            timer = 0f;
            PerformAutoBattle();
        }
    }

    void PerformAutoBattle()
    {
        // プレイヤーが先に攻撃
        EnemyHealth enemyHP = enemy.GetComponent<EnemyHealth>();
        if (enemyHP != null)
        {
            enemyHP.TakeDamage(1);
            if (enemyHP.CurrentHP <= 0)
            {
                EndBattle(true);
                return;
            }
        }

        // 次に敵が攻撃
        PlayerHealth playerHP = player.GetComponent<PlayerHealth>();
        if (playerHP != null)
        {
            playerHP.TakeDamage(1);
            if (playerHP.currentHP <= 0)
            {
                EndBattle(false);
            }
        }
    }

    public static void StartBattle(GameObject player, GameObject enemy)
    {
        PlayerInBattle = player;
        EnemyInBattle = enemy;

        // シーン間で削除されないように保持
        Object.DontDestroyOnLoad(player);
        Object.DontDestroyOnLoad(enemy);

        // シーンをバトル用に切り替える
        SceneManager.LoadScene("BattleScene");
    }

    void EndBattle(bool playerWon)
    {
        Debug.Log(playerWon ? "勝利！" : "敗北...");
        // あとで UI を表示したり、シーン遷移も追加する予定
        SceneManager.LoadScene("MainScene"); // 戻る例
    }

}
