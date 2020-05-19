using Game.Selection;
using Lortedo.Utilities.Inspector;
using Lortedo.Utilities.Managers;
using Lortedo.Utilities.Pattern;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Game.UI;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;


public class UIManager : AbstractUIManager
{
    #region Fields
    private const string debugLogHeader = "UIManager : ";

    [SerializeField] private PanelGameInformation _panelGameInformation;
    [SerializeField] private PanelSelection _panelSelection;
    [SerializeField] private PanelConstruction _panelConstruction;
    [SerializeField] private PanelGameOver _panelGameOver;
    [SerializeField] private PanelVictory _panelVictory;
    #endregion

    #region Properties
    protected override Type[] OwnedPanels
    {
        get => new Type[] {
            typeof(PanelGameInformation),
            typeof(PanelSelection),
            typeof(PanelConstruction),
            typeof(PanelGameOver),
            typeof(PanelVictory)
        };
    }
    #endregion

    #region Methods
    protected override void Start()
    {
        base.Start();

        _panelConstruction.Show();
        _panelSelection.Hide();
        _panelGameInformation.Show();
        _panelGameOver.Hide();
        _panelVictory.Hide();        
    }

    protected override void OnEnable()
    {
        GameManager.OnGameOver += GameManager_OnGameOver;
        GameManager.OnVictory += GameManager_OnVictory;
        SelectionManager.OnSelectionUpdated += ManagePanelDisplay;

        base.OnEnable();
    }

    protected override void OnDisable()
    {
        GameManager.OnGameOver -= GameManager_OnGameOver;
        GameManager.OnVictory -= GameManager_OnVictory;
        SelectionManager.OnSelectionUpdated -= ManagePanelDisplay;

        base.OnDisable();
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

    private void GameManager_OnGameOver(GameManager gameManager)
    {
        Assert.IsFalse(_panelVictory.IsPanelEnable(), debugLogHeader + "panel victory is show, but we just received a 'GameOver' from GameManager.");

        _panelGameOver.Show();
        _panelGameInformation.Hide();
        _panelSelection.Hide();
        _panelConstruction.Hide();
    }

    private void GameManager_OnVictory(GameManager gameManager)
    {
        Assert.IsFalse(_panelGameOver.IsPanelEnable(), debugLogHeader + "panel gameover is show, but we just received a 'Victory' from GameManager.");

        _panelVictory.Show();
        _panelGameInformation.Hide();
        _panelSelection.Hide();
        _panelConstruction.Hide();
    }
    #endregion
}

