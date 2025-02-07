using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TilemapSettings", menuName = "Tilemap/Settings", order = 1)]
public class TilemapSettings : ScriptableObject
{
    public TileBase defaultTile; // デフォルトのタイルマップ
    public Sprite defaultSprite;
    public TileRule[] tileRules; // TileRule配列を追加
}

[System.Serializable]
public class TileRule
{
    public string directionString;
    public Tile tile;
    public bool topLeft, top, topRight, right, bottomRight, bottom, bottomLeft, left;

    public bool allowDiagonalCheck = false; // 斜め方向を許容するかどうか（デフォルトはfalse）

    // directionStringを自動的に更新するメソッド
    public void UpdateDirectionString()
    {
        directionString = "";

        if (topLeft) directionString += "Top Left, ";
        if (top) directionString += "Top, ";
        if (topRight) directionString += "Top Right, ";
        if (right) directionString += "Right, ";
        if (bottomRight) directionString += "Bottom Right, ";
        if (bottom) directionString += "Bottom, ";
        if (bottomLeft) directionString += "Bottom Left, ";
        if (left) directionString += "Left, ";

        // 最後のカンマとスペースを削除
        if (directionString.Length > 2)
            directionString = directionString.Substring(0, directionString.Length - 2);
    }
}
