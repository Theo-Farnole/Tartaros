using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[InitializeOnLoad]
public static class ForceLayersWindow
{
    private static readonly int _wantedVisibleMask = ~LayerMask.GetMask("FogOfWar", "UI", "MiniMap");

    private const string DIALOG_POPUP_TITLE = "TARTAROS - Déactiver les layers inutiles";
    private const string DIALOG_POPUP_MESSAGE = "Est-ce que tu veux cacher des layers qui pourrait être te gêner (ça changera juste ta vue, rien au projet). Par exemple, actuellement l'UI qui s'affiche dans la fenêtre scène alors que tu en as pas besoin.";
    private const string DIALOG_POPUP_OK = "Avec plaisir!";
    private const string DIALOG_POPUP_CANCEL = "Non merci, mais t'es le boss";

    private const string DIALOG_FEEDBACK_TITLE = "TARTAROS";
    private const string DIALOG_FEEDBACK_MESSAGE = "Les layers ont bien été déactivés.";
    private const string DIALOG_FEEDBACK_OK = "OK";

    private const bool DISPLAY_DIALOG_POPUP_ONSTART = true;

    #region Internal - DON'T TOUCH
    private const string DEBUG_LOG_HEADER = "<color=cyan>Layers Window : </color>";

    static ForceLayersWindow()
    {
        if (DISPLAY_DIALOG_POPUP_ONSTART)
        {
            DisplayPopup();
        }
    }

    private static void DisplayPopup()
    {
        // don't display popup if visible layers are the same of layerMask
        if (Tools.visibleLayers == _wantedVisibleMask)
            return;

        bool setVisibleLayers = EditorUtility.DisplayDialog(DIALOG_POPUP_TITLE, DIALOG_POPUP_MESSAGE, DIALOG_POPUP_OK, DIALOG_POPUP_CANCEL);

        if (setVisibleLayers)
        {
            Tools.visibleLayers = _wantedVisibleMask;
            Debug.Log(DEBUG_LOG_HEADER + " Layers has been successfully changed.");
        }
    }

    [MenuItem("Tartaros/Hide useless layers")]
    private static void HideUselessLayers()
    {
        Tools.visibleLayers = _wantedVisibleMask;

        EditorUtility.DisplayDialog(DIALOG_FEEDBACK_TITLE, DIALOG_FEEDBACK_MESSAGE, DIALOG_FEEDBACK_OK);
    }
    #endregion
}
