using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Game.Entities.Board;
using UnityEditor;
using UnityEngine;

namespace Editor.Utils
{
    public class TileAssetPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(
            string[] importedAssets, string[] deletedAssets,
            string[] movedAssets, string[] movedFromAssetPaths)
        {
            var customTileGuid = TileSubstitutor.GetCustomTileScriptGuid();
            if (customTileGuid == null) return;

            var toReimport = importedAssets
                .Where(p => p.EndsWith(".asset"))
                .Where(p => TileSubstitutor.TryConvert(p, customTileGuid))
                .ToArray();

            foreach (var path in toReimport)
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
    }

    public static class TileSubstitutor
    {
        private const string BuiltinTileGuid = "0000000000000000e000000000000000";

        [MenuItem("Assets/Convert Tiles to CustomTile", priority = -999)]
        private static void ConvertSelectedFolder()
        {
            var folderPath = GetSelectedFolderPath();
            if (folderPath == null)
            {
                Debug.LogError("TileSubstitutor: Select a folder in the Project window first.");
                return;
            }

            var customTileGuid = GetCustomTileScriptGuid();
            if (customTileGuid == null)
            {
                Debug.LogError("TileSubstitutor: Could not find CustomTile script GUID. Is CustomTile.cs in the project?");
                return;
            }

            var assetPaths = AssetDatabase.FindAssets("t:TileBase", new[] { folderPath })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(p => p.EndsWith(".asset"))
                .ToArray();

            if (assetPaths.Length == 0)
            {
                Debug.Log("TileSubstitutor: No tile assets found in the selected folder.");
                return;
            }

            int converted = 0;
            int skipped = 0;

            foreach (var assetPath in assetPaths)
            {
                if (TryConvert(assetPath, customTileGuid))
                    converted++;
                else
                    skipped++;
            }

            AssetDatabase.Refresh();
            Debug.Log($"TileSubstitutor: Converted {converted} tile(s), skipped {skipped} (already custom or not built-in Tile).");
        }

        [MenuItem("Assets/Convert Tiles to CustomTile", true)]
        private static bool ConvertSelectedFolderValidate()
        {
            return GetSelectedFolderPath() != null;
        }

        public static bool TryConvert(string assetPath, string customTileGuid)
        {
            var absolutePath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", assetPath));
            var text = File.ReadAllText(absolutePath);

            if (!text.Contains(BuiltinTileGuid))
                return false;

            text = SubstituteScriptReference(text, customTileGuid);
            text = text.Replace("m_Sprite:", "_Sprite:");
            File.WriteAllText(absolutePath, text);
            return true;
        }

        private static string SubstituteScriptReference(string yaml, string customTileGuid)
        {
            // Replace the built-in Tile script ref (fileID 13312, type 0) with CustomTile (fileID 11500000, type 3)
            return Regex.Replace(
                yaml,
                @"m_Script:\s*\{fileID:\s*\d+,\s*guid:\s*0000000000000000e000000000000000,\s*type:\s*\d+\}",
                $"m_Script: {{fileID: 11500000, guid: {customTileGuid}, type: 3}}"
            );
        }

        public static string GetCustomTileScriptGuid()
        {
            var script = Resources.FindObjectsOfTypeAll<MonoScript>()
                .FirstOrDefault(s => s.GetClass() == typeof(CustomTile));

            if (script != null)
            {
                var path = AssetDatabase.GetAssetPath(script);
                return AssetDatabase.AssetPathToGUID(path);
            }

            // Fallback: search by type name
            var guids = AssetDatabase.FindAssets($"t:MonoScript CustomTile");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var loaded = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                if (loaded != null && loaded.GetClass() == typeof(CustomTile))
                    return guid;
            }

            return null;
        }

        private static string GetSelectedFolderPath()
        {
            var obj = Selection.activeObject;
            if (obj == null) return null;

            var path = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(path)) return null;

            return Directory.Exists(path) ? path : null;
        }
    }
}
