namespace Game.GameState
{
    using UnityEngine;

    public delegate void GameStateDelegate();

    public static class GameState
    {
        public static event GameStateDelegate OnGamePaused;
        public static event GameStateDelegate OnGameResumed;

        /// <summary>
        /// This will set timeScale to zero and mute the game.
        /// If you to play sounds' interface, you can use 'AudioSource.ignoreListenerPause=true;'
        /// </summary>
        public static void PauseGame()
        {
            Time.timeScale = 0;
            AudioListener.pause = true;

            OnGamePaused?.Invoke();
        }

        public static void ResumeGame()
        {
            Time.timeScale = 1;
            AudioListener.pause = false;

            OnGameResumed?.Invoke();
        }

        public static void ToggleGameState()
        {
            if (IsGamePaused()) ResumeGame();
            else PauseGame();
        }

        public static void QuitGame()
        {
#if UNITY_EDITOR
            // Application.Quit() does not work in the editor so
            // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
            UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
        }

        public static bool IsGamePaused() => Time.timeScale == 0;
    }
}
