using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Registers
{
    public static class EntitiesRegister
    {
        public static RegisterData GetRegisterData(EntityType type)
        {
            Unit? unitType = type.IsUnitType();

            if (unitType != null)
            {
                return UnitsRegister.Instance.GetItem((Unit)unitType);
            }
            else
            {
                Building? buildingType = type.IsBuildingType();

                if (buildingType != null)
                {
                    return BuildingsRegister.Instance.GetItem((Building)buildingType);
                }
            }

            return null;
        }
    }
}
