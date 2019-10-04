using UnityEngine;

[System.Serializable]
public class SnapGrid
{
    #region Fields
    [SerializeField] private SnapGridDatabase _data;
    [Space]
    [Header("Debug")]
    [SerializeField] private bool _debugDrawGrid = false;
    [SerializeField] private Color _gridColor = Color.yellow;

    private LayerMask _layerMask = -1;
    private GameObject _plane;
    #endregion

    #region Methods
    public SnapGrid()
    {
        
    }

    public void InstantiatePlane(Transform parent)
    {
        if (_plane != null)
            return;

        _plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        _plane.transform.position = Vector3.zero;
        _plane.transform.rotation = Quaternion.identity;

        _plane.layer = LayerMask.NameToLayer("Grid");
        _plane.name = "Grid Plane";
        _plane.transform.parent = parent;

        _plane.transform.localScale = Vector3.one * _data.CellCount * _data.CellSize / 10; ;

        Object.Destroy(_plane.GetComponent<MeshRenderer>()); // hide _plane
    }

    public Vector3? GetNearestPointFromMouse()
    {
        if (_layerMask == -1) _layerMask = LayerMask.GetMask("Grid");

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _layerMask))
        {
            return GetNearestPointOnGrid(hit.point);
        }
        else
        {
            return null;
        }
    }

    public Vector3 GetNearestPointOnGrid(Vector3 position)
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

    void OnDrawGizmos()
    {
        if (_debugDrawGrid == false)
            return;

        Gizmos.color = _gridColor;
        int midGridCount = _data.CellCount / 2;

        for (int x = -midGridCount; x <= midGridCount; x++)
        {
            for (int z = -midGridCount; z <= midGridCount; z++)
            {
                var point = new Vector3(x * _data.CellSize, 0, z * _data.CellSize);
                Gizmos.DrawSphere(point, 0.05f);
            }
        }

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(Vector3.zero, 0.15f);
    }
    #endregion
}