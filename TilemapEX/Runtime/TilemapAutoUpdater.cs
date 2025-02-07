using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class TilemapAutoUpdater : MonoBehaviour
{
    [SerializeField] Tilemap tilemap;
    public TilemapSettings tilemapSettings;  // TilemapSettingsをインスペクタで設定

    // Gizmosを使ってタイルマップの変更を監視する
    private void OnDrawGizmos()
    {
        // TilemapSettings が設定されていない場合は何もしない
        if (tilemapSettings == null || tilemapSettings.tileRules == null || tilemapSettings.tileRules.Length == 0)
        {
            Debug.LogWarning("No tile rules are defined.");
            return;
        }

        AutoChangeTiles();  // タイルを自動的に更新
    }

    // タイルの自動切り替え
    public void AutoChangeTiles()
    {
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
                        TileBase newTile = matchingRule.tile;
                        tilemap.SetTile(cellPosition, newTile);
                    }
                }
            }
        }
    }

    private TileRule GetMatchingTileRule(Vector3Int position)
    {
        TileRule[] rules = tilemapSettings.tileRules;

        foreach (TileRule rule in rules)
        {
            if (CheckAdjacentState(position, rule))
            {
                return rule;
            }
        }

        return null;
    }

    private bool CheckAdjacentState(Vector3Int position, TileRule rule)
    {
        // 現在のタイルの周囲を調べる
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

        // ルールに設定された隣接状態と現在の隣接状態を比較
        return rule.top == top &&
               rule.topRight == topRight &&
               rule.right == right &&
               rule.bottomRight == bottomRight &&
               rule.bottom == bottom &&
               rule.bottomLeft == bottomLeft &&
               rule.left == left &&
               rule.topLeft == topLeft;
    }
}
