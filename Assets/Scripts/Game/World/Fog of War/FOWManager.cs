using Lortedo.Utilities.Pattern;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.FogOfWar
{
    public class FOWManager : Singleton<FOWManager>
    {
        #region Fields
        private const string debugLogHeader = "FOWManager : ";

        [SerializeField] private SnapGridDatabase _snapGrid;
        [SerializeField, HideInInspector] private FogState[,] _visiblityMap; // allow hot reloading in Editor
        [Header("COMPONENTS")]
        [SerializeField] private Projector _projectorFogOfWar;

        [Header("DEBUGS")]
        [SerializeField] private bool _debugDrawSnapGrid = false;

        private List<IFogVision> _viewers = new List<IFogVision>();
        private List<IFogCoverable> _coverables = new List<IFogCoverable>();

        private bool _isDisabled = false;
        #endregion

        #region properties
        public bool IsDisabled { get => _isDisabled; }
        #endregion

        #region Methods
        #region MonoBehaviour Callbacks
        void Awake()
        {
            Assert.IsNotNull(_snapGrid, "Fog of War : Please assign a snapgrid in inspector.");

            _visiblityMap = new FogState[_snapGrid.CellCount, _snapGrid.CellCount];

            // initialize circle with NOT_VISIBLE
            int lenghtOne = _visiblityMap.GetLength(0);

            for (int i = 0; i < lenghtOne; i++)
            {
                int lengthTwo = _visiblityMap.GetLength(1);

                for (int j = 0; j < lengthTwo; j++)
                {
                    _visiblityMap[i, j] = FogState.NotVisible;
                }
            }
        }

        void Update()
        {
            UpdateVisibilityMap();
            UpdateCoverablesVisibility();
        }

        void OnDrawGizmos()
        {
            if (_debugDrawSnapGrid)
                _snapGrid?.DrawGizmos();
        }
        #endregion

        #region Entities Manager
        public void AddViewer(EntityFogVision entity)
        {
            _viewers.Add(entity);
        }

        public void RemoveViewer(EntityFogVision entity)
        {
            _viewers.Remove(entity);
        }

        public void RemoveCoverable(EntityFogCoverable entity)
        {
            _coverables.Remove(entity);
        }

        public void AddCoverable(EntityFogCoverable entity)
        {
            _coverables.Add(entity);
        }
        #endregion

        #region Get Tiles Methods
        private Vector2Int CoordsToUncenteredCoords(Vector2Int centeredCoords, Vector2Int size)
        {
            if (size.x % 2 != 0) size.x--;
            if (size.y % 2 != 0) size.y--;

            Vector2Int uncenteredCoords = centeredCoords - size / 2;

            return uncenteredCoords;
        }

        public bool AreTilesVisible(Vector3 position, Vector2Int size)
            => AreTilesVisible(WorldToCoords(position), size);

        public Vector2Int WorldToCoords(Vector3 position)
            => _snapGrid.WorldToCoords(position);

        public Vector3 CoordsToWorld(Vector2Int coords)
            => _snapGrid.CoordsToWorldPosition(coords);

        public bool AreTilesVisible(Vector2Int coords, Vector2Int size)
        {
            Vector2Int uncenteredCoords = CoordsToUncenteredCoords(coords, size);

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    Vector2Int coordsOffset = new Vector2Int(x, y);
                    Vector2Int tileCoords = uncenteredCoords + coordsOffset;

                    // can't get tile, or fog state isn't visible
                    if (!TryGetTile(tileCoords, out FogState fogState) || (fogState != FogState.Visible))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool TryGetTile(Vector3 worldPosition, out FogState fogState)
            => TryGetTile(_snapGrid.WorldToCoords(worldPosition), out fogState);

        private bool TryGetTile(Vector2Int coords, out FogState fogState)
        {
            if (coords.x < 0 || coords.x >= _visiblityMap.GetLength(0)
                || coords.y < 0 || coords.y >= _visiblityMap.GetLength(1))
            {
                Debug.LogErrorFormat(debugLogHeader + "Coords passed in args aren't in visibility map");
                fogState = FogState.Visible;
                return false;
            }            

            fogState = _visiblityMap[coords.x, coords.y];

            // overwrite output
            if (_isDisabled) fogState = FogState.Visible;
            
            return true;
        }
        #endregion

        #region Visibility Map Routine Methods
        void UpdateVisibilityMap()
        {
            if (_isDisabled)
                return;

            // set all VISIBLE coords to REAVEALED
            int lengthOne = _visiblityMap.GetLength(0);
            for (int x = 0; x < lengthOne; x++)
            {
                int lengthTwo = _visiblityMap.GetLength(1);
                for (int y = 0; y < lengthTwo; y++)
                {
                    if (_visiblityMap[x, y] == FogState.Visible)
                    {
                        _visiblityMap[x, y] = FogState.Revealed;
                    }
                }
            }

            // draw VISIBLE circle from viewers
            for (int i = 0; i < _viewers.Count; i++)
            {
                Vector2Int viewersCoords = _snapGrid.GetNearestCoords(_viewers[i].Transform.position);
                int viewRadius = Mathf.RoundToInt(_viewers[i].ViewRadius / _snapGrid.CellSize);

                _visiblityMap.DrawCircleInside(viewersCoords.x, viewersCoords.y, viewRadius, FogState.Visible);
            }
        }

        void UpdateCoverablesVisibility()
        {
            if (_isDisabled)
                return;

            for (int i = 0; i < _coverables.Count; i++)
            {
                Vector2Int coords = _snapGrid.GetNearestCoords(_coverables[i].Transform.position);

                bool isCover = true;

                if (coords.x >= 0 && coords.x < _visiblityMap.GetLength(0) &&
                    coords.y >= 0 && coords.y < _visiblityMap.GetLength(1))
                {
                    if (_visiblityMap[coords.x, coords.y] == FogState.Visible)
                    {
                        isCover = false;
                    }
                }

                _coverables[i].IsCover = isCover;
            }
        }
        #endregion

        void DisableFOW()
        {
            _isDisabled = true;
            UncoverAllCoverables();

            _projectorFogOfWar.enabled = false;
        }

        void UncoverAllCoverables()
        {
            foreach (var coverable in _coverables)
            {
                coverable.IsCover = false;
            }
        }

        void ReactiveFOW()
        {
            _isDisabled = false;
            _projectorFogOfWar.enabled = true;
        }

        void DebugLogVisiblityMap()
        {
            StringBuilder sb = new StringBuilder();

            for (int y = _visiblityMap.GetLength(1) - 1; y >= 0; y--)
            {
                for (int x = 0; x < _visiblityMap.GetLength(0); x++)
                {
                    if (_visiblityMap[x, y] == FogState.NotVisible) sb.Append(".");
                    else if (_visiblityMap[x, y] == FogState.Revealed) sb.Append("_");
                    else if (_visiblityMap[x, y] == FogState.Visible) sb.Append("X");
                }

                sb.AppendLine();
            }

            Debug.Log(sb.ToString());
        }
        #endregion
    }
}
