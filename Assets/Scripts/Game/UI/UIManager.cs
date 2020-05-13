using Game.Selection;
using Lortedo.Utilities.Inspector;
using Lortedo.Utilities.Managers;
using Lortedo.Utilities.Pattern;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI.Game;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : AbstractUIManager
{
    #region Fields
    [SerializeField] private PanelGameInformation _panelGameInformation;
    [SerializeField] private PanelSelection _panelSelection;
    [SerializeField] private PanelConstruction _panelConstruction;
    #endregion

    #region Properties
    public PanelGameInformation PanelGameInformation { get => _panelGameInformation; }
    public PanelSelection PanelSelection { get => _panelSelection; }
    public PanelConstruction PanelConstruction { get => _panelConstruction; }

    protected override Type[] OwnedPanels
    {
        get => new Type[] { typeof(PanelGameInformation), typeof(PanelSelection), typeof(PanelConstruction) };
    }
    #endregion

    #region Methods
    protected override void Start()
    {
        base.Start();

        _panelConstruction.Show();
        _panelSelection.Hide();
        _panelGameInformation.Show();

        SelectionManager.OnSelectionUpdated += ManagePanelDisplay;
    }

    void ManagePanelDisplay(SelectionManager.SelectionGroup[] selectedGroups, int highlightGroupIndex)
    {
        if (selectedGroups.Length == 0)
        {
            _panelConstruction.Show();
            _panelSelection.Hide();
        }
        else
        {
            _panelSelection.Show();
            _panelConstruction.Hide();
        }
    }
    #endregion
}

