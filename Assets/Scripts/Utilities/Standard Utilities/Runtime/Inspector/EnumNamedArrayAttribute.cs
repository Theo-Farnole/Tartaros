using System;
using UnityEngine;

/// <author>
/// https://answers.unity.com/questions/1589226/showing-an-array-with-enum-as-keys-in-the-property.html
/// </author>

namespace Lortedo.Utilities.Inspector
{
    public class EnumNamedArrayAttribute : PropertyAttribute
    {
        public string[] names;

        public EnumNamedArrayAttribute(Type enumType)
        {
            this.names = Enum.GetNames(enumType);
        }

        public EnumNamedArrayAttribute(string[] names)
        {
            this.names = names;
        }
    }
}