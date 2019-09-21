using System;

public enum Entity
{
    // units
    Alexios = 0,
    Kassandra = 1,

    // building
    House = 1000,
    Barracks = 1001
}

public enum Building
{
    House = Entity.House,
    Barracks = Entity.Barracks
}

public enum Unit
{
    Alexios = Entity.Alexios,
    Kassandra = Entity.Kassandra
}

public static class EntityExtension
{
    public static Unit? IsUnitType(this Entity e)
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

    public static Building? IsBuildingType(this Entity e)
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