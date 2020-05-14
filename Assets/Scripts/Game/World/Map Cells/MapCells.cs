using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LeonidasLegacy.MapCellEditor
{
    [CreateAssetMenu(menuName = "Tartaros/World/Map Cell")]
    public class MapCells : SerializedScriptableObject
    {
        private static readonly float gizmos_cellSizePercent = 0.9f;
        private static readonly float gizmos_offsetPositionY = 0.1f;

        // We could have inherited MapsCells from SnapGridDatabase
        // However, MapCells need Odin's serialization
        // and SnapGridDatabase don't need it.
        //
        // We must use Unity serialisation as possible
        // So, we don't make SnapGridDatabsae use Odin's serialization
        [InlineEditor]
        [SerializeField] private SnapGridDatabase _snapGrid;

        // we hide _mapContent for performance reason       
        [SerializeField, HideInInspector] private CellType[,] _mapContent = new CellType[5, 5];


        public void SetCellTypeFromWorldPosition(float x, float y, CellType cellType)
        {
            int worldX = Mathf.RoundToInt(x * _snapGrid.CellSize);
            int worldY = Mathf.RoundToInt(y * _snapGrid.CellSize);

            SetCellTypeFromLocalPosition(worldX, worldY, cellType);
        }

        public void SetCellTypeFromLocalPosition(int x, int y, CellType cellType)
        {
            if (x < 0 || x >= _mapContent.GetLength(0) ||
                y < 0 || y >= _mapContent.GetLength(1))
            {
                Debug.LogErrorFormat("Map Cells : Can't set cell at coords [{0}; {1}], because it's out of map.", x, y);
                return;
            }

            _mapContent[x, y] = cellType;

            Debug.LogFormat("Map Cells : " + "Set cellType {0} at coords [{1};{2}]", cellType, x, y);
        }

        #region Draw Gizmos
        private Dictionary<CellType, Color> _debugGizmosColor;

        public void DrawGizmos()
        {
            if (_snapGrid == null)
            {
                Debug.LogErrorFormat("Map Cells: Please assign a snap grid to DrawGizmos().");
                return;
            }

            if (_mapContent == null || _mapContent.GetLength(0) == 0 || _mapContent.GetLength(1) == 0)
            {
                Debug.LogErrorFormat("Map Cells : _mapContent is null or empty. Can't DrawGizmos().");
                return;
            }

            int visibleDrawn = 0;

            float cellSize1D = _snapGrid.CellSize;
            Vector3 cellSize3D = new Vector3(cellSize1D, 0, cellSize1D) * gizmos_cellSizePercent;
            int gizmos_maxVisible = MapCellsDrawerSettings.Gizmos_MaxVisibleCells;

            if (_debugGizmosColor == null || _debugGizmosColor[CellType.Forest].a != MapCellsDrawerSettings.Gizmos_CellOpacity)
                _debugGizmosColor = CalculateFillColors();

            for (int x = 0; x < _mapContent.GetLength(0); x++)
            {
                for (int y = 0; y < _mapContent.GetLength(1); y++)
                {
                    if (visibleDrawn >= gizmos_maxVisible)
                        return;

                    var position = GetCellPosition(cellSize1D, x, y);

                    if (IsVisibleBySceneViewCamera(position))
                    {
                        visibleDrawn++;
                        DrawGizmos_Cell(cellSize3D, position, _debugGizmosColor[_mapContent[x, y]]);
                    }
                }
            }
        }

        static Vector3 GetCellPosition(float cellSize, int x, int y)
        {
            return cellSize * new Vector3(x, gizmos_offsetPositionY, y);
        }

        bool IsVisibleBySceneViewCamera(Vector3 point)
        {
            // https://answers.unity.com/questions/720447/if-game-object-is-in-cameras-field-of-view.html

            if (Camera.current == null)
            {
                Debug.LogErrorFormat("SceneView's camera is null. Returning false.");
                return false;
            }

            Vector3 screenPoint = Camera.current.WorldToViewportPoint(point);
            bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;

            return onScreen;
        }

        void DrawGizmos_Cell(Vector3 size, Vector3 position, Color fillColor)
        {
            // draw fill collor
            Gizmos.color = fillColor;
            Gizmos.DrawCube(position, size);
        }

        Dictionary<CellType, Color> CalculateFillColors()
        {
            var o = new Dictionary<CellType, Color>();

            foreach (CellType cellType in Enum.GetValues(typeof(CellType)))
            {
                Color fillColor = cellType.GetColor();
                fillColor.a = MapCellsDrawerSettings.Gizmos_CellOpacity;

                o.Add(cellType, fillColor);
            }

            return o;
        }
        #endregion

        #region Adjust map size
        void TryAdjustMapSize()
        {
            if (_mapContent.GetLength(0) != _snapGrid.CellCount || _mapContent.GetLength(1) != _snapGrid.CellCount)
                AdjustMapSize();
        }

        [Button]
        void AdjustMapSize()
        {
            Debug.Log("Adjusting map cells size... It's can be very long.");

            _mapContent = ResizeArray(_mapContent, _snapGrid.CellCount, _snapGrid.CellCount);

            Debug.Log("Adjusting map cells is finish!");
        }

        // from
        // https://stackoverflow.com/questions/6539571/how-to-resize-multidimensional-2d-array-in-c
        protected T[,] ResizeArray<T>(T[,] original, int x, int y)
        {
            T[,] newArray = new T[x, y];
            int minX = Math.Min(original.GetLength(0), newArray.GetLength(0));
            int minY = Math.Min(original.GetLength(1), newArray.GetLength(1));

            for (int i = 0; i < minY; ++i)
                Array.Copy(original, i * original.GetLength(0), newArray, i * newArray.GetLength(0), minX);

            return newArray;
        }
        #endregion
    }
}
