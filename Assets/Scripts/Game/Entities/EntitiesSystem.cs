using System;

public static class EntitiesSystem
{
    public static readonly int STARTING_INDEX_UNIT = 0;
    public static readonly int STARTING_INDEX_BUILDING = 1000;
}

public enum EntityType
{
    // units
    Alexios = 0,
    Kassandra = 1,
    Xerxes = 2,

    // building
    House = 1000,
    Barracks = 1001,
    Farm = 1002
}

public enum Building
{
    House = EntityType.House,
    Barracks = EntityType.Barracks,
    Farm = EntityType.Farm
}

public enum Unit
{
    Alexios = EntityType.Alexios,
    Kassandra = EntityType.Kassandra,
    Xerxes = EntityType.Xerxes,
}

public static class EntityTypeExtension
{
    public static Owner GetOwner(this EntityType e)
    {
        switch (e)
        {
            case EntityType.Alexios:
            case EntityType.Kassandra:
                return Owner.Sparta;

            case EntityType.Xerxes:
                return Owner.Persian;

            case EntityType.House:
            case EntityType.Barracks:
                return Owner.Sparta;
        }

        return Owner.Nature;
    }

    public static Unit? IsUnitType(this EntityType e)
    {
        foreach (Unit item in Enum.GetValues(typeof(Unit)))
        {
            if ((int)item == (int)e)
            {
                return item;
            }
        }

        return null;
    }

    public static Building? IsBuildingType(this EntityType e)
    {
        foreach (Building item in Enum.GetValues(typeof(Building)))
        {
            if ((int)item == (int)e)
            {
                return item;
            }
        }

        return null;
    }
}