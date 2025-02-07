using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class TilemapManualUpdater : MonoBehaviour
{
    [SerializeField] Tilemap tilemap;  // Tilemapを参照

    public TilemapSettings tilemapSettings;  // TilemapSettingsをインスペクタで設定

    private void OnValidate()
    {
        // オブジェクトがシーンに追加されたり、設定が変更された際にタイルを自動で更新
        AutoChangeTiles();
    }

    // タイルの自動切り替え
    public void AutoChangeTiles()
    {
        if (tilemapSettings == null || tilemapSettings.tileRules == null || tilemapSettings.tileRules.Length == 0)
        {
            Debug.LogWarning("No tile rules are defined.");
            return;
        }

        // タイルマップのセルを走査
        BoundsInt bounds = tilemap.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                TileBase tile = tilemap.GetTile(cellPosition);

                if (tile != null)
                {
                    // タイルがある位置の周囲を調べて、適切なTileRuleを適用
                    TileRule matchingRule = GetMatchingTileRule(cellPosition);

                    if (matchingRule != null)
                    {
                        // 新しいタイルに置き換える
                        TileBase newTile = matchingRule.tile; // ルールに基づいた新しいタイルを取得
                        tilemap.SetTile(cellPosition, newTile);
                    }
                    else
                    {
                        // マッチするルールがない場合、デフォルトのタイルを設定
                        tilemap.SetTile(cellPosition, tilemapSettings.defaultTile);
                    }
                }
            }
        }
    }

    private TileRule GetMatchingTileRule(Vector3Int position)
    {
        // 現在のタイルの周囲の状態を調べる
        TileRule[] rules = tilemapSettings.tileRules;

        foreach (TileRule rule in rules)
        {
            if (CheckAdjacentState(position, rule))
            {
                return rule;  // ルールが一致した場合、TileRuleを返す
            }
        }

        return null;  // 一致するルールがない場合
    }

    private bool CheckAdjacentState(Vector3Int position, TileRule rule)
    {
        // 現在のタイルの周囲を調べ、隣接している方向に基づいて状態をチェック
        bool top = tilemap.GetTile(new Vector3Int(position.x, position.y + 1, position.z)) != null;
        bool topRight = tilemap.GetTile(new Vector3Int(position.x + 1, position.y + 1, position.z)) != null;
        bool right = tilemap.GetTile(new Vector3Int(position.x + 1, position.y, position.z)) != null;
        bool bottomRight = tilemap.GetTile(new Vector3Int(position.x + 1, position.y - 1, position.z)) != null;
        bool bottom = tilemap.GetTile(new Vector3Int(position.x, position.y - 1, position.z)) != null;
        bool bottomLeft = tilemap.GetTile(new Vector3Int(position.x - 1, position.y - 1, position.z)) != null;
        bool left = tilemap.GetTile(new Vector3Int(position.x - 1, position.y, position.z)) != null;
        bool topLeft = tilemap.GetTile(new Vector3Int(position.x - 1, position.y + 1, position.z)) != null;

        // 斜め方向を許容する場合は、斜めチェックをスキップ
        if (rule.allowDiagonalCheck)
        {
            topLeft = topRight = bottomLeft = bottomRight = false;
        }

        // // どの方向が隣接しているかをデバッグ表示
        // Debug.Log($"Tile at ({position.x}, {position.y}): " +
        //         $"Top={top}, TopRight={topRight}, Right={right}, BottomRight={bottomRight}, " +
        //         $"Bottom={bottom}, BottomLeft={bottomLeft}, Left={left}, TopLeft={topLeft}");

        // ルールに設定された隣接状態と現在の隣接状態を比較
        bool result = rule.top == top &&
                    rule.topRight == topRight &&
                    rule.right == right &&
                    rule.bottomRight == bottomRight &&
                    rule.bottom == bottom &&
                    rule.bottomLeft == bottomLeft &&
                    rule.left == left &&
                    rule.topLeft == topLeft;

        // Debug.Log($"Checking rule: {result}");
        return result;
    }
}
