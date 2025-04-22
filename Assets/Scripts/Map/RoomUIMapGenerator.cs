using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// RoomNode構造からUIボタン群を自動生成するクラス
/// </summary>
public class RoomUIMapGenerator : MonoBehaviour
{
    public RectTransform rootCanvas;            // 配置先Canvas（RectTransform）
    public GameObject roomButtonPrefab;         // プレハブボタン（RoomNodeButton）
    public float spacing = 100f;                // UIボタン間の距離

    /// <summary>
    /// ツリー構造に基づいてUIボタンを生成
    /// </summary>
    public void GenerateUIFromTree(RoomNode root)
    {
        foreach (var node in root.Traverse())
        {
            int rootX = node.Position.x;
            int rootY = node.Position.y;

            foreach (RoomNode child in node.Children)
            {
                // 子部屋のY座標を取得
                int childY = child.Position.y;

                // 矢印を生成
                GameObject btn;
                if (childY > rootY && node.Children.Count > 1)
                {
                    btn = Instantiate(roomButtonPrefab, new Vector3(rootX, rootY + 4, 0), Quaternion.identity);
                }
                else if (childY < rootY && node.Children.Count > 1)
                {
                    btn = Instantiate(roomButtonPrefab, new Vector3(rootX, rootY - 4, 0), Quaternion.identity);
                    btn.transform.eulerAngles = new Vector3(0, 0, 180);
                }
                else
                {
                    btn = Instantiate(roomButtonPrefab, new Vector3(rootX + 7, rootY, 0), Quaternion.identity);
                    btn.transform.eulerAngles = new Vector3(0, 0, 270);
                }

                // 矢印に部屋情報をセット
                var ui = btn.GetComponent<RoomNodeUI>();
                ui.Setup(child);
            }
        }
    }
}