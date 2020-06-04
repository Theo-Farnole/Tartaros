using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    // code from
    // https://forum.unity.com/threads/solved-how-to-show-where-the-players-main-camera-is-located-in-a-minimap.366592/
    [RequireComponent(typeof(Camera))]
    public class MinimapCameraFrustum : MonoBehaviour
    {
        [Tooltip("Materials must have process vertex color to display _frustumColor.")]
        [SerializeField] private Material _material;
        [SerializeField] private bool _useThickness = true;
        [SerializeField] private float thickness = 5f;
        [SerializeField] private Color _frustumColor = Color.red;

        private Plane plane;
        private Vector3 topLeftPosition, topRightPosition, bottomLeftPosition, bottomRightPosition;

        private Camera playerCamera;
        private Camera _minimapCamera;

        public void Start()
        {
            playerCamera = Camera.main;
            _minimapCamera = GetComponent<Camera>();

            plane = new Plane(Vector3.up, Vector3.zero);
        }

        public void Update()
        {
            topLeftPosition = _minimapCamera.WorldToViewportPoint(GetCameraFrustumPosition(new Vector3(0, Screen.height)));
            topRightPosition = _minimapCamera.WorldToViewportPoint(GetCameraFrustumPosition(new Vector3(Screen.width, Screen.height)));
            bottomLeftPosition = _minimapCamera.WorldToViewportPoint(GetCameraFrustumPosition(new Vector3(0f, 0f)));
            bottomRightPosition = _minimapCamera.WorldToViewportPoint(GetCameraFrustumPosition(new Vector3(Screen.width, 0f)));


            topLeftPosition.z = 1f;
            topRightPosition.z = 1f;
            bottomLeftPosition.z = 1f;
            bottomRightPosition.z = 1f;
        }

        private Vector3 GetCameraFrustumPosition(Vector3 position)
        {
            float cameraDistance = Camera.main.transform.position.y;
            Ray positionRay = Camera.main.ScreenPointToRay(position);

            return plane.Raycast(positionRay, out float enter) ? positionRay.GetPoint(enter) : new Vector3();
        }

        public void OnPostRender()
        {
            if (!_material)
            {
                Debug.LogError("Minimap Camera : Please Assign a material on the inspector");
                return;
            }

            GL.PushMatrix();
            {
                _material.SetPass(0);

                GL.LoadOrtho();

                if (_useThickness)
                    DrawQuad();
                else
                    DrawLines();
            }

            GL.PopMatrix();
        }

        void DrawLines()
        {
            GL.Begin(GL.LINES);
            {
                GL.Color(_frustumColor);
                GL.Vertex(bottomLeftPosition);
                GL.Vertex(bottomRightPosition);
                GL.Vertex(bottomRightPosition);
                GL.Vertex(topRightPosition);
                GL.Vertex(topRightPosition);
                GL.Vertex(topLeftPosition);
                GL.Vertex(topLeftPosition);
                GL.Vertex(bottomLeftPosition);
            }
            GL.End();
        }

        // code from 
        // http://rengelbert.com/blog/2015/11/21/drawing-lines-with-unity/
        void DrawQuad()
        {
            List<Vector3> vertices = new List<Vector3>
            {
                bottomLeftPosition,
                bottomRightPosition,
                topRightPosition,
                topLeftPosition,
                bottomLeftPosition
            };

            int i = 0;

            GL.Begin(GL.QUADS);
            {
                GL.Color(_frustumColor);
                while (i < vertices.Count)
                {

                    if (i > 0)
                    {

                        var point1 = vertices[i - 1];
                        var point2 = vertices[i];

                        Vector2 startPoint = Vector2.zero;
                        Vector2 endPoint = Vector2.zero;

                        var diffx = Mathf.Abs(point1.x - point2.x);
                        var diffy = Mathf.Abs(point1.y - point2.y);

                        if (diffx > diffy)
                        {
                            if (point1.x <= point2.x)
                            {
                                startPoint = point1;
                                endPoint = point2;
                            }
                            else
                            {
                                startPoint = point2;
                                endPoint = point1;
                            }
                        }
                        else
                        {
                            if (point1.y <= point2.y)
                            {
                                startPoint = point1;
                                endPoint = point2;
                            }
                            else
                            {
                                startPoint = point2;
                                endPoint = point1;
                            }
                        }

                        var angle = Mathf.Atan2(endPoint.y - startPoint.y, endPoint.x - startPoint.x);
                        var perp = angle + Mathf.PI * 0.5f;

                        var p1 = Vector3.zero;
                        var p2 = Vector3.zero;
                        var p3 = Vector3.zero;
                        var p4 = Vector3.zero;

                        var cosAngle = Mathf.Cos(angle);
                        var cosPerp = Mathf.Cos(perp);
                        var sinAngle = Mathf.Sin(angle);
                        var sinPerp = Mathf.Sin(perp);

                        var distance = Vector2.Distance(startPoint, endPoint);

                        p1.x = startPoint.x - (thickness * 0.5f) * cosPerp;
                        p1.y = startPoint.y - (thickness * 0.5f) * sinPerp;

                        p2.x = startPoint.x + (thickness * 0.5f) * cosPerp;
                        p2.y = startPoint.y + (thickness * 0.5f) * sinPerp;

                        p3.x = p2.x + distance * cosAngle;
                        p3.y = p2.y + distance * sinAngle;

                        p4.x = p1.x + distance * cosAngle;
                        p4.y = p1.y + distance * sinAngle;

                        GL.Vertex3(p1.x, p1.y, 0);
                        GL.Vertex3(p2.x, p2.y, 0);
                        GL.Vertex3(p3.x, p3.y, 0);
                        GL.Vertex3(p4.x, p4.y, 0);

                    }
                    i++;
                }
            }

            GL.End();
        }
    }
}

