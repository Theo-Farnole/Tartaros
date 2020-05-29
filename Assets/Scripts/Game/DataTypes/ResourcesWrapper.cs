using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization;

public enum Resource
{
    Food,
    Wood,
    Stone
}

[System.Serializable]
public struct ResourcesWrapper
{
    public readonly static ResourcesWrapper Zero = new ResourcesWrapper(0, 0, 0);

    [PositiveValueOnly] public int food;

    [PositiveValueOnly] public int wood;

    [FormerlySerializedAs("gold")]
    [PositiveValueOnly] public int stone;

    public ResourcesWrapper(int food = 0, int wood = 0, int stone = 0)
    {
        this.food = food;
        this.wood = wood;
        this.stone = stone;
    }

    public int GetResource(Resource resource)
    {
        switch (resource)
        {
            case Resource.Food:
                return food;

            case Resource.Wood:
                return wood;

            case Resource.Stone:
                return stone;
        }

        return -1;
    }

    public void AddResource(Resource resource, int amount)
    {
        switch (resource)
        {
            case Resource.Food:
                food += amount;
                break;

            case Resource.Wood:
                wood += amount;
                break;

            case Resource.Stone:
                stone += amount;
                break;
        }
    }

    public bool HasEnoughResources(ResourcesWrapper b)
    {
        return (this >= b);
    }

    public string ToString(bool cropEmptyResources)
    {
        string o = string.Empty;

        if (cropEmptyResources)
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (food != 0)
            {
                stringBuilder.AppendFormat("food: {0}", food);
            }

            if (wood != 0)
            {
                if (food != 0)
                    stringBuilder.Append(",");

                stringBuilder.AppendFormat("wood: {0}", wood);
            }

            if (stone != 0)
            {
                if (food != 0 && wood != 0 || food != 0 && wood == 0)
                    stringBuilder.Append(",");

                stringBuilder.AppendFormat("stone: {0}", stone);
            }

            o = stringBuilder.ToString();

            if (o == string.Empty)
                o = "RESOURCES EMPTY";
        }
        else
        {
            o = ToString();
        }

        return o;
    }

    public override string ToString()
    {

        return string.Format("food: {0}, wood: {1}, stone: {2} ", food, wood, stone);
    }

    #region Operator
    public static ResourcesWrapper operator -(ResourcesWrapper a, ResourcesWrapper b) =>
        new ResourcesWrapper(a.food - b.food, a.wood - b.wood, a.stone - b.stone);

    public static ResourcesWrapper operator +(ResourcesWrapper a, ResourcesWrapper b) =>
        new ResourcesWrapper(a.food + b.food, a.wood + b.wood, a.stone + b.stone);

    public static ResourcesWrapper operator *(ResourcesWrapper a, int b) =>
        new ResourcesWrapper(a.food * b, a.wood * b, a.stone * b);

    public static ResourcesWrapper operator +(ResourcesWrapper a, int b) =>
        new ResourcesWrapper(a.food + b, a.wood + b, a.stone + b);

    public static bool operator <(ResourcesWrapper a, ResourcesWrapper b) =>
        a.food < b.food && a.wood < b.wood && a.stone < b.stone;

    public static bool operator >(ResourcesWrapper a, ResourcesWrapper b) =>
        a.food > b.food && a.wood > b.wood && a.stone > b.stone;

    public static bool operator >=(ResourcesWrapper a, ResourcesWrapper b) =>
        a.food >= b.food && a.wood >= b.wood && a.stone >= b.stone;

    public static bool operator <=(ResourcesWrapper a, ResourcesWrapper b) =>
        a.food <= b.food || a.wood <= b.wood || a.stone <= b.stone;

    public static bool operator ==(ResourcesWrapper a, ResourcesWrapper b) =>
        a.food == b.food && a.wood == b.wood && a.stone == b.stone;

    public static bool operator !=(ResourcesWrapper a, ResourcesWrapper b) =>
        !(a == b);

    public override bool Equals(object obj)
    {
        if (!(obj is ResourcesWrapper))
        {
            return false;
        }

        var wrapper = (ResourcesWrapper)obj;
        return food == wrapper.food &&
               wood == wrapper.wood &&
               stone == wrapper.stone;
    }

    public override int GetHashCode()
    {
        var hashCode = -1301703148;
        hashCode += hashCode * -1521134295 + food.GetHashCode();
        hashCode += hashCode * -1521134295 + wood.GetHashCode();
        hashCode += hashCode * -1521134295 + stone.GetHashCode();
        return hashCode;
    }
    #endregion
}