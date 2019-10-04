using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PortraitsManager
{
    public static Sprite GetPortrait(EntityType entity)
    {
        Unit? unitType = entity.IsUnitType();

        if (unitType != null)
        {
            return UnitsPortraitsRegister.Instance.GetItem((Unit)unitType);
        }
        else
        {
            Building? buildingType = entity.IsBuildingType();

            if (buildingType != null)
            {
                return BuildingsPortraitsRegister.Instance.GetItem((Building)buildingType);
            }
        }

        return null;
    }
}
