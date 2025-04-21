using TMPro;
using UnityEngine;

public class PlayerNameplateUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI hpText;
    public string playerName = "Hero";

    void Start()
    {
        // プレイヤーを自動的に探す（プレハブから生成されたものを見つける）
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        // 名前をセット
        if (nameText != null)
        {
            nameText.text = playerName;
        }
    }
}
