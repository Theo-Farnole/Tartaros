namespace Game.Utils
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public partial class SetTransformToRaycastHit : MonoBehaviour
    {
        [SerializeField] private Transform _transformToMove;

        [SerializeField] private float _maxDistance = Mathf.Infinity;

        void Update()
        {
            Ray ray = new Ray(transform.position, transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, _maxDistance))
            {
                _transformToMove.position = hit.point;
            }
            else
            {
                _transformToMove.position = ray.origin + ray.direction * _maxDistance;
            }
        }
    }

#if UNITY_EDITOR
    public partial class SetTransformToRaycastHit : MonoBehaviour
    {
        void OnDrawGizmosSelected()
        {
            if (Mathf.Abs(_maxDistance) == Mathf.Infinity)
                return;

            Vector3 destination = transform.position + transform.forward * _maxDistance;
            Vector3 origin = transform.position;

            Gizmos.color = Color.green;

            Gizmos.DrawLine(origin, destination);
            Gizmos.DrawWireSphere(destination, 0.1f);
        }
    }
#endif
}
