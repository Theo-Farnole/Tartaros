using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class AbstractPanel : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private bool _hideOnStart;

        protected virtual void Awake()
        {
            // sometime developer disable UI by deactivating the canvas
            // however, for performance reason, we just disable the Canvas component
            // so, we assert that our gameobject isn't disabled
            _canvas.gameObject.SetActive(true);
        }

        protected virtual void Start()
        {
            if (_hideOnStart) Hide();
            else Show();
        }

        public bool IsPanelShowing() => _canvas.enabled;

        public void Show()
        {
            if (!_canvas.enabled)
                OnShow();

            _canvas.enabled = true;
        }

        public void Hide()
        {
            if (_canvas.enabled)
                OnHide();

            _canvas.enabled = false;
        }

        public virtual void OnShow() { }
        public virtual void OnHide() { }
    }
}
