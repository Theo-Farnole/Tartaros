using UnityEngine;
using System.Collections;

/// <author>
/// That's not my work, but I've lost the link ! :/
/// </author>

namespace Lortedo.Utilities
{
    [RequireComponent(typeof(LineRenderer))]
    public class DrawCircle : MonoBehaviour
    {
        [Range(0, 50)] public int segments = 50;
        public float xradius = 5;
        public float yradius = 5;

        private LineRenderer line;

        void Start()
        {
            UpdateCircle();
        }

        public void UpdateCircle(float radius)
        {
            xradius = radius;
            yradius = radius;

            UpdateCircle();
        }

        public void UpdateCircle()
        {
            line = gameObject.GetComponent<LineRenderer>();

            //line.SetVertexCount(segments + 1);
            line.positionCount = segments + 1;
            line.useWorldSpace = false;
            CreatePoints();
        }

        void CreatePoints()
        {
            float x;
            float z;

            float angle = 20f;

            for (int i = 0; i < (segments + 1); i++)
            {
                x = Mathf.Sin(Mathf.Deg2Rad * angle) * xradius;
                z = Mathf.Cos(Mathf.Deg2Rad * angle) * yradius;

                line.SetPosition(i, new Vector3(x, 0, z));

                angle += (360f / segments);
            }
        }
    }
}