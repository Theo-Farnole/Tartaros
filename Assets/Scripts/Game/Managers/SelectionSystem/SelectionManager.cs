using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Group
{
    public Entity type;

    public List<GameObject> gameObjects = new List<GameObject>();

    public Group(Entity type, GameObject gameObject) : this(type, new List<GameObject>() { gameObject })
    { }

    public Group(Entity type, List<GameObject> gameObject)
    {
        this.type = type;

        gameObjects = gameObject;
    }
}

public class SelectionManager : Singleton<SelectionManager>
{
    #region Fields
    [SerializeField] private bool _displaySelectionOnUI = true;

    private List<Group> _selectedGroups = new List<Group>();
    #endregion

    #region Methods
    public void AddEntity(Entity type, GameObject gameObject)
    {
        for (int i = 0; i < _selectedGroups.Count; i++)
        {
            // if find a selectedGroup of same unit
            if (_selectedGroups[i].type == type)
            {
                // if gameObject isn't already in gameObjects list
                if (_selectedGroups[i].gameObjects.Contains(gameObject) == false)
                {
                    _selectedGroups[i].gameObjects.Add(gameObject);
                    UpdatePortrait();
                    return;
                }
                else
                {
                    Debug.Log("Selection Manager # " + gameObject + " can't be added because is already selected");
                    return;
                }
            }
        }

        // group with same unit doesn't exist, so we're creating one
        //Debug.Log("SelectionManager # Add " + gameObject + " of " + type);
        _selectedGroups.Add(new Group(type, gameObject));
        UpdatePortrait();
    }

    public void RemoveEntity(Entity type, GameObject gameObject)
    {
        for (int i = 0; i < _selectedGroups.Count; i++)
        {
            // if find a selectedGroup of same unit
            if (_selectedGroups[i].type == type)
            {
                //Debug.Log("SelectionManager # Remove " + gameObject + " of " + type);
                _selectedGroups[i].gameObjects.Remove(gameObject);

                if (_selectedGroups[i].gameObjects.Count == 0)
                {
                    _selectedGroups.RemoveAt(i);
                }

                UpdatePortrait();
                return;
            }
        }
    }

    public void UpdatePortrait()
    {
        if (_selectedGroups.Count > 0)
        {
            Entity firstItemType = _selectedGroups[0].type;
            UIManager.Instance.SetSelectedPortrait(firstItemType);
        }
        else
        {
            UIManager.Instance.ResetSelectedPortrait();
        }
    }

    public void OnGUI()
    {
        if (_displaySelectionOnUI == false)
            return;

        StringBuilder o = new StringBuilder();
        o.AppendLine("~ Selection ~");

        for (int i = 0; i < _selectedGroups.Count; i++)
        {
            o.AppendLine(_selectedGroups[i].gameObjects.Count + " " + _selectedGroups[i].type);
        }

        Rect rect = new Rect(15, 45, 150, 400);
        GUI.Label(rect, o.ToString());
    }
    #endregion
}
