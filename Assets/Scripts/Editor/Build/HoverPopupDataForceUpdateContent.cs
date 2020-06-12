using Game.UI.HoverPopup;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>
/// This script get all HoverPopupData in project, then update content on build.
/// 
/// THE PROBLEM:
/// A EntityData can be linked to a HoverPopupData.
/// This linking updates the content of the HoverPopupData, only by OnValidate callback.
/// But, OnValidate is called only if HoverPopupData is modified (eg. when linking is made).
/// So it's not called when modifying EntityData.
/// 
/// THE SOLUTION:
/// To make sure that HoverPopupData has the right content from EntityData, 
/// we call 'UpdateContent()' method on every HoverPopupData in the project 
/// before a build.
/// </summary>
public class HoverPopupDataForceUpdateContent : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        ForceUpdateHoverPopupData();
    }

    [MenuItem("Tartaros/Force update HoverPopupData", priority = 2)]
    private static void ForceUpdateHoverPopupData()
    {
        string[] guids = AssetDatabase.FindAssets("t:" + nameof(HoverPopupData));

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            HoverPopupData hoverPopupData = AssetDatabase.LoadAssetAtPath<HoverPopupData>(assetPath);

            int beforeUpdateContentHash = hoverPopupData.GetHashCode();
            hoverPopupData.UpdateContent();

            bool doHoverPopupHasBeenUpdated = beforeUpdateContentHash != hoverPopupData.GetHashCode();

            if (doHoverPopupHasBeenUpdated)
            {
                Debug.LogFormat("HoverHoverPopupData : Update content of {0}", hoverPopupData.name);
            }
        }
    }
}
