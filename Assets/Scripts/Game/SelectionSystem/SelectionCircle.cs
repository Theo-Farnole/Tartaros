using Game.Entities;
using Lortedo.Utilities.Inspector;
using Lortedo.Utilities.Pattern;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Selection
{

    [RequireComponent(typeof(SpriteRenderer))]
    public class SelectionCircle : MonoBehaviour
    {
        #region Fields
        [SerializeField] private SelectionCircleColorData _selectionCircleColor;

        private SpriteRenderer _spriteRenderer;
        #endregion

        #region Properties
        public SpriteRenderer SpriteRenderer
        {
            get
            {
                if (_spriteRenderer == null)
                    _spriteRenderer = GetComponent<SpriteRenderer>();

                return _spriteRenderer;
            }
        }
        #endregion

        #region Methods
        #region Public methods
        public void SetCircleColor(Team team)
        {
            SpriteRenderer.color = _selectionCircleColor.GetColor(team);
        }

        public void SetSize(string entityID) => SetSize(MainRegister.Instance.GetEntityData(entityID).GetRadius());

        public void SetSize(float size)
        {
            transform.localScale = Vector3.one * size;
        }
        #endregion
        #endregion
    }
}
