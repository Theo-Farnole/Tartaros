using UnityEngine;

// from
// https://unity3d.college/2017/10/08/simple-unity3d-snap-grid-system/
//
public class Grid : MonoBehaviour
{
    #region Fields
    [SerializeField] private float _cellSize = 1f;
    [Space]
    [Header("Debug")]
    [SerializeField] private bool _drawDebug = false;
    [SerializeField] private Color _gridColor = Color.yellow;
    [SerializeField, Range(5, 25)] private int _debugGridCount = 5;

    private LayerMask _layerMask;
    #endregion

    #region Methods
    void Awake()
    {
        _layerMask = LayerMask.GetMask("Grid");
    }

    void Start()
    {
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.position = Vector3.zero;
        plane.transform.rotation = Quaternion.identity;

        plane.layer = LayerMask.NameToLayer("Grid");
    }

    public Vector3? GetNearestPointFromMouse()
    {
        Vector3 result = Vector3.zero;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, _layerMask))
        {
            result = hit.point;
        }
        else
        {
            return null;
        }

        return GetNearestPointOnGrid(result);
    }

    public Vector3 GetNearestPointOnGrid(Vector3 position)
    {
        float xCount = Mathf.Round(position.x / _cellSize);
        float yCount = Mathf.Round(position.y / _cellSize);
        float zCount = Mathf.Round(position.z / _cellSize);

        Vector3 result = new Vector3(
             xCount * _cellSize,
             yCount * _cellSize,
             zCount * _cellSize);

        Vector3 delta = Vector3.zero;

        if (position.x / _cellSize > xCount) delta.x = 1;
        else delta.x = -1;

        if (position.z / _cellSize > zCount) delta.z = 1;
        else delta.z = -1;


        result += delta * (_cellSize / 2);

        return result;
    }

    void OnDrawGizmos()
    {
        if (_drawDebug == false)
            return;

        Gizmos.color = _gridColor;

        int midGridCount = _debugGridCount / 2;

        for (int x = -midGridCount; x <= midGridCount; x++)
        {
            for (int z = -midGridCount; z <= midGridCount; z++)
            {
                var point = new Vector3(x * _cellSize, 0, z * _cellSize);
                Gizmos.DrawSphere(point, 0.1f);
            }

        }
    }
    #endregion
}