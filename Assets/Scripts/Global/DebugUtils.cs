using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class DebugUtils : MonoBehaviour
{
    #region Fields
    [SerializeField] private KeyCode _toggleKey;
    private bool _active = true;
    #endregion

    #region Methods
    void Awake()
    {
#if !UNITY_EDITOR
        _active = false;
#endif
    }

    void Update()
    {
        if (Input.GetKeyDown(_toggleKey))
        {
            _active = !_active;
        }
    }

    void OnGUI()
    {
        if (!_active)
            return;

        DrawGameManagerState();
        DrawSelectedGroups();
    }

    private void DrawGameManagerState()
    {
        Rect rect = new Rect(15, 0, 150, 30);
        string label = "Current state: " + GameManager.Instance.State;
        GUIStyle style = new GUIStyle
        {
            fontSize = 30,
        };

        GUI.Label(rect, label, style);
    }

    private void DrawSelectedGroups()
    {

        StringBuilder o = new StringBuilder();
        o.AppendLine("~ Selection ~");

        var selectedGroupsArray = SelectionManager.Instance.SelectedGroups;
        for (int i = 0; i < selectedGroupsArray.Length; i++)
        {
            o.AppendLine(selectedGroupsArray[i].Value.Count + " " + selectedGroupsArray[i].Key.type);
        }

        Rect rect = new Rect(15, 45, 150, 400);
        GUI.Label(rect, o.ToString());
    }
    #endregion
}
