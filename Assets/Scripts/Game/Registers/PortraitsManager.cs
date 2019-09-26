using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PortraitsManager
{
    public static Sprite GetPortrait(EntityType ent)
    {
        var entUnitType = ent.IsUnitType();

        if (entUnitType != null)
        {
            return UnitsPortraitsRegister.Instance?.GetItem((Unit)entUnitType);
        }
        else
        {
            var entBuildingType = ent.IsBuildingType();

            if (entBuildingType != null)
            {
                return BuildingsPortraitsRegister.Instance?.GetItem((Building)entBuildingType);
            }
        }

        return null;
    }
}
