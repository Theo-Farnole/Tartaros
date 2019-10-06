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
        [SerializeField] private SpriteRenderer _fogOfWarVision = null;
        [Header("Coverable Settings")]
        [SerializeField] private FOWManager.Coverable _coverable;

        public float ViewRadius { get => _viewRadius; }

        void Start()
        {
            switch (GetComponent<Entity>().Owner)
            {
                case Owner.Sparta:
                    FOWManager.Instance.AddViewer(this);
                    _fogOfWarVision.gameObject.SetActive(true);
                    _fogOfWarVision.transform.localScale = Vector3.one * _viewRadius * 2;
                    break;

                case Owner.Persian:
                    FOWManager.Instance.AddCoverable(_coverable);
                    _fogOfWarVision.enabled = false;
                    break;

                case Owner.Nature:
                    break;
            }
        }
    }
}
