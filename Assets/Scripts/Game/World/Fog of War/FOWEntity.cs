using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FogOfWar
{
    [RequireComponent(typeof(Entity))]
    public class FOWEntity : MonoBehaviour
    {
        [Header("Viewer Settings")]
        [SerializeField] private float _viewRadius = 3;
        [Header("Coverable Settings")]
        [SerializeField] private FOWManager.Coverable _coverable;

        public float ViewRadius { get => _viewRadius; }

        void Start()
        {
            switch (GetComponent<Entity>().Owner)
            {
                case Owner.Sparta:
                    FOWManager.Instance.AddViewer(this);
                    break;

                case Owner.Persian:
                    FOWManager.Instance.AddCoverable(_coverable);
                    break;

                case Owner.Nature:
                    break;
            }
        }
    }
}
