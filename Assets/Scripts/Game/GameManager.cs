using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

/// <summary>
/// ゲームの進行状態（ゲームクリア／ゲームオーバー）を一元管理するクラス
/// プレイヤーや敵、マップ生成後のフローをコントロールします
/// </summary>
public class GameManager : MonoBehaviour
{
    // ✅ Singleton によるインスタンスアクセス
    public static GameManager Instance { get; private set; }

    [Header("ゲームクリア UI")]
    [SerializeField] private GameObject clearPanel;               // クリア時に表示するパネル

    [Header("ゲームオーバー UI")]
    [SerializeField] private GameObject gameOverPanel;            // ゲームオーバー時に表示するパネル

    [Header("停止対象のプレイヤー")]
    public GameObject player;                   // 操作を止めるプレイヤーオブジェクト

    [Header("ミニマップ")]
    [SerializeField] private MiniMapController miniMapController;                 // ミニマップ生成クラス

    private int enemyCount = 0;                 // 敵の残数
    private int normalEnemyCount = 0;           // 通常敵の残数
    private bool bossDefeated = false;          // ボス撃破フラグ
    private bool isGameOver = false;            // ゲームオーバーフラグ




    void Awake()
    {
        // Singletonの初期化
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // UI初期化（非表示）
        if (clearPanel != null) clearPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    void Start()
    {
        // マップが生成されるまで待機し、敵数を数える
        StartCoroutine(WaitForMap());
    }

    /// <summary>
    /// マップ生成完了を待ってからの処理
    /// </summary>
    IEnumerator WaitForMap()
    {
        // マップ生成が完了するまで待機
        yield return new WaitUntil(() =>
            MapGeneratorTree.Instance != null && MapGeneratorTree.Instance.IsGenerated
        );

        // プレイヤーを生成して取得
        player = MapGeneratorTree.Instance.Player;
        if (player != null)
        {
            var health = player.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.OnPlayerDeath.AddListener(OnPlayerDeath);
            }
        }
        else
        {
            Debug.Log("プレイヤーが生成できませんでした");

        }

        // // ボスを生成して取得
        // GameObject boss = MapGenerator.Instance.SpawnBoss();
        // if (boss == null)
        // {
        //     Debug.Log("ボスが生成できませんでした");
        // }

        // // 敵を生成して数を取得
        // normalEnemyCount = MapGenerator.Instance.SpawnEnemies();
        // if (normalEnemyCount == 0)
        // {
        //     Debug.Log("敵が生成できませんでした");
        // }

        // // ミニマップを初期化
        // if (miniMapController != null && player != null)
        // {
        //     miniMapController.Initialize(MapGenerator.Instance.map, player);
        // }
        // else
        // {
        //     Debug.Log("ミニマップが生成できませんでした");
        // }
    }

    /// <summary>
    /// プレイヤーが倒されたときに呼ばれる（EnemyHealthから）
    /// </summary>
    private void OnPlayerDeath()
    {
        TriggerGameOver(); // ← UI制御や再挑戦ボタンの表示もここで
    }

    /// <summary>
    /// 敵が1体倒されたときに呼ばれる（EnemyHealthから）
    /// </summary>
    public void OnEnemyDefeated(bool isBoss)
    {
        // ボスと通常的で処理を分岐
        if (isBoss)
        {
            // ボス撃破フラグを立てて、クリア画面を表示
            bossDefeated = true;
            TriggerGameClear();
        }
        else
        {
            // 通常敵の数をカウントする
            normalEnemyCount--;
            Debug.Log($"敵残り: {normalEnemyCount}");

            // 条件付きイベント：全雑魚敵撃破時に何かする、など
        }
    }

    /// <summary>
    /// ボスが倒されたときに呼ばれる（EnemyHealthから）
    /// </summary>
    public void TriggerGameClear()
    {
        if (isGameOver) return;
        isGameOver = true;

        Debug.Log("🎉 ボス撃破！ゲームクリア！");

        // UI表示
        if (clearPanel != null) clearPanel.SetActive(true);

        // ゲーム停止
        GameStop();
    }

    /// <summary>
    /// プレイヤーが死亡したときに呼ばれる（PlayerHealthから）
    /// </summary>
    public void TriggerGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        Debug.Log("☠️ ゲームオーバー");

        // UI表示
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        // ゲーム停止
        GameStop();
    }

    /// <summary>
    /// ゲームを停止させる
    /// </summary>
    private void GameStop()
    {
        // プレイヤーの操作をすべて止める（MonoBehaviourを無効化）
        if (player != null)
        {
            foreach (var comp in player.GetComponents<MonoBehaviour>())
            {
                comp.enabled = false;
            }
        }

        // 時間停止（必要に応じて）
        Time.timeScale = 0f;
    }

    /// <summary>
    /// ゲームをリトライする
    /// </summary>
    public void Retry()
    {
        Debug.Log("🔁 リトライを実行中...");
        isGameOver = false;
        Time.timeScale = 1f; // 一時停止を解除（必要な場合）
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // 現在のシーンを再読み込み
    }
}
