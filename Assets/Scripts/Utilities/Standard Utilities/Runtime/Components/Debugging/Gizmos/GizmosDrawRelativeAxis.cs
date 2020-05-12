using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allow draw debug axis in the SceneView.
/// </summary>
public class GizmosDrawRelativeAxis : MonoBehaviour
{
    [System.Serializable]
    public struct Axis
    {
        public bool draw;
        public Color drawColor;

        public Axis(bool draw, Color drawColor)
        {
            this.draw = draw;
            this.drawColor = drawColor;
        }

        public void DrawAxis(Vector3 origin, Vector3 direction, float length = 1)
        {
            if (draw)
            {
                DrawArrow.ForGizmo(origin, direction * length, drawColor);
                DrawArrow.ForGizmo(origin, direction * length, drawColor);
            }
        }
    }

    [SerializeField] private bool _drawOnSelectedOnly = false;
    [SerializeField] private float _axisLength = 1;
    [SerializeField] private Vector3 _offset;
    [Header("AXIS")]
    public Axis drawUpAxis = new Axis(true, Color.blue);
    public Axis drawDownAxis = new Axis(true, Color.blue);
    public Axis drawForwardAxis = new Axis(true, Color.blue);
    public Axis drawBackwardAxis = new Axis(true, Color.blue);
    public Axis drawRightAxis = new Axis(true, Color.blue);
    public Axis drawLeftAxis = new Axis(true, Color.blue);

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        DrawAxis();
    }

    public void DrawAxis()
    {
        // this method is called from custom editor

        bool isSelected = UnityEditor.Selection.activeGameObject == gameObject;

        if (_drawOnSelectedOnly && isSelected || !_drawOnSelectedOnly)
        {
            Vector3 rootPosition = transform.position + _offset;

            drawUpAxis.DrawAxis(rootPosition, transform.up, _axisLength);
            drawDownAxis.DrawAxis(rootPosition, -transform.up, _axisLength);
            drawForwardAxis.DrawAxis(rootPosition, transform.forward, _axisLength);
            drawBackwardAxis.DrawAxis(rootPosition, -transform.forward, _axisLength);
            drawRightAxis.DrawAxis(rootPosition, transform.right, _axisLength);
            drawLeftAxis.DrawAxis(rootPosition, -transform.right, _axisLength);
        }
    }

    public void DisableEveryAxis()
    {
        drawUpAxis.draw = false;
        drawDownAxis.draw = false;
        drawForwardAxis.draw = false;
        drawBackwardAxis.draw = false;
        drawRightAxis.draw = false;
        drawLeftAxis.draw = false;
    }
#endif
}
