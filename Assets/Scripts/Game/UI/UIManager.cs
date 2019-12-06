using Lortedo.Utilities.Inspector;
using Lortedo.Utilities.Managers;
using Lortedo.Utilities.Pattern;
using Registers;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI.Game;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : AbstractUIManager<UIManager>
{
    #region Fields
    [SerializeField] private PanelGameInformation _panelGameInformation;
    [SerializeField] private PanelSelection _panelSelection;
    [SerializeField] private PanelConstruction _panelConstruction;
    #endregion

    #region Properties
    public PanelGameInformation PanelGameInformation { get => _panelGameInformation; set => _panelGameInformation = value; }
    public PanelSelection PanelSelection { get => _panelSelection; }
    public PanelConstruction PanelConstruction { get => _panelConstruction; }
    #endregion

    #region Methods
    void Start()
    {
        AddAlwaysDisplay<PanelGameInformation>();

        DisplayPanel<PanelConstruction>();
    }
    #endregion
}

