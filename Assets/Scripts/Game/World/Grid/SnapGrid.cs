using UnityEngine;

public class SnapGrid : MonoBehaviour
{
    #region Fields
    [SerializeField] private TileDatabase _data;
    [Space]
    [Header("Debug")]
    [SerializeField] private bool _debugDrawGrid = false;
    [SerializeField] private Color _gridColor = Color.yellow;

    private LayerMask _layerMask;
    #endregion

    #region Methods
    void Awake()
    {
        _layerMask = LayerMask.GetMask("Grid");
    }

    void Start()
    {
        var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.position = Vector3.zero;
        plane.transform.rotation = Quaternion.identity;

        plane.layer = LayerMask.NameToLayer("Grid");
        plane.name = "Grid Plane";
        plane.transform.parent = transform;

        // update Scale
        Vector3 scale = Vector3.one;
        scale *= _data.CellCount * _data.CellSize / 10;

        plane.transform.localScale = scale;
    }

    public Vector3? GetNearestPointFromMouse()
    {
        Vector3 result = Vector3.zero;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, _layerMask))
        {
            result = hit.point;
            return GetNearestPointOnGrid(result);
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