using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FogOfWar
{
    public class FOWManager : Singleton<FOWManager>
    {
        #region Struct
        [System.Serializable]
        public struct Coverable
        {
            public Transform transform;
            [Space]
            public Collider collider;
            public Renderer renderer;
        }
        #endregion

        #region Fields
        public static readonly int NOT_VISIBLE = 0;
        public static readonly int REVEALED = 1;
        public static readonly int VISIBLE = 2;

        [SerializeField] private SnapGrid _snapGrid;
        private int[,] _visiblityMap;

        private List<FOWEntity> _viewers = new List<FOWEntity>();
        private List<Coverable> _coverables = new List<Coverable>();
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

        public void AddCoverable(Coverable coverable)
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
                Vector2Int viewersCoords = _snapGrid.GetNearestCoords(_viewers[i].transform.position);
                int viewRadius = Mathf.RoundToInt(_viewers[i].ViewRadius / _snapGrid.Data.CellSize);

                _visiblityMap.DrawCircleInside(viewersCoords.x, viewersCoords.y, viewRadius, VISIBLE);
            }
        }

        void UpdateCoverablesVisibility()
        {
            for (int i = 0; i < _coverables.Count; i++)
            {
                Vector2Int coords = _snapGrid.GetNearestCoords(_coverables[i].transform.position);

                bool displayCoverable = true;

                if (coords.x >= 0 && coords.x < _visiblityMap.GetLength(0) &&
                    coords.y >= 0 && coords.y < _visiblityMap.GetLength(1))
                {
                    if (_visiblityMap[coords.x, coords.y] == VISIBLE)
                    {
                        displayCoverable = true;
                    }
                    else
                    {
                        displayCoverable = false;
                    }
                }

                _coverables[i].renderer.enabled = displayCoverable;
                _coverables[i].collider.enabled = displayCoverable;
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
