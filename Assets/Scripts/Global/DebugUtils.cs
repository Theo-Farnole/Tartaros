using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class DebugUtils : MonoBehaviour
{
    #region Fields
    [Header("Activation Settings")]
    [SerializeField] private bool _activeOnStart = false;
    [SerializeField] private KeyCode _toggleKey = KeyCode.F2;
    [Header("Give Resources Settings")]
    [SerializeField] private KeyCode _giveKey = KeyCode.F3;
    [SerializeField] private ResourcesWrapper _giveResources = new ResourcesWrapper(100, 100, 100);

    private bool _active = true;
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Awake()
    {
        _active = _activeOnStart;

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

        if (_active)
        {
            if (Input.GetKeyDown(_giveKey))
            {
                GameManager.Instance.Resources += _giveResources;
            }
        }
    }

    void OnGUI()
    {
        if (!_active)
            return;

        //DrawGameManagerState();
        DrawSelectedGroups();
    }
    #endregion

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
        for (int i = 0; i < selectedGroupsArray.Count; i++)
        {
            o.AppendLine(selectedGroupsArray[i].selectedEntities.Count + " " + selectedGroupsArray[i].entityType);
        }

        Rect rect = new Rect(15, 45, 150, 400);
        GUI.Label(rect, o.ToString());
    }
    #endregion
}
