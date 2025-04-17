using UnityEngine;

/// <summary>
/// 部屋の種類を保持
/// </summary>
public enum RoomType
{
    Normal,     // 通常の部屋
    Start,      // プレイヤーの出現場所
    Boss,       // ボス専用部屋
    Item,       // アイテムが出現
    Rest        // 回復部屋
}

/// <summary>
/// ダンジョン内の部屋を表すクラス
/// 部屋の位置とサイズを保持し、中心座標や重なりチェックも可能
/// </summary>
public class Room
{
    public RectInt rect;

    /// <summary>
    /// 指定位置・サイズの部屋を生成
    /// </summary>
    public Room(int x, int y, int width, int height)
    {
        rect = new RectInt(x, y, width, height);
    }

    /// <summary>
    /// 部屋の種類のフィールド
    /// 初期値はNormal
    /// </summary>
    public RoomType Type { get; set; } = RoomType.Normal;

    /// <summary>
    /// 部屋の中心座標（通路接続や配置に使用）
    /// </summary>
    public Vector2Int GetCenter()
    {
        return new Vector2Int(rect.x + rect.width / 2, rect.y + rect.height / 2);
    }

    /// <summary>
    /// 部屋の中心座標（Tilemap 用）
    /// </summary>
    public Vector3Int GetCenter3()
    {
        Vector2Int center = GetCenter();
        return new Vector3Int(center.x, center.y, 0);
    }


    /// <summary>
    /// この部屋の壁に近すぎない座標を返す（スポーン位置に使用）
    /// </summary>
    public Vector2Int GetRandomPosition()
    {
        int x = Random.Range(rect.x + 1, rect.xMax - 1);
        int y = Random.Range(rect.y + 1, rect.yMax - 1);
        return new Vector2Int(x, y);
    }

    /// <summary>
    /// 他の部屋と重なっているかを判定
    /// </summary>
    public bool Overlaps(Room other)
    {
        RectInt expanded = new RectInt(rect.x - 2, rect.y - 2, rect.width + 4, rect.height + 4);
        return expanded.Overlaps(other.rect);
    }

    /// <summary>
    /// 指定した部屋の方向に最も近い「エッジ（縁）」の座標を返す
    /// 通路の接続点として使える
    /// </summary>
    public Vector2Int GetEdgePositionToward(Room other)
    {
        Vector2Int center = GetCenter();
        Vector2Int otherCenter = other.GetCenter();

        int x = Mathf.Clamp(otherCenter.x, rect.x + 1, rect.xMax - 2);
        int y = Mathf.Clamp(otherCenter.y, rect.y + 1, rect.yMax - 2);

        return new Vector2Int(x, y);
    }

    /// <summary>
    /// 指定座標がこの部屋内に含まれているかどうかを判定（Vector2Int）
    /// </summary>
    public bool Contains(Vector2Int pos)
    {
        return rect.Contains(pos);
    }

    /// <summary>
    /// 指定座標がこの部屋内に含まれているかどうかを判定（Vector3Int）
    /// </summary>
    public bool Contains(Vector3Int pos)
    {
        return rect.Contains(new Vector2Int(pos.x, pos.y));
    }
}
