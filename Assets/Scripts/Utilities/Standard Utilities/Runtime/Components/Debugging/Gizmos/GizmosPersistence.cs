using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lortedo.Utilities
{
    public static class GizmosPersistence
    {
        /// <summary>
        /// Draw a line that will be alive for X seconds.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="lifetime">Duration of the line in seconds</param>
        public static void DrawPersistentLine(Vector3 from, Vector2 to, float lifetime = 3f)
        {
            var gizmos = new GameObject().AddComponent<GizmosLine>();
            gizmos.from = from;
            gizmos.to = to;

            GameObject.Destroy(gizmos.gameObject, lifetime);
        }
    }
}
