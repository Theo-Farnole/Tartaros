namespace Game.Inputs
{
    using UnityEngine;

    public delegate void DoubleClickDelegate();

    public class DoubleClickDetector : MonoBehaviour
    {
        [SerializeField] private float _doubleClickTime = 0.5f;
        private float lastClickTime = -10f;

        public event DoubleClickDelegate OnDoubleClick;


        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                float timeDelta = Time.time - lastClickTime;

                if (timeDelta < _doubleClickTime)
                {
                    OnDoubleClick?.Invoke();
                    lastClickTime = 0;
                }
                else
                {
                    lastClickTime = Time.time;
                }
            }
        }
    }
}