using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ArrayExtension
{
    /// <summary>
    /// Resize the array if needed w/ the size of count of enum values.
    /// </summary>
    public static T[] Resize<T>(this T[] array, Type enumType)
    {
        int enumLength = Enum.GetValues(enumType).Length;

        if (array.Length != enumLength)
        {
            Array.Resize<T>(ref array, enumLength);
        }

        return array;
    }

    public static bool IsIndexInsideBounds<T>(this T[] array, int index)
    {
        return index >= 0 && index < array.Length;
    }
}
