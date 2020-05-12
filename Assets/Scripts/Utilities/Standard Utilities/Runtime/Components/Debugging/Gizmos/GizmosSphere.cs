using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lortedo.Utilities
{
    public class GizmosSphere : MonoBehaviour
    {
        [SerializeField] private Color _color = Color.yellow;
        [SerializeField] private float _radius = 0.3f;        

        void OnDrawGizmos()
        {
            // Draw a yellow sphere at the transform's position
            Gizmos.color = _color;
            Gizmos.DrawSphere(transform.position, _radius);
        }
    }
}
