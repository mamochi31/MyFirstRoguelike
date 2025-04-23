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
    // 生成したマップ
    public RoomTreeGenerator TreeGenerator { get; private set; }
    // マップ生成完了フラグ
    public bool IsGenerated { get; private set; } = false;

    [Header("マップサイズ・タイル")]
    public int mapWidth = 500;
    public int mapHeight = 500;
    public Tilemap floorTilemap;
    public TileBase floorTile;
    public Tilemap miniMapFloorTilemap;
    public TileBase miniMapTile;

    [Header("プレハブ")]
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public GameObject bossPrefab;

    [Header("UI")]
    public RoomUIMapGenerator uiMapGenerator;

    public MapData map;
    private Dictionary<MapData.TileType, TileBase> tileMap;
    private bool hasBoss = false;

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
        TreeGenerator = new RoomTreeGenerator();
        TreeGenerator.GenerateTree();

        // マップデータ初期化
        map = new MapData(mapWidth, mapHeight);
        map.FillWithWalls();

        // 部屋と通路の描画
        DrawRoomTree(TreeGenerator.Root);

        // タイルマップを描画
        DrawTilemapFromMapData();

        // キャラクター配置
        SpawnCharacters(TreeGenerator.Root);

        // 移動用矢印配置
        uiMapGenerator.GenerateUIFromTree(TreeGenerator.Root);
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
        // 各部屋のサイズ
        int xSize = 14;
        int ySize = 8;
        Vector2Int origin = new Vector2Int(node.X * 20, node.Y * 12); // 描画位置を分離

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                int gx = origin.x + x;
                int gy = origin.y + y;
                map.SetTile(gx, gy, MapData.TileType.Floor);
            }
        }

        // 部屋の中央を記録（通路接続・キャラ配置用）
        node.Position = new Vector2Int(origin.x + xSize / 2, origin.y + ySize / 2);
    }

    /// <summary>
    /// 2部屋間にL字型の通路を作成する
    /// </summary>
    private void CreateCorridor(RoomNode from, RoomNode to)
    {
        // 2部屋の座標を取得
        Vector2Int fromP = from.Position;
        Vector2Int toP = to.Position;

        // 分岐部屋のみTOの中心を基準にする
        bool isForkRoom = from.Type == RoomNodeType.Start;
        int midY = isForkRoom ? toP.y : fromP.y;
        int midX = isForkRoom ? fromP.x : toP.x;
        // TOのY座標がFROMより下の場合
        bool isUnder = toP.y < fromP.y;
        if (isUnder) midY--;

        // 水平方向の通路（左右に敷く）
        int minX = Mathf.Min(fromP.x, toP.x);
        int maxX = Mathf.Max(fromP.x, toP.x);
        for (int x = minX; x <= maxX; x++)
        {
            map.SetTile(x, midY, MapData.TileType.Floor);
            map.SetTile(x, midY + (isUnder ? 1 : -1), MapData.TileType.Floor);
        }

        // 垂直方向の通路（上下に敷く）
        int minY = Mathf.Min(fromP.y, toP.y);
        int maxY = Mathf.Max(fromP.y, toP.y);
        for (int y = minY; y <= maxY; y++)
        {
            map.SetTile(midX, y, MapData.TileType.Floor);
            map.SetTile(midX - 1, y, MapData.TileType.Floor);

            if (isUnder)
                map.SetTile(midX - 1, y - 1, MapData.TileType.Floor);
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
                    miniMapFloorTilemap.SetTile(pos, miniMapTile);
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
            Vector3 worldPos = floorTilemap.CellToWorld((Vector3Int)node.Position);

            switch (node.Type)
            {
                // スタート部屋：プレイヤーを1体配置
                case RoomNodeType.Start:
                    Player = Instantiate(playerPrefab, worldPos, Quaternion.identity);
                    break;

                // ボス部屋：ボスを1体配置
                case RoomNodeType.Boss:
                    if (!hasBoss)
                    {
                        Instantiate(bossPrefab, worldPos, Quaternion.identity);
                        hasBoss = true;
                    }
                    break;

                // 通常部屋：今は何もなし
                case RoomNodeType.Normal:
                    break;

                // 戦闘部屋：敵を1体配置
                case RoomNodeType.Battle:
                    Instantiate(enemyPrefab, worldPos, Quaternion.identity);
                    break;
            }
        }
    }
}
