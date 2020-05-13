using LeonidasLegacy.IA.Action;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitiesGoToCenter : MonoBehaviour
{
    [SerializeField] private Vector3 _targetPointOffset;
    [SerializeField] private Color _targetColor;

    public Vector3 TargetPoint { get => _targetPointOffset + transform.position; }

    void Start()
    {
        StartCoroutine(EntitiesGoToTargetPoint());        
    }

    void OnDrawGizmos()
    {
        Gizmos.color = _targetColor;
        Gizmos.DrawWireSphere(TargetPoint, 0.3f);
    }

    IEnumerator EntitiesGoToTargetPoint()
    {
        Entity[] entities = FindObjectsOfType<Entity>();

        int length = entities.Length;
        for (int i = 0; i < length; i++)
        {
            var command = new ActionMoveToPositionAggressively(entities[i], TargetPoint);
            entities[i].SetAction(command);

            if (i % 10 == 0)
                yield return new WaitForEndOfFrame();
        }

        Debug.LogFormat("Stress test for {0} entities.", entities.Length);
    }
}
