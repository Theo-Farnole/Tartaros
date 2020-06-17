namespace Game.Inputs
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Game.GameState;
    using Game.GameManagers;

    public class PauseToggleInputHandler : MonoBehaviour
    {
        #region Fields
        [SerializeField] private KeyCode _resumeKeyCode = KeyCode.Escape;

        private int _dontHandleInputAtFrameCount = -1;
        #endregion

        #region Methods
        #region MonoBehaviour Callbacks
        void LateUpdate()
        {
            HandleResumeInput();
        }

        void OnEnable()
        {
            GameManager.OnStopBuild += GameManager_OnStopBuild;
        }

        void OnDisable()
        {
            GameManager.OnStopBuild -= GameManager_OnStopBuild;
        }
        #endregion

        #region Private Methods
        private void GameManager_OnStopBuild(GameManager gameManager)
        {
            // next frame
            _dontHandleInputAtFrameCount = Time.frameCount;
            GameState.ResumeGame();
        }

        private void HandleResumeInput()
        {
            if (_dontHandleInputAtFrameCount == Time.frameCount)
                return;

            if (Input.GetKeyDown(_resumeKeyCode))
            {
                GameState.ToggleGameState();
            }
        }
        #endregion
        #endregion
    }
}
