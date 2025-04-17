using TMPro;
using UnityEngine;

public class PlayerNameplateUI : MonoBehaviour
{
    public PlayerHealth player;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI hpText;
    public string playerName = "Hero";

    void Start()
    {
        // プレイヤーを自動的に探す（プレハブから生成されたものを見つける）
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.GetComponent<PlayerHealth>();
            }
        }

        // 名前をセット
        if (nameText != null)
        {
            nameText.text = playerName;
        }
    }

    // void Update()
    // {
    //     if (player != null && hpText != null)
    //     {
    //         hpText.text = GetHeartString(player.currentHP);
    //     }
    // }

    // // ♥ をHPの数だけ繰り返す
    // string GetHeartString(int hp)
    // {
    //     int max = player.maxHP;
    //     int current = Mathf.Clamp(hp, 0, max);

    //     string full = $"<color=#ff0000>{new string('♥', current)}</color>";
    //     string empty = $"<color=#999999>{new string('♡', max - current)}</color>";
    //     return full + empty;
    // }
}
