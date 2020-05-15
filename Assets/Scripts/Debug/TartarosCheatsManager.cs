using System.Collections;
using System.Collections.Generic;
using TF.Cheats;
using UnityEngine;
using UnityEngine.Assertions;

namespace LeonidasLegacy.Cheats
{
    public class TartarosCheatsManager : MonoBehaviour
    {
        private readonly string debugLogHeader = "Tartaros Cheats : ";

        private ResourcesWrapper _gameManagerResourcesBeforeInfiniteResources = new ResourcesWrapper(-1, -1, -1);

        #region MonoBehaviour Callbacks
        void Start()
        {
            _gameManagerResourcesBeforeInfiniteResources = GameManager.Instance.Resources;
            ManageActivation_InfiniteResources();
        }

        void OnEnable()
        {
            CheatsManager.OnCheatChanged += OnCheatChanged;
        }

        void OnDisable()
        {
            CheatsManager.OnCheatChanged -= OnCheatChanged;
        }
        #endregion

        #region Events handlers
        void OnCheatChanged(string key, CheatType cheatType)
        {
            switch (cheatType)
            {
                case CheatType.Boolean:
                    
                    if (key == CheatsKey.keyInfiniteResources)                    
                        ManageActivation_InfiniteResources(); 

                    break;
                default:
                    throw new System.NotImplementedException();
            }
        }
        #endregion

        void ManageActivation_InfiniteResources()
        {
            var cheatActive = CheatsManager.GetBool(CheatsKey.keyInfiniteResources);

            if (cheatActive)
            {
                Active_InfiniteResources();
            }
            else
            {
                Disable_InfiniteResources();
            }
        }

        void Active_InfiniteResources()
        {
            _gameManagerResourcesBeforeInfiniteResources = GameManager.Instance.Resources;

            GameManager.Instance.Resources = new ResourcesWrapper(1, 1, 1) * 99999;
        }

        void Disable_InfiniteResources()
        {                        
            Assert.AreNotEqual(_gameManagerResourcesBeforeInfiniteResources, new ResourcesWrapper(-1, -1, -1),
                string.Format(debugLogHeader + "resources before infinite resources has not been setted! Please call Active_InfiniteResources before!"));

            GameManager.Instance.Resources = _gameManagerResourcesBeforeInfiniteResources;
        }
    }
}
