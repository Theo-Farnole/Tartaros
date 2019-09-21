using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    #region Fields
    [SerializeField] private Image _selectedPortrait;
    #endregion

    #region Methods
    public void SetSelectedPortrait(Entity ent)
    {
        Sprite sprite = null;

        var entUnitType = ent.IsUnitType();
        if (entUnitType != null)
        {
            sprite = UnitsPortraitsRegister.Instance.GetItem((Unit)entUnitType);
        }
        else
        {
            var entBuildingType = ent.IsBuildingType();

            if (entBuildingType != null)
            {
                sprite = BuildingsPortraitsRegister.Instance.GetItem((Building)entBuildingType);
            }
        }

        _selectedPortrait.sprite = sprite;
    }

    public void ResetSelectedPortrait()
    {
        _selectedPortrait.sprite = null;
    }
    #endregion
}
