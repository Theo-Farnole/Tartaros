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

        #region Setter
        public void SetCellType_WorldPosition(float worldX, float worldY, CellType cellType)
        {            
            ToLocalPosition(worldX, worldY, out int localX, out int localY);
            SetCellType_LocalPosition(localX, localY, cellType);
        }

        public void SetCellType_LocalPosition(int localX, int localY, CellType cellType)
        {
            if (localX < 0 || localX >= _mapContent.GetLength(0) ||
                localY < 0 || localY >= _mapContent.GetLength(1))
            {
                Debug.LogErrorFormat("Map Cells : Can't set cell at coords [{0}; {1}], because it's out of map.", localX, localY);
                return;
            }

            _mapContent[localX, localY] = cellType;
        }

        public void SetCellType_WorldPosition(float worldX, float worldY, CellType cellType, float radius)
        {
            ToLocalPosition(worldX, worldY, out int localX, out int localY);
            int localRadius = Mathf.RoundToInt(radius * _snapGrid.CellSize);

            SetCellType_LocalPosition(localX, localY, cellType, localRadius);
        }

        public void SetCellType_LocalPosition(int x, int y, CellType cellType, int radius)
        {
            _mapContent.DrawCircleInside(x, y, radius, cellType);
        }

        private void ToLocalPosition(float worldX, float worldY, out int localX, out int localY)
        {
            localX = Mathf.RoundToInt(worldX * _snapGrid.CellSize);
            localY = Mathf.RoundToInt(worldY * _snapGrid.CellSize);
        }

        #endregion

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
