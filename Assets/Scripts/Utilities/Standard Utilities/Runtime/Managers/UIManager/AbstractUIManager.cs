using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Lortedo.Utilities.Pattern;

namespace Lortedo.Utilities.Managers
{
    // can't have an ABSTRACT class w/ mono behaviour inheritance
    public class AbstractUIManager : MonoBehaviour
    {
        #region Fields
        private Dictionary<Type, Panel> _panels = new Dictionary<Type, Panel>();

        private Panel _currentPanel;
        #endregion

        #region Properties
        private Panel CurrentPanel
        {
            get => _currentPanel;

            set
            {
                _currentPanel?.OnStateExit();
                _currentPanel?.Root.SetActive(false);

                _currentPanel = value;

                _currentPanel?.Root.SetActive(true);
                _currentPanel?.OnStateEnter();
            }
        }

        protected virtual Type[] OwnedPanels
        {
            get => null;
        }
        #endregion

        #region Methods
        #region Mono Callbacks
        protected virtual void Awake()
        {
            InitializePanels();
        }

        protected virtual void OnValidate()
        {
            foreach (Type type in OwnedPanels)
            {
                GetPanel(type)?.OnValidate();
            }
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Create a dictionnary from type & _panel serialized fields
        /// </summary>
        void InitializePanels()
        {
            foreach (Type type in OwnedPanels)
            {
                var panel = GetPanel(type);

                if (panel == null)
                    continue;

                panel.Initialize(this);
                panel.Root.SetActive(false);

                _panels.Add(type, panel);
            }
        }

        Panel GetPanel(Type type)
        {
            // get panel field info from type
            FieldInfo panelFieldInfo = GetType().GetFields(BindingFlags.Instance |
                          BindingFlags.Static |
                          BindingFlags.NonPublic |
                          BindingFlags.Public).FirstOrDefault(f => f.FieldType == type);

            if (panelFieldInfo == null)
            {
                Debug.LogWarning("Can't find " + type + " panel on " + this + ".");
                return null;
            }

            Panel panel = (Panel)panelFieldInfo.GetValue(this);
            return panel;
        }
        #endregion

        #region Public methods
        public void DisplayPanel<TPanel>() where TPanel : Panel
        {
            CurrentPanel = _panels[typeof(TPanel)];
        }
        #endregion
        #endregion
    }
}
