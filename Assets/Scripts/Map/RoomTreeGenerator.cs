// RoomTreeGenerator.cs
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ツリー状のダンジョン構造を生成するクラス（共通のボス部屋に集約）
/// </summary>
public class RoomTreeGenerator
{
    public int depth = 5;             // ツリーの深さ（スタートからボスまでの階層数）
    public int branchCount = 2;       // 各階層での分岐数
    private int centerY = 5; // 生成される全体のYの中央を決める（十分大きめに）

    public RoomNode Root { get; private set; }  // スタート部屋

    /// <summary>
    /// ダンジョン構造を生成するメイン関数
    /// </summary>
    public void GenerateTree()
    {
        Root = new RoomNode(RoomNodeType.Start, 0, centerY);

        // ステップ1：スタートから各ルートを再帰生成（ボスはまだ作らない）
        var child1 = new RoomNode(GetRandomRoomType(), 1, centerY - 1);
        var child2 = new RoomNode(GetRandomRoomType(), 1, centerY);
        var child3 = new RoomNode(GetRandomRoomType(), 1, centerY + 1);

        Root.AddChild(child1);
        Root.AddChild(child2);
        Root.AddChild(child3);

        GenerateChildren(child1, 2);
        GenerateChildren(child2, 2);
        GenerateChildren(child3, 2);

        // ステップ2：末端ノードを取得
        List<RoomNode> leafNodes = new List<RoomNode>();
        CollectLeafNodes(Root, leafNodes);

        // ステップ3：共通のボス部屋を作成
        int bossX = depth + 1;
        int bossY = centerY;
        var boss = new RoomNode(RoomNodeType.Boss, bossX, bossY);

        // ステップ4：各末端ノードの子としてボスを追加
        foreach (var leaf in leafNodes)
        {
            leaf.AddChild(boss);
        }
    }

    /// <summary>
    /// 再帰的に通常部屋を生成（最終階層まで）
    /// </summary>
    private void GenerateChildren(RoomNode parent, int currentDepth)
    {
        if (currentDepth > depth) return;

        var child = new RoomNode(GetRandomRoomType(), parent.X + 1, parent.Y);
        parent.AddChild(child);
        GenerateChildren(child, currentDepth + 1);
    }

    /// <summary>
    /// 木構造の末端ノード（子を持たないノード）を再帰的に取得
    /// </summary>
    private void CollectLeafNodes(RoomNode node, List<RoomNode> leafNodes)
    {
        if (node.Children.Count == 0)
        {
            leafNodes.Add(node);
            return;
        }

        foreach (var child in node.Children)
        {
            CollectLeafNodes(child, leafNodes);
        }
    }

    /// <summary>
    /// 通常、バトル、宝箱のいずれかの部屋タイプをランダムに返す
    /// </summary>
    private RoomNodeType GetRandomRoomType()
    {
        int rand = Random.Range(0, 3);
        return rand switch
        {
            0 => RoomNodeType.Normal,
            1 => RoomNodeType.Battle,
            2 => RoomNodeType.Treasure,
            _ => RoomNodeType.Normal
        };
    }
}
