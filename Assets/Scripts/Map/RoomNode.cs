// RoomNode.cs
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 部屋の種類を表す列挙型（RoomNodeType）
/// </summary>
public enum RoomNodeType
{
    Start,      // 開始部屋
    Normal,     // 通常部屋
    Battle,     // 戦闘部屋
    Treasure,   // 宝箱部屋
    Heal,       // 回復部屋
    Boss        // ボス部屋
}

/// <summary>
/// ツリー構造で使用する部屋ノードのクラス
/// </summary>
public class RoomNode
{
    public RoomNodeType Type;                // 部屋の種類
    public int X, Y;                         // ツリー上の位置インデックス（UIや描画用）
    public RoomNode Parent;                  // 親ノード（null = ルート）
    public List<RoomNode> Children;          // 子ノードリスト
    public Vector2Int Position;              // タイルマップ上の中心座標

    /// <summary>
    /// ノードを初期化する
    /// </summary>
    public RoomNode(RoomNodeType type, int x, int y)
    {
        Type = type;
        X = x;
        Y = y;
        Children = new List<RoomNode>();
    }

    /// <summary>
    /// 子ノードを追加し、親子関係を構築
    /// </summary>
    public void AddChild(RoomNode child)
    {
        child.Parent = this;
        Children.Add(child);
    }

    /// <summary>
    /// ノードの簡易表現（デバッグ用）
    /// </summary>
    public override string ToString()
    {
        return $"{Type} ({X}, {Y})";
    }

    /// <summary>
    /// 自身を含むツリー構造全体を走査するイテレータ
    /// </summary>
    public IEnumerable<RoomNode> Traverse()
    {
        yield return this;
        foreach (var child in Children)
        {
            foreach (var desc in child.Traverse())
                yield return desc;
        }
    }
}
