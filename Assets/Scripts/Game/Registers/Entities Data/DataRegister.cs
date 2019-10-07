using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataRegister
{
    public static EntityData GetData(EntityType entity)
    {
        Unit? unitType = entity.IsUnitType();

        if (unitType != null)
        {
            return UnitsDataRegister.Instance.GetItem((Unit)unitType);
        }
        else
        {
            Building? buildingType = entity.IsBuildingType();

            if (buildingType != null)
            {
                return BuildingDataRegister.Instance.GetItem((Building)buildingType);
            }
        }

        return null;
    }
}
