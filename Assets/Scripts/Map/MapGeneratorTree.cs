// MapGeneratorTree.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// ツリー構造に基づいたマップ生成と描画、キャラクター配置を行うクラス
/// </summary>
public class MapGeneratorTree : MonoBehaviour
{
    public static MapGeneratorTree Instance { get; private set; }
    // 生成したプレイヤー
    public GameObject Player { get; private set; }
    // マップ生成完了フラグ
    public bool IsGenerated { get; private set; } = false;

    [Header("マップサイズ・タイル")]
    public int mapWidth = 100;
    public int mapHeight = 100;
    public Tilemap floorTilemap;
    public TileBase floorTile;

    [Header("プレハブ")]
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public GameObject bossPrefab;

    private MapData map;
    private Dictionary<MapData.TileType, TileBase> tileMap;

    private void Awake()
    {
        Instance = this;

        tileMap = new Dictionary<MapData.TileType, TileBase>
        {
            { MapData.TileType.Floor, floorTile },
            { MapData.TileType.Corridor, floorTile },
            // { MapData.TileType.Wall, wallTile }
            // ← 今後ここに Door, Trap, Exit などを追加するだけでOK！
        };
    }

    void Start()
    {
        // 自動でマップ生成
        GenerateDungeonFromTree();

        // マップ生成完了
        IsGenerated = true;
    }

    /// <summary>
    /// ツリー構造に基づいてダンジョンを生成する
    /// </summary>
    public void GenerateDungeonFromTree()
    {
        // ツリー構造のルートノードを生成
        RoomTreeGenerator generator = new RoomTreeGenerator();
        generator.GenerateTree();

        // マップデータ初期化
        map = new MapData(mapWidth, mapHeight);
        map.FillWithWalls();

        // 部屋と通路の描画
        DrawRoomTree(generator.Root);

        // タイルマップを描画
        DrawTilemapFromMapData();

        // キャラクター配置
        SpawnCharacters(generator.Root);
    }

    /// <summary>
    /// 部屋と通路をツリー構造に基づいて再帰的に描画する
    /// </summary>
    private void DrawRoomTree(RoomNode root)
    {
        if (root == null) return;

        DrawRoom(root);

        foreach (var child in root.Children)
        {
            DrawRoomTree(child);
            CreateCorridor(root, child);
        }
    }

    /// <summary>
    /// 単一の部屋を描画し、ノードに中心座標を記録する
    /// </summary>
    private void DrawRoom(RoomNode node)
    {
        int size = 5; // 各部屋のサイズ（正方形）
        Vector2Int origin = new Vector2Int(node.X * 10, node.Y * 10); // 描画位置を分離

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                int gx = origin.x + x;
                int gy = origin.y + y;
                map.SetTile(gx, gy, MapData.TileType.Floor);
            }
        }

        // 部屋の中央を記録（通路接続・キャラ配置用）
        node.Position = new Vector2Int(origin.x + size / 2, origin.y + size / 2);
    }

    /// <summary>
    /// 2部屋間にL字型の通路を作成する
    /// </summary>
    private void CreateCorridor(RoomNode from, RoomNode to)
    {
        Vector2Int a = from.Position;
        Vector2Int b = to.Position;

        // 垂直方向の通路
        for (int y = Mathf.Min(a.y, b.y); y <= Mathf.Max(a.y, b.y); y++)
        {
            map.SetTile(b.x, y, MapData.TileType.Floor);
        }

        // 水平方向の通路
        for (int x = Mathf.Min(a.x, b.x); x <= Mathf.Max(a.x, b.x); x++)
        {
            map.SetTile(x, a.y, MapData.TileType.Floor);
        }
    }

    /// <summary>
    /// MapDataの内容をTilemapに反映して描画する
    /// </summary>
    private void DrawTilemapFromMapData()
    {
        floorTilemap.ClearAllTiles(); // 念のためクリア

        for (int x = 0; x < map.Width; x++)
        {
            for (int y = 0; y < map.Height; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                var tileType = map.GetTile(x, y);

                if (tileMap.TryGetValue(tileType, out TileBase tile))
                {
                    floorTilemap.SetTile(pos, tile);
                }
            }
        }
    }


    /// <summary>
    /// 各部屋の種類に応じてキャラクターを配置する
    /// </summary>
    private void SpawnCharacters(RoomNode root)
    {
        foreach (var node in root.Traverse())
        {
            // タイル座標 → ワールド座標に変換（中心に配置）
            Vector3 worldPos = floorTilemap.CellToWorld((Vector3Int)node.Position) + new Vector3(0.5f, 0.5f, 0);

            switch (node.Type)
            {
                case RoomNodeType.Start:
                    Player = Instantiate(playerPrefab, worldPos, Quaternion.identity);
                    break;

                case RoomNodeType.Boss:
                    Instantiate(bossPrefab, worldPos, Quaternion.identity);
                    break;

                case RoomNodeType.Normal:
                case RoomNodeType.Battle:
                    int count = Random.Range(0, 4);
                    for (int i = 0; i < count; i++)
                    {
                        Vector3 offset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
                        Instantiate(enemyPrefab, worldPos + offset, Quaternion.identity);
                    }
                    break;
            }
        }
    }
}
