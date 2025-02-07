using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[CustomEditor(typeof(TilemapSettings))]
public class TilemapSettingsEditor : Editor
{
    private const float Radius = 50f; // 魔法陣の半径
    private const float AngleStep = 45f; // 8方向で360度を分ける

    // TileRuleごとの折りたたみ状態を保持するための配列
    private bool[] foldoutStates;

    public override void OnInspectorGUI()
    {
        // 基本のインスペクターを表示
        base.OnInspectorGUI();

        TilemapSettings settings = (TilemapSettings)target;

        // タイルルールの表示
        if (settings.tileRules != null && settings.tileRules.Length > 0)
        {
            // foldoutStates 配列の初期化
            if (foldoutStates == null || foldoutStates.Length != settings.tileRules.Length)
            {
                foldoutStates = new bool[settings.tileRules.Length];
            }

            // 各 Tile Rule の折りたたみ表示
            for (int i = 0; i < settings.tileRules.Length; i++)
            {
                GUILayout.Space(10);

                TileRule rule = settings.tileRules[i];

                // "Adjacent Directions" の折りたたみ状態を制御
                foldoutStates[i] = EditorGUILayout.Foldout(foldoutStates[i], "Adjacent Directions: " + rule.directionString);

                if (foldoutStates[i])
                {
                    // directionStringを更新
                    rule.UpdateDirectionString();

                    // directionStringを表示
                    GUILayout.Label("Tile Rule: " + rule.tile.name, EditorStyles.boldLabel);

                    // 魔法陣の描画
                    DrawTileRuleCircle(rule);
                }
            }
        }
        else
        {
            GUILayout.Label("No Tile Rules defined.");
        }
    }

    // 魔法陣風に隣接情報を描画
    private void DrawTileRuleCircle(TileRule rule)
    {
        // 魔法陣を描画するRectを設定
        Rect rect = GUILayoutUtility.GetRect(130f, 130f);
        Vector2 center = new Vector2(rect.x + rect.width / 2, rect.y + rect.height / 2);

        // 8方向の隣接状態を視覚的に表示
        for (int i = 0; i < 8; i++)
        {
            Vector2 direction = GetDirection(i);
            bool isAdjacent = GetAdjacentState(rule, i);

            // 線の始点と終点を修正
            Vector2 lineStart = center;
            Vector2 lineEnd = center + direction * Radius;

            // 線を描画（色変更をしない）
            Handles.DrawLine(lineStart, lineEnd);

            // 隣接状態に応じた円の色
            Color circleColor = isAdjacent ? Color.green : Color.red;

            // 円を描画
            Handles.color = circleColor;
            Handles.DrawSolidDisc(lineEnd, Vector3.forward, 10f);  // 小さな円

            // 各円をクリック可能にして、クリックされたらブール値を変更
            if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition) && 
                Vector2.Distance(lineEnd, Event.current.mousePosition) < 10f)
            {
                SetAdjacentState(rule, i, !isAdjacent); // クリックされた方向の隣接情報を反転
                Event.current.Use(); // イベントを使ったことにする
            }
        }

        // 中央に元のタイルを描画
        Handles.color = Color.white;
        Handles.DrawSolidDisc(center, Vector3.forward, 10f);  // 中央円
    }

    // 方向に応じて隣接状態を取得
    private bool GetAdjacentState(TileRule rule, int direction)
    {
        switch (direction)
        {
            case 0: return rule.top;
            case 1: return rule.topRight;
            case 2: return rule.right;
            case 3: return rule.bottomRight;
            case 4: return rule.bottom;
            case 5: return rule.bottomLeft;
            case 6: return rule.left;
            case 7: return rule.topLeft;
            default: return false;
        }
    }

    // 方向ごとのベクトルを返す
    private Vector2 GetDirection(int index)
    {
        switch (index)
        {
            case 0: return new Vector2(0, -1);       // 上
            case 1: return new Vector2(1, -1);       // 右上
            case 2: return new Vector2(1, 0);       // 右
            case 3: return new Vector2(1, 1);      // 右下
            case 4: return new Vector2(0, 1);      // 下
            case 5: return new Vector2(-1, 1);     // 左下
            case 6: return new Vector2(-1, 0);      // 左
            case 7: return new Vector2(-1, -1);      // 左上
            default: return Vector2.zero;
        }
    }

    // 方向に応じて隣接情報をセット
    private void SetAdjacentState(TileRule rule, int direction, bool value)
    {
        switch (direction)
        {
            case 0: rule.top = value; break;
            case 1: rule.topRight = value; break;
            case 2: rule.right = value; break;
            case 3: rule.bottomRight = value; break;
            case 4: rule.bottom = value; break;
            case 5: rule.bottomLeft = value; break;
            case 6: rule.left = value; break;
            case 7: rule.topLeft = value; break;
        }

        EditorUtility.SetDirty(target); // 変更があったことを通知
    }
}
