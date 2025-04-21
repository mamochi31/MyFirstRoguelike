using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// ミニマップを生成して RawImage に表示する
/// </summary>
public class MiniMapController : MonoBehaviour
{
    public RawImage miniMapImage; // UIに配置した RawImage
    public int pixelPerTile = 4;  // 1タイル = 4ピクセル角など
    public Color wallColor = Color.black;
    public Color floorColor = Color.gray;
    public Color corridorColor = Color.gray;
    public Color playerColor = Color.red;

    private Texture2D texture;
    private MapData mapData;
    private GameObject player;

    // タイルカラー辞書
    private Dictionary<MapData.TileType, Color> tileColors = new();

    // プレイヤーの現在地
    Vector3Int lastPlayerCell;

    void Awake()
    {
        // タイルカラーをセット
        tileColors[MapData.TileType.Floor] = floorColor;
        tileColors[MapData.TileType.Corridor] = corridorColor;
        tileColors[MapData.TileType.Wall] = wallColor;
        // 必要に応じて追加
    }

    public void Initialize(MapData map, GameObject playerObj)
    {
        mapData = map;
        player = playerObj;

        int w = map.Width * pixelPerTile;
        int h = map.Height * pixelPerTile;
        texture = new Texture2D(w, h);
        texture.filterMode = FilterMode.Point;

        miniMapImage.texture = texture;

        RedrawMap();
    }

    void Update()
    {
        if (mapData == null || player == null) return;

        // プレイヤーが動いたときにミニマップを更新
        Vector3Int current = MapGeneratorTree.Instance.floorTilemap.WorldToCell(player.transform.position);
        if (current != lastPlayerCell)
        {
            RedrawMap();
            lastPlayerCell = current;
        }
    }

    void RedrawMap()
    {
        for (int x = 0; x < mapData.Width; x++)
        {
            for (int y = 0; y < mapData.Height; y++)
            {
                MapData.TileType type = mapData.GetTile(x, y);
                Color color = tileColors.ContainsKey(type) ? tileColors[type] : Color.black;
                FillTile(x, y, color);
            }
        }

        // プレイヤーの位置を描画（マップ上座標に変換）
        Vector3 world = player.transform.position;
        Vector3Int cell = MapGeneratorTree.Instance.floorTilemap.WorldToCell(world);
        FillTile(cell.x, cell.y, playerColor);

        texture.Apply();
    }

    void FillTile(int tileX, int tileY, Color color)
    {
        int px = tileX * pixelPerTile;
        int py = tileY * pixelPerTile;

        for (int dx = 0; dx < pixelPerTile; dx++)
        {
            for (int dy = 0; dy < pixelPerTile; dy++)
            {
                texture.SetPixel(px + dx, py + dy, color);
            }
        }
    }
}
