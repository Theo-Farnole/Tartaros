using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Game.MapCellEditor
{
    public enum CellType
    {
        Walkable,
        Sea,
        Mountains,
        Farm_LowFertility,
        Farm_MedFertility,
        Farm_HighFertility,
        Stone,
        Forest
    }

    public static class CellTypeExtension
    {
        public static Color GetColor(this CellType cellType)
        {
#if UNITY_EDITOR
            string editorKey = "CellColor_" + cellType;
            string hex = EditorPrefs.GetString(editorKey, GetDefaultColor(cellType));

            return hex.HexToColor();
#else
            Debug.LogWarning("CellType.ToColor can only be used in Editor. We'll return magenta.");
            return Color.magenta;
#endif
        }

        private static string GetDefaultColor(this CellType cellType)
        {
            switch (cellType)
            {
                case CellType.Walkable:
                    return "ECF0F1";
                case CellType.Sea:
                    return "3498DB";
                case CellType.Mountains:
                    return "2980B9";
                case CellType.Farm_LowFertility:
                    return "E74C3C";
                case CellType.Farm_MedFertility:
                    return "E67E22";
                case CellType.Farm_HighFertility:
                    return "27AE60";
                case CellType.Stone:
                    return "34495E";
                case CellType.Forest:
                    return "8E44AD";
                default:
                    throw new System.NotImplementedException();
            }
        }
    }
}