using UnityEngine;

[System.Serializable]
public class SnapGrid
{
    #region Fields
    [SerializeField] private SnapGridDatabase _data;
    [Space]
    [Header("Debug")]
    [SerializeField] private bool _debugDrawGrid = false;
    [Space]
    [SerializeField] private Color _gridColor = Color.yellow;

    private LayerMask _layerMask = -1;
    private GameObject _plane;
    #endregion

    #region Properties
    public SnapGridDatabase Data { get => _data; }
    #endregion

    #region Methods
    public SnapGrid() { }

    public Vector3? GetNearestPositionFromMouse()
    {
        if (_layerMask == -1) _layerMask = LayerMask.GetMask("Terrain");

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _layerMask))
        {
            return GetNearestPosition(hit.point);
        }
        else
        {
            return null;
        }
    }

    public Vector2Int GetNearestCoords(Vector3 position)
    {
        int xCount = Mathf.RoundToInt(position.x / _data.CellSize);
        int zCount = Mathf.RoundToInt(position.z / _data.CellSize);

        return new Vector2Int(xCount, zCount);
    }

    public Vector3 GetNearestPosition(Vector3 position)
    {
        float xCount = Mathf.Round(position.x / _data.CellSize);
        float yCount = Mathf.Round(position.y / _data.CellSize);
        float zCount = Mathf.Round(position.z / _data.CellSize);

        Vector3 result = new Vector3(
             xCount * _data.CellSize,
             yCount * _data.CellSize,
             zCount * _data.CellSize);

        return result;
    }

    public void DrawGizmos()
    {
        if (_debugDrawGrid == false)
            return;

        Gizmos.color = _gridColor;

        for (int x = 0; x <= _data.CellCount; x++)
        {
            for (int z = 0; z <= _data.CellCount; z++)
            {
                var point = new Vector3(x * _data.CellSize, 0, z * _data.CellSize);
                Gizmos.DrawWireCube(point, new Vector3(_data.CellSize, 0, _data.CellSize));
            }
        }

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(Vector3.zero, 0.15f);
    }
    #endregion
}