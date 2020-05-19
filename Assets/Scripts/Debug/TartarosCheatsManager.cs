using Game.FogOfWar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TF.Cheats;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.Cheats
{
    public class TartarosCheatsManager : MonoBehaviour
    {
        #region Fields
        private static readonly int gui_buttonWidth = 200;
        private static readonly int gui_buttonHeight = 20;
        private static readonly int gui_buttonleftMargin = 40;
        private static readonly int gui_buttontopMargin = 10;
        private static readonly int gui_startPositionY = 120;

        private static readonly int gui_backgroundWidth = 240;
        private static readonly int gui_backgroundHeightOffset = 60;
        private static readonly string gui_labelMenu = "Cheat Menu";

        private static readonly KeyCode _openCheatsMenuKey = KeyCode.C;
        private static readonly KeyCode _additionalKey = KeyCode.LeftShift;


        private static readonly string debugLogHeader = "Tartaros Cheats : ";

        private ResourcesWrapper _gameManagerResourcesBeforeInfiniteResources = new ResourcesWrapper(-1, -1, -1);

        private bool _cheatsMenuOpen = false;
        #endregion

        #region Methods
        #region MonoBehaviour Callbacks
        void Start()
        {
            _gameManagerResourcesBeforeInfiniteResources = GameManager.Instance.Resources;
            ManageActivation_InfiniteResources();
        }

        void Update()
        {
            Process_Inputs_OpenCheatsMenu();
        }

        void OnEnable()
        {
            CheatsManager.OnCheatChanged += OnCheatChanged;
        }

        void OnDisable()
        {
            CheatsManager.OnCheatChanged -= OnCheatChanged;
        }

        void OnGUI()
        {
            DrawCheatsMenu();
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

                    if (key == CheatsKey.keyDisableFog)
                        ManageActive_FogOfWar();

                        break;
                default:
                    throw new NotImplementedException();
            }
        }
        #endregion

        #region Infinites Resources
        private static bool IsInfiniteResourcesActive() => CheatsManager.GetBool(CheatsKey.keyInfiniteResources);

        void Toggle_InfiniteResources()
        {
            bool cheatActive = IsInfiniteResourcesActive();
            CheatsManager.SetBool(CheatsKey.keyInfiniteResources, !cheatActive);
        }

        void ManageActivation_InfiniteResources()
        {
            var cheatActive = IsInfiniteResourcesActive();

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
        #endregion

        #region Skip waves
        void StartWave()
        {
            ExecutePrivateMethodWithReflection<WaveManager>("StartWave");
            return;
        }
        #endregion

        #region Disable Fog of War
        private static bool IsFogDisabled() => CheatsManager.GetBool(CheatsKey.keyDisableFog);

        void ManageActive_FogOfWar()
        {
            if (IsFogDisabled()) Disable_FogOfWar();
            else Active_FogOfWar();
        }

        void Active_FogOfWar()
        {
            ExecutePrivateMethodWithReflection<FOWManager>("ReactiveFOW");
        }

        void Disable_FogOfWar()
        {
            ExecutePrivateMethodWithReflection<FOWManager>("DisableFOW");
        }

        void Toggle_FogOfWar()
        {
            CheatsManager.ToggleBool(CheatsKey.keyDisableFog);
        }
        #endregion

        void ExecutePrivateMethodWithReflection<T>(string methodName) where T : UnityEngine.Object
        {
            string typeToString = typeof(T).ToString();
            T manager = FindObjectOfType<T>();

            if (manager == null)
            {
                Debug.LogErrorFormat(debugLogHeader + "There is no {0} manager. Aborting {1} cheat.", typeToString, methodName);
                return;
            }

            // We use reflection to get a private method.
            // Because we are only calling this method for a cheat,
            // we don't want to set as public.
            //
            // The more there is encapsulation, the better!
            var type = typeof(T);
            MethodInfo methodInfo = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.IsNotNull(methodInfo, debugLogHeader + "method " + methodName + " not found in manager " + typeToString + ".");

            methodInfo.Invoke(manager, null);
        }

        #region Menu Open/Close
        private void CloseCheatsMenu() => _cheatsMenuOpen = false;
        private void OpenCheatsMenu() => _cheatsMenuOpen = true;

        private void ToggleCheatsMenu()
        {
            _cheatsMenuOpen = !_cheatsMenuOpen;
        }

        private void Process_Inputs_OpenCheatsMenu()
        {
            if (Input.GetKeyDown(_openCheatsMenuKey) && Input.GetKey(_additionalKey))
            {
                ToggleCheatsMenu();
            }
        }
        #endregion

        #region GUI Draw
        private void DrawCheatsMenu()
        {
            if (!_cheatsMenuOpen)
                return;

            int buttonCount = 4;

            Draw_MenuBackground(buttonCount);

            Draw_InfiniteResources(0);
            Draw_StartWave(1);
            Draw_DisableFow(2);
            Draw_CloseCheatsMenu(buttonCount - 1);
        }

        private void Draw_MenuBackground(int buttonCount)
        {
            Rect rectBox = new Rect(
                20,
                80,
                gui_backgroundWidth,
                GetCheatsMenuHeight(buttonCount));

            GUI.Box(rectBox, gui_labelMenu);
        }

        #region Cheats buttons
        private void DrawGenericButton(int buttonIndex, string label, Action onclickAction)
        {
            Rect buttonRect = new Rect(
                gui_buttonleftMargin,
                gui_startPositionY + (gui_buttonHeight + gui_buttontopMargin) * buttonIndex,
                gui_buttonWidth,
                gui_buttonHeight);

            if (GUI.Button(buttonRect, label))
                onclickAction();
        }

        private void Draw_CloseCheatsMenu(int buttonIndex)
        {
            DrawGenericButton(buttonIndex, "Close this menu", CloseCheatsMenu);
        }

        private void Draw_InfiniteResources(int buttonIndex)
        {
            string label = "Infinite Resources - " + (IsInfiniteResourcesActive() ? "Disable" : "Active");

            DrawGenericButton(buttonIndex, label, Toggle_InfiniteResources);
        }

        private void Draw_StartWave(int buttonIndex)
        {
            DrawGenericButton(buttonIndex, "Start wave", StartWave);
        }

        private void Draw_DisableFow(int buttonIndex)
        {
            string label = IsFogDisabled() ? "Active" : "Disable" + " Fog of War";
            DrawGenericButton(buttonIndex, label, Toggle_FogOfWar);
        }
        #endregion

        private int GetCheatsMenuHeight(int buttonCount)
        {
            return (gui_buttonHeight + gui_buttontopMargin) * buttonCount + gui_backgroundHeightOffset;
        }
        #endregion
        #endregion
    }
}
