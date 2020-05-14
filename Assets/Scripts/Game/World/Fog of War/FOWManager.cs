using Lortedo.Utilities.Pattern;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Game.FogOfWar
{
    public class FOWManager : Singleton<FOWManager>
    {
        #region Fields
        [SerializeField] private SnapGridDatabase _snapGrid;
        [SerializeField, HideInInspector] private FogState[,] _visiblityMap; // allow hot reloading in Editor

        [Header("DEBUGS")]
        [SerializeField] private bool _debugDrawSnapGrid = false;

        private List<IFogVision> _viewers = new List<IFogVision>();
        private List<IFogCoverable> _coverables = new List<IFogCoverable>();
        #endregion

        #region Methods
        #region MonoBehaviour Callbacks
        void Awake()
        {
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

        void UpdateVisibilityMap()
        {
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

        public void DebugLogVisiblityMap()
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
