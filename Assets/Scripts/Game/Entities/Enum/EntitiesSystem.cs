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
    Alexios = EntitiesSystem.STARTING_INDEX_UNIT + 0,
    Kassandra = EntitiesSystem.STARTING_INDEX_UNIT + 1,
    Xerxes = EntitiesSystem.STARTING_INDEX_UNIT + 2,

    // building
    House = EntitiesSystem.STARTING_INDEX_BUILDING + 0,
    Barracks = EntitiesSystem.STARTING_INDEX_BUILDING + 1,
    Farm = EntitiesSystem.STARTING_INDEX_BUILDING + 2
}

public enum BuildingType
{
    None  = EntityType.None,
    House = EntityType.House,
    Barracks = EntityType.Barracks,
    Farm = EntityType.Farm
}

public enum UnitType
{
    None = EntityType.None,
    Alexios = EntityType.Alexios,
    Kassandra = EntityType.Kassandra,
    Xerxes = EntityType.Xerxes,
}

public enum Team
{
    Sparta,
    Persian, 
    Nature
}

public static class EntityTypeExtension
{
    public static Team GetOwner(this EntityType e)
    {
        switch (e)
        {
            case EntityType.Alexios:
            case EntityType.Kassandra:
                return Team.Sparta;

            case EntityType.Xerxes:
                return Team.Persian;

            case EntityType.House:
            case EntityType.Barracks:
                return Team.Sparta;
        }

        return Team.Nature;
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

    #region old - TO DELETE
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

    #region new - TO KEEP
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
    #endregion
}