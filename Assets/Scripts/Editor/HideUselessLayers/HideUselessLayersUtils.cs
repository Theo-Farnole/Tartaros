using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[InitializeOnLoad]
public static class ForceLayersWindow
{
    private static readonly int _wantedVisibleMask = ~LayerMask.GetMask("FogOfWar", "UI", "MiniMap");

    private const string DIALOG_POPUP_TITLE = "Le super popup de Tartaros";
    private const string DIALOG_POPUP_MESSAGE = "C'est un peu long a expliquer, mais clique sur '" + DIALOG_POPUP_OK + "' stp.\nBisous, Théo.";
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
            if (Tools.visibleLayers != _wantedVisibleMask)
            {
                DisplayPopup();
            }
        }
    }

    [MenuItem("Tartaros/Advanced - DON'T TOUCH/Display 'useless less' popup")]
    private static void DisplayPopup()
    {


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
