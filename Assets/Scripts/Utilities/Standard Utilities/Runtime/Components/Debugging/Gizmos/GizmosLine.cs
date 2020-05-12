using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lortedo.Utilities
{
    /// <author>
    /// https://answers.unity.com/questions/1139985/gizmosdrawline-thickens.html
    /// </author>
    public class GizmosLine : MonoBehaviour
    {
        public Vector3 from;
        public Vector3 to;

        public float width = 1f;

        void OnDrawGizmos()
        {
            int count = Mathf.CeilToInt(width); // how many lines are needed.
            if (count == 1)
                Gizmos.DrawLine(from, to);
            else
            {
                Camera c = Camera.main;

                if (c == null)
                {
                    Debug.LogError("Camera.current is null");

                    return;
                }

                Vector3 v1 = (to - from).normalized; // line direction
                Vector3 v2 = (c.transform.position - from).normalized; // direction to camera
                Vector3 n = Vector3.Cross(v1, v2); // normal vector
                for (uint i = 0; i < count; i++)
                {
                    Vector3 o = n * width * ((float)i / (count - 1) - 0.5f);
                    Gizmos.DrawLine(from + o, to + o);
                }
            }
        }
    }
}
