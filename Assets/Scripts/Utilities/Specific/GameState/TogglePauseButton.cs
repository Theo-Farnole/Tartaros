namespace Game.GameState
{
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Button))]
    public class TogglePauseButton : MonoBehaviour
    {
        private Button _button = null;

        public Button Button
        {
            get
            {
                if (_button == null)
                    _button = GetComponent<Button>();

                return _button;
            }
        }

        void OnEnable()
        {
            Button.onClick.AddListener(GameState.ToggleGameState);
        }

        void OnDisable()
        {
            Button.onClick.RemoveListener(GameState.ToggleGameState);            
        }
    }
}
