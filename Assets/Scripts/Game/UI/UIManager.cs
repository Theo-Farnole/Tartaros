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


public class UIManager : MonoBehaviour
{
    #region Fields
    private const string debugLogHeader = "UIManager : ";

    [SerializeField] private PanelGameResources _panelGameInformation;
    [SerializeField] private PanelSelection _panelSelection;
    [SerializeField] private PanelConstruction _panelConstruction;
    [SerializeField] private PanelGameOver _panelGameOver;
    [SerializeField] private PanelVictory _panelVictory;
    #endregion


    #region Methods
    void Start()
    {
        _panelGameInformation.gameObject.SetActive(true);
        _panelSelection.gameObject.SetActive(true);
        _panelConstruction.gameObject.SetActive(true);
        _panelGameOver.gameObject.SetActive(true);
        _panelVictory.gameObject.SetActive(true);


        _panelConstruction.Show();
        _panelSelection.Hide();
        _panelGameInformation.Show();
        _panelGameOver.Hide();
        _panelVictory.Hide();
    }

    void OnEnable()
    {
        GameManager.OnGameOver += GameManager_OnGameOver;
        GameManager.OnVictory += GameManager_OnVictory;
        SelectionManager.OnSelectionUpdated += ManagePanelDisplay;

    }

    void OnDisable()
    {
        GameManager.OnGameOver -= GameManager_OnGameOver;
        GameManager.OnVictory -= GameManager_OnVictory;
        SelectionManager.OnSelectionUpdated -= ManagePanelDisplay;
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
        Assert.IsFalse(_panelVictory.IsPanelShowing(), debugLogHeader + "panel victory is show, but we just received a 'GameOver' from GameManager.");

        _panelGameOver.Show();
        _panelGameInformation.Hide();
        _panelSelection.Hide();
        _panelConstruction.Hide();
    }

    private void GameManager_OnVictory(GameManager gameManager)
    {
        Assert.IsFalse(_panelGameOver.IsPanelShowing(), debugLogHeader + "panel gameover is show, but we just received a 'Victory' from GameManager.");

        _panelVictory.Show();
        _panelGameInformation.Hide();
        _panelSelection.Hide();
        _panelConstruction.Hide();
    }
    #endregion
}

