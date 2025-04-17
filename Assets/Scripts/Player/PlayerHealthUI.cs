using TMPro;
using UnityEngine;

public class PlayerHealthUI : MonoBehaviour
{
    public PlayerHealth player;                 // プレイヤーのスクリプト（自動設定される）
    public TextMeshProUGUI hpText;             // 表示用Text（♥♥♥♡♡など）

    void Start()
    {
        // プレイヤーを自動で探してセット
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.GetComponent<PlayerHealth>();
            }
        }
    }

    void Update()
    {
        if (player != null && hpText != null)
        {
            hpText.text = GetHeartString(player.currentHP);
        }
    }

    // ♥と♡でHPを表現（例：♥♥♥♡♡）
    string GetHeartString(int hp)
    {
        int max = player.maxHP;
        int current = Mathf.Clamp(hp, 0, max);

        string full = $"<color=#ff0000>{new string('♥', current)}</color>";
        string empty = $"<color=#999999>{new string('♡', max - current)}</color>";
        return full + empty;
    }
}
