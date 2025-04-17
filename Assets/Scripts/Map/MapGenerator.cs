using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

/// <summary>
/// ダンジョンマップの自動生成、描画、プレイヤー・敵の配置を行うクラス
/// </summary>
public class MapGenerator : MonoBehaviour
{
    public static MapGenerator Instance { get; private set; }

    [Header("マップ設定")]
    public int width = 50;
    public int height = 30;
    public int maxRooms = 10;
    public int minRoomSize = 5;
    public int maxRoomSize = 10;

    [Header("タイル設定")]
    public Tilemap floorTilemap;
    public Tilemap wallTilemap;
    // public TileBase floorTile;     // 部屋用タイル
    // public TileBase corridorTile;  // 通路用タイル
    // public TileBase wallTile;      // 壁用タイル

    [Header("プレハブ設定")]
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public GameObject bossPrefab;


    // フロアの位置を記録（ランダム出現に使用）
    public List<Vector3Int> floorPositions = new();

    // マップ生成完了フラグ
    public bool IsGenerated { get; private set; } = false;
    // マップデータ
    public MapData map { get; private set; }

    // 部屋リスト
    private List<Room> rooms = new();
    public IReadOnlyList<Room> Rooms => rooms;   // 他クラスからの読み取り用
    // プレイヤーのスポーン部屋
    private Room playerRoom;
    // ボスのスポーン部屋
    private Room bossRoom;

    [SerializeField]
    private TileBase floorTile;
    [SerializeField]
    private TileBase corridorTile;
    [SerializeField]
    private TileBase wallTile;

    private Dictionary<MapData.TileType, TileBase> tileMap;

    private void Awake()
    {
        Instance = this;

        tileMap = new Dictionary<MapData.TileType, TileBase>
        {
            { MapData.TileType.Floor, floorTile },
            { MapData.TileType.Corridor, floorTile },
            { MapData.TileType.Wall, wallTile }
            // ← 今後ここに Door, Trap, Exit などを追加するだけでOK！
        };
    }

    void Start()
    {
        // マップ生成
        GenerateMap();

        // マップ生成完了
        IsGenerated = true;
    }

    /// <summary>
    /// マップを生成し、Tilemapに描画する
    /// </summary>
    public void GenerateMap()
    {
        // 初期化
        IsGenerated = false;
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        floorPositions.Clear();
        rooms.Clear();

        map = new MapData(width, height);
        map.Clear();

        // ランダムに部屋を配置（最大maxRooms個）
        for (int i = 0; i < maxRooms; i++)
        {
            int w = Random.Range(minRoomSize, maxRoomSize + 1);
            int h = Random.Range(minRoomSize, maxRoomSize + 1);
            int x = Random.Range(1, width - w - 1);
            int y = Random.Range(1, height - h - 1);

            Room newRoom = new Room(x, y, w, h);

            // 他の部屋と重なっていないかチェック
            bool overlaps = false;
            foreach (Room room in rooms)
            {
                if (newRoom.Overlaps(room))
                {
                    overlaps = true;
                    break;
                }
            }

            if (!overlaps)
            {
                rooms.Add(newRoom);

                // 部屋を床としてマーク
                for (int rx = x; rx < x + w; rx++)
                    for (int ry = y; ry < y + h; ry++)
                        map.SetTile(rx, ry, MapData.TileType.Floor);
            }
        }

        // 通路生成
        for (int i = 1; i < rooms.Count; i++)
        {
            Vector2Int a = rooms[i - 1].GetEdgePositionToward(rooms[i]);
            Vector2Int b = rooms[i].GetEdgePositionToward(rooms[i - 1]);

            if (Random.value < 0.5f)
            {
                CreateHorizontalCorridor(map, a.x, b.x, a.y);
                CreateVerticalCorridor(map, a.y, b.y, b.x);
            }
            else
            {
                CreateVerticalCorridor(map, a.y, b.y, a.x);
                CreateHorizontalCorridor(map, a.x, b.x, b.y);
            }
        }

        // Tilemapに描画
        DrawMap(map);
    }

    /// <summary>
    /// 2点間に水平通路（1マス幅）を作成し、左右に壁を自動追加
    /// </summary>
    private void CreateHorizontalCorridor(MapData map, int x1, int x2, int y)
    {
        for (int x = Mathf.Min(x1, x2); x <= Mathf.Max(x1, x2); x++)
        {
            if (map.GetTile(x, y) == MapData.TileType.Empty)
                map.SetTile(x, y, MapData.TileType.Corridor);
        }
    }



    /// <summary>
    /// 2点間に垂直通路（1マス幅）を作成し、左右に壁を自動追加
    /// </summary>
    private void CreateVerticalCorridor(MapData map, int y1, int y2, int x)
    {
        for (int y = Mathf.Min(y1, y2); y <= Mathf.Max(y1, y2); y++)
        {
            if (map.GetTile(x, y) == MapData.TileType.Empty)
                map.SetTile(x, y, MapData.TileType.Corridor);
        }
    }

    /// <summary>
    /// MapDataの情報をTilemapに描画する
    /// </summary>
    private void DrawMap(MapData map)
    {
        // 床の描画
        DrawFloors(map);
        // 壁の描画
        DrawWalls(map);
    }

    /// <summary>
    /// 床をTilemapに描画
    /// </summary>
    private void DrawFloors(MapData map)
    {
        for (int x = 0; x < map.Width; x++)
        {
            for (int y = 0; y < map.Height; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                // 部屋も通路も同じタイル
                TileBase tile;
                if (tileMap.TryGetValue(map.GetTile(x, y), out tile))
                {
                    floorTilemap.SetTile(pos, floorTile);
                    // 床・通路のTileを管理用リストに登録する
                    AddFloorTilePosition(pos);
                }
            }
        }
    }

    /// <summary>
    /// 指定座標を floorPositions に追加（共通処理）
    /// </summary>
    private void AddFloorTilePosition(Vector3Int pos)
    {
        if (!floorPositions.Contains(pos))
            floorPositions.Add(pos);
    }

    /// <summary>
    /// 壁をTilemapに描画
    /// </summary>
    private void DrawWalls(MapData map)
    {
        // 床に隣接する空きマスにのみ壁を設置
        for (int x = 0; x < map.Width; x++)
        {
            for (int y = 0; y < map.Height; y++)
            {
                if (map.GetTile(x, y) != MapData.TileType.Empty) continue;

                bool nearFloor = false;
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0) continue;
                        if (map.InBounds(x + dx, y + dy) && map.IsFloor(x + dx, y + dy))
                        {
                            nearFloor = true;
                            break;
                        }
                    }
                    if (nearFloor) break;
                }

                if (nearFloor)
                {
                    wallTilemap.SetTile(new Vector3Int(x, y, 0), wallTile);
                }
            }
        }
    }

    /// <summary>
    /// いずれかの部屋にプレイヤーを配置する
    /// </summary>
    public GameObject SpawnPlayer()
    {
        if (playerPrefab == null || rooms.Count == 0)
            return null;

        // ランダムな部屋を1つ選択
        playerRoom = rooms[Random.Range(0, rooms.Count)];
        playerRoom.Type = RoomType.Start;
        Vector3Int center = playerRoom.GetCenter3();
        Vector3 worldPos = floorTilemap.CellToWorld(center) + new Vector3(0.5f, 0.5f, 0);

        return Instantiate(playerPrefab, worldPos, Quaternion.identity);
    }


    /// <summary>
    /// 各部屋に対して最大3体の敵を生成する
    /// プレーヤーと同じ部屋にはスポーン不可
    /// </summary>
    public int SpawnEnemies()
    {
        if (enemyPrefab == null || rooms.Count == 0)
            return 0;

        int count = 0;

        foreach (Room room in rooms)
        {
            if (room.Type == RoomType.Start || room.Type == RoomType.Boss) continue;

            int enemyCountInRoom = Random.Range(1, 4); // 1〜3体

            for (int i = 0; i < enemyCountInRoom; i++)
            {
                Vector2Int pos = room.GetRandomPosition(); // GetCenter ではなく部屋内ランダム位置
                Vector3 worldPos = floorTilemap.CellToWorld(new Vector3Int(pos.x, pos.y, 0)) + new Vector3(0.5f, 0.5f, 0);
                Instantiate(enemyPrefab, worldPos, Quaternion.identity);
                count++;
            }
        }

        return count;
    }

    /// <summary>
    /// ボスをスポーンさせる
    /// プレイヤー部屋が選ばれた後に実行する
    /// </summary>
    public GameObject SpawnBoss()
    {
        // ボス部屋を決定する
        List<Room> bossCandidates = new(rooms);
        bossCandidates.Remove(playerRoom); // プレイヤー部屋は除外

        bossRoom = bossCandidates[Random.Range(0, bossCandidates.Count)];

        // ボスをスポーンさせる
        if (bossPrefab == null || bossRoom == null) return null;
        bossRoom.Type = RoomType.Boss;

        Vector3Int center = bossRoom.GetCenter3();
        Vector3 worldPos = floorTilemap.CellToWorld(center) + new Vector3(0.5f, 0.5f, 0);

        GameObject boss = Instantiate(bossPrefab, worldPos, Quaternion.identity);
        boss.GetComponent<EnemyHealth>().isBoss = true;

        return boss;
    }

}
