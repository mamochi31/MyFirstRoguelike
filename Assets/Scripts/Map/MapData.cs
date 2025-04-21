/// <summary>
/// マップ全体のタイルデータを管理するクラス
/// TileType を使って2Dマップの状態を保持・変更する
/// </summary>
public class MapData
{
    public enum TileType
    {
        Empty,
        Floor,         // 部屋用
        Corridor,      // 通路用
        Wall
    }

    public TileType[,] tiles;

    public int Width { get; private set; }
    public int Height { get; private set; }

    /// <summary>
    /// 幅・高さを指定して初期化
    /// </summary>
    public MapData(int width, int height)
    {
        Width = width;
        Height = height;
        tiles = new TileType[width, height];
    }

    /// <summary>
    /// 指定位置のタイルを取得
    /// </summary>
    public TileType GetTile(int x, int y)
    {
        return tiles[x, y];
    }

    /// <summary>
    /// 指定位置のタイルを設定
    /// </summary>
    public void SetTile(int x, int y, TileType type)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
            tiles[x, y] = type;

    }

    /// <summary>
    /// マップの幅
    /// </summary>
    // public int Width => tiles.GetLength(0);

    /// <summary>
    /// マップの高さ
    /// </summary>
    // public int Height => tiles.GetLength(1);

    /// <summary>
    /// 全体を壁で初期化
    /// </summary>
    public void FillWithWalls()
    {
        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                tiles[x, y] = TileType.Wall;
    }

    /// <summary>
    /// 全体を初期化
    /// </summary>
    public void Clear() => Fill(TileType.Empty);

    public bool InBounds(int x, int y)
    {
        return x >= 0 && y >= 0 && x < Width && y < Height;
    }

    /// <summary>
    /// 部屋か通路であるかの判定
    /// </summary>
    public bool IsFloor(int x, int y)
    {
        var tile = GetTile(x, y);
        return tile == TileType.Floor || tile == TileType.Corridor;
    }

    /// <summary>
    /// 指定のタイルで埋める
    /// </summary>
    public void Fill(TileType type)
    {
        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                tiles[x, y] = type;
    }

}
