using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FogOfWar
{
    public class FOWManager : Singleton<FOWManager>
    {
        #region Struct
        
        #endregion

        #region Fields
        public static readonly int NOT_VISIBLE = 0;
        public static readonly int REVEALED = 1;
        public static readonly int VISIBLE = 2;

        [SerializeField] private SnapGrid _snapGrid;
        private int[,] _visiblityMap;

        private List<FOWEntity> _viewers = new List<FOWEntity>();
        private List<FOWCoverable> _coverables = new List<FOWCoverable>();
        #endregion

        #region Methods
        #region MonoBehaviour Callbacks
        void Awake()
        {
            _visiblityMap = new int[_snapGrid.Data.CellCount, _snapGrid.Data.CellCount];

            // initialize circle with NOT_VISIBLE
            for (int i = 0; i < _visiblityMap.GetLength(0); i++)
            {
                for (int j = 0; j < _visiblityMap.GetLength(1); j++)
                {
                    _visiblityMap[i, j] = NOT_VISIBLE;
                }
            }
        }

        void Update()
        {
            // update fog each 3 frames
            if (Time.frameCount % 3 == 0)
            {
                UpdateVisibilityMap();
                UpdateCoverablesVisibility();
            }
        }

        void OnDrawGizmos()
        {
            _snapGrid.DrawGizmos();
        }
        #endregion

        #region Entities Manager
        public void AddViewer(FOWEntity entity)
        {
            _viewers.Add(entity);
        }

        public void RemoveViewer(FOWEntity entity)
        {
            _viewers.Remove(entity);
        }

        public void RemoveCoverable(FOWCoverable coverable)
        {
            _coverables.Remove(coverable);
        }

        public void AddCoverable(FOWCoverable coverable)
        {
            _coverables.Add(coverable);
        }
        #endregion

        void UpdateVisibilityMap()
        {
            // set all VISIBLE coords to REAVEALED
            for (int x = 0; x < _visiblityMap.GetLength(0); x++)
            {
                for (int y = 0; y < _visiblityMap.GetLength(1); y++)
                {
                    if (_visiblityMap[x, y] == VISIBLE)
                    {
                        _visiblityMap[x, y] = REVEALED;
                    }
                }
            }

            // draw VISIBLE circle from viewers
            for (int i = 0; i < _viewers.Count; i++)
            {
                Vector2Int viewersCoords = _snapGrid.GetNearestCoords(_viewers[i].Transform.position);
                int viewRadius = Mathf.RoundToInt(_viewers[i].ViewRadius / _snapGrid.Data.CellSize);

                _visiblityMap.DrawCircleInside(viewersCoords.x, viewersCoords.y, viewRadius, VISIBLE);
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
                    if (_visiblityMap[coords.x, coords.y] == VISIBLE)
                    {
                        isCover = false;
                    }
                }

                _coverables[i].Cover(isCover);
            }
        }

        public void DebugLogVisiblityMap()
        {
            StringBuilder sb = new StringBuilder();

            for (int y = _visiblityMap.GetLength(1) - 1; y >= 0; y--)
            {
                for (int x = 0; x < _visiblityMap.GetLength(0); x++)
                {
                    if (_visiblityMap[x, y] == NOT_VISIBLE) sb.Append(".");
                    else if (_visiblityMap[x, y] == REVEALED) sb.Append("_");
                    else if (_visiblityMap[x, y] == VISIBLE) sb.Append("X");
                }

                sb.AppendLine();
            }

            Debug.Log(sb.ToString());
        }
        #endregion
    }
}
