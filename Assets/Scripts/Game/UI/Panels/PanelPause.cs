namespace Game.UI
{
    using Game.GameState;

    public class PanelPause : AbstractPanel
    {
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
    }
}
