using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    public static GameObject playerUnit;
    public static GameObject enemyUnit;

    private BattleCharacter playerChar;
    private BattleCharacter enemyChar;

    private static List<GameObject> dungeonSceneRoots = new();

    public static bool IsInBattle { get; private set; } = false;

    public static void StartBattle(GameObject player, GameObject enemy)
    {
        if (IsInBattle) return; // 二重呼び出し防止
        IsInBattle = true;

        playerUnit = player;
        enemyUnit = enemy;

        // ダンジョンシーンのルートオブジェクトを非アクティブ化
        Scene dungeonScene = SceneManager.GetSceneByName("DungeonScene");
        dungeonSceneRoots.Clear();

        foreach (GameObject root in dungeonScene.GetRootGameObjects())
        {
            dungeonSceneRoots.Add(root);
            root.SetActive(false);
        }

        // BattleScene を Additive 読み込み
        SceneManager.LoadScene("BattleScene", LoadSceneMode.Additive);
    }

    private IEnumerator Start()
    {
        yield return null; // シーン遷移直後の安全待ち

        // アクティブシーンをバトルに切り替え
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("BattleScene"));

        SetupCharacters();
        StartCoroutine(RunAutoBattle(playerChar, enemyChar));
    }

    private void SetupCharacters()
    {
        playerChar = BattleManager.playerUnit.GetComponentInChildren<BattleCharacter>();
        enemyChar = BattleManager.enemyUnit.GetComponentInChildren<BattleCharacter>();

        // プレイヤーのHP UIを紐づける
        playerChar.hpSlider = GameObject.Find("PlayerHPBar")?.GetComponent<Slider>();
        playerChar.hpText = GameObject.Find("PlayerHPText")?.GetComponent<TextMeshProUGUI>();

        // 敵のHP UIを紐づける
        enemyChar.hpSlider = GameObject.Find("EnemyHPBar")?.GetComponent<Slider>();
        enemyChar.hpText = GameObject.Find("EnemyHPText")?.GetComponent<TextMeshProUGUI>();

        // HPを初期表示
        playerChar.UpdateHPUI();
        enemyChar.UpdateHPUI();
    }

    private IEnumerator RunAutoBattle(BattleCharacter playerChar, BattleCharacter enemyChar)
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            enemyChar.TakeDamage(1);
            // enemyChar.enemyHealth.ApplyCurrentHP(enemyHp);

            if (enemyChar.currentHP <= 0)
            {
                EndBattle(true);
                yield break;
            }

            yield return new WaitForSeconds(1f);
            playerChar.TakeDamage(1);
            playerChar.playerHealth.ApplyCurrentHP(playerChar.currentHP);

            if (playerChar.currentHP <= 0)
            {
                EndBattle(false);
                yield break;
            }
        }
    }

    private void EndBattle(bool playerWon)
    {
        Debug.Log(playerWon ? "勝利！" : "敗北...");
        IsInBattle = false;

        Destroy(enemyUnit);

        // BattleScene を閉じる
        SceneManager.UnloadSceneAsync("BattleScene");

        // DungeonScene を再アクティブ化
        foreach (GameObject root in dungeonSceneRoots)
        {
            root.SetActive(true);
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByName("DungeonScene"));
    }
}
