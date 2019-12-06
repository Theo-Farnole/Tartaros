using System;
using System.Linq;

public static class EntitiesSystem
{
    public const int STARTING_INDEX_UNIT = 0;
    public const int STARTING_INDEX_BUILDING = 1000;
}

public enum EntityType
{
    // units
    Alexios = EntitiesSystem.STARTING_INDEX_UNIT + 0,
    Kassandra = EntitiesSystem.STARTING_INDEX_UNIT + 1,
    Xerxes = EntitiesSystem.STARTING_INDEX_UNIT + 2,

    // building
    House = EntitiesSystem.STARTING_INDEX_BUILDING,
    Barracks = EntitiesSystem.STARTING_INDEX_BUILDING + 1,
    Farm = EntitiesSystem.STARTING_INDEX_BUILDING + 2
}

public enum BuildingType
{
    House = EntityType.House,
    Barracks = EntityType.Barracks,
    Farm = EntityType.Farm
}

public enum UnitType
{
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

    public static UnitType? IsUnitType(this EntityType e)
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

    public static BuildingType? IsBuildingType(this EntityType e)
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
}