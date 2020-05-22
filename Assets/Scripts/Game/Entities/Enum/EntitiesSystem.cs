using System;
using System.Linq;

public static class EntitiesSystem
{
    public const int STARTING_INDEX_UNIT = 1;
    public const int STARTING_INDEX_BUILDING = 1000;
}

public enum EntityType
{
    None = 0,

    // units
    Melee_01 = EntitiesSystem.STARTING_INDEX_UNIT + 0,
    Range_01 = EntitiesSystem.STARTING_INDEX_UNIT + 1,

    // building
    House = EntitiesSystem.STARTING_INDEX_BUILDING + 0,
    Barracks = EntitiesSystem.STARTING_INDEX_BUILDING + 1,
    Farm = EntitiesSystem.STARTING_INDEX_BUILDING + 2,
    Temple = EntitiesSystem.STARTING_INDEX_BUILDING + 3,
    Wall = EntitiesSystem.STARTING_INDEX_BUILDING + 4
}

public enum BuildingType
{
    None  = EntityType.None,
    House = EntityType.House,
    Barracks = EntityType.Barracks,
    Farm = EntityType.Farm,
    Temple = EntityType.Temple,
    Wall = EntityType.Wall
}

public enum UnitType
{
    None = EntityType.None,
    Melee_01 = EntityType.Melee_01,
    Range_01 = EntityType.Range_01,
}

public enum Team
{
    Player,
    Enemy, 
    Nature
}

public static class EntityTypeExtension
{
    public static Team GetOwner(this EntityType e)
    {
        switch (e)
        {
            case EntityType.Melee_01:
            case EntityType.Range_01:
                return Team.Player;

            case EntityType.House:
            case EntityType.Barracks:
                return Team.Player;

            default:
                throw new NotImplementedException();
        }
    }

    public static bool IsUnitType(this EntityType e)
    {
        foreach (UnitType item in Enum.GetValues(typeof(UnitType)))
        {
            if ((int)item == (int)e)
            {
                return true;
            }
        }

        return false;
    }

    public static bool IsBuildingType(this EntityType e)
    {
        foreach (BuildingType item in Enum.GetValues(typeof(BuildingType)))
        {
            if ((int)item == (int)e)
            {
                return true;
            }
        }

        return false;
    }

    public static bool TryGetUnitType(this EntityType e, out UnitType unitType)
    {
        foreach (UnitType item in Enum.GetValues(typeof(UnitType)))
        {
            if ((int)item == (int)e)
            {
                unitType = item;
                return true;
            }
        }

        unitType = UnitType.None;
        return false;
    }

    public static bool TryGetBuildingType(this EntityType e, out BuildingType buildingType)
    {
        foreach (BuildingType item in Enum.GetValues(typeof(BuildingType)))
        {
            if ((int)item == (int)e)
            {
                buildingType = item;
                return true;
            }
        }

        buildingType = BuildingType.None;
        return false;
    }

    #region Obsolete
    [Obsolete("Please use TryGetUnitType instead.")]
    public static UnitType? GetUnitType(this EntityType e)
    {
        foreach (UnitType item in Enum.GetValues(typeof(UnitType)))
        {
            if ((int)item == (int)e)
            {
                return item;
            }
        }

        return null;
    }

    [Obsolete("Please use TryGetBuildingType instead.")]
    public static BuildingType? GetBuildingType(this EntityType e)
    {
        foreach (BuildingType item in Enum.GetValues(typeof(BuildingType)))
        {
            if ((int)item == (int)e)
            {
                return item;
            }
        }

        return null;
    }
    #endregion
}