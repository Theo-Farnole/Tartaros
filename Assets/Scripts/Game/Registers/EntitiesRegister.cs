using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Registers
{
    public static class EntitiesRegister
    {
        public static RegisterData GetRegisterData(EntityType type)
        {
            UnitType? unitType = type.IsUnitType();

            if (unitType != null)
            {
                return UnitsRegister.Instance.GetItem((UnitType)unitType);
            }
            else
            {
                BuildingType? buildingType = type.IsBuildingType();

                if (buildingType != null)
                {
                    return BuildingsRegister.Instance.GetItem((BuildingType)buildingType);
                }
            }

            return null;
        }
    }
}
