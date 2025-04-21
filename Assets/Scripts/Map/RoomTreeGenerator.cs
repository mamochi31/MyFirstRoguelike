// RoomTreeGenerator.cs
using UnityEngine;

/// <summary>
/// ツリー状のダンジョン構造を生成するクラス
/// </summary>
public class RoomTreeGenerator
{
    public int depth = 5; // ツリーの深さ（スタートからボスまでの階層数）
    public int branchCount = 2; // 各階層での分岐数
    private int centerY = 5; // 生成される全体のYの中央を決める（十分大きめに）

    public RoomNode Root { get; private set; } // ルートノード（スタート部屋）

    /// <summary>
    /// ツリー全体を生成するエントリーポイント
    /// </summary>
    public void GenerateTree()
    {
        Root = new RoomNode(RoomNodeType.Start, 0, centerY);

        // スタート部屋から2本の枝を生やす（Y方向に分ける）
        var child1 = new RoomNode(GetRandomRoomType(), 1, centerY - 1);
        var child2 = new RoomNode(GetRandomRoomType(), 1, centerY + 1);

        Root.AddChild(child1);
        Root.AddChild(child2);

        GenerateChildren(child1, 2);
        GenerateChildren(child2, 2);
    }


    /// <summary>
    /// 指定ノードの子ノードを再帰的に生成する
    /// </summary>
    private void GenerateChildren(RoomNode parent, int currentDepth)
    {
        if (currentDepth >= depth)
        {
            var boss = new RoomNode(RoomNodeType.Boss, parent.X + 1, parent.Y);
            parent.AddChild(boss);
            return;
        }

        var child = new RoomNode(GetRandomRoomType(), parent.X + 1, parent.Y);
        parent.AddChild(child);
        GenerateChildren(child, currentDepth + 1);
    }


    /// <summary>
    /// ランダムで部屋の種類を決定する
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
