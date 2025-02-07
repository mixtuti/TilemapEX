using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

[CustomEditor(typeof(TilemapManualUpdater))]
public class TilemapManualUpdaterEditor : Editor
{
    private TilemapManualUpdater tilemapManualUpdater;

    private void OnEnable()
    {
        tilemapManualUpdater = (TilemapManualUpdater)target;
    }

    public override void OnInspectorGUI()
    {
        // 基本のインスペクターを表示
        base.OnInspectorGUI();

        // Auto Change Tiles ボタンを表示（手動更新用）
        if (GUILayout.Button("Auto Change Tiles"))
        {
            tilemapManualUpdater.AutoChangeTiles();  // タイルを更新
        }
    }
}
