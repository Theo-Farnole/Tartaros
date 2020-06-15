namespace Game.UI
{
    using Game.GameState;
    using UnityEngine;

    public class PanelPause : AbstractPanel
    {
        [SerializeField] private KeyCode _resumeKeyCode = KeyCode.Escape;

        void Update()
        {
            HandleResumeInput();
        }

        void OnEnable()
        {
            GameState.OnGamePaused += GameState_OnGamePaused;
            GameState.OnGameResumed += GameState_OnGameResumed;
        }

        void OnDisable()
        {
            GameState.OnGamePaused -= GameState_OnGamePaused;
            GameState.OnGameResumed -= GameState_OnGameResumed;
        }

        private void GameState_OnGameResumed() => Hide();

        private void GameState_OnGamePaused() => Show();

        private void HandleResumeInput()
        {
            if (Input.GetKeyDown(_resumeKeyCode))
            {
                GameState.ToggleGameState();
            }
        }
    }
}
