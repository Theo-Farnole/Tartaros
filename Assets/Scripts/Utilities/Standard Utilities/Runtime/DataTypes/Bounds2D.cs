namespace Lortedo.Utilities.DataTypes
{
    using UnityEngine;

    [System.Serializable]
    public class Bounds2D
    {
        public float minimum;
        public float maximum;

        public Bounds2D(float min, float max)
        {
            minimum = min;
            maximum = max;
        }

        public Bounds2D(Vector2 limits)
        {
            minimum = limits.x;
            maximum = limits.y;
        }

        public bool WithinLimits(float value)
        {
            return (value >= minimum && value <= maximum);
        }
    }

    public static class Bounds2DExtension
    {
        public static Vector2 ToVector(this Bounds2D b)
        {
            return new Vector2(b.minimum, b.maximum);
        }

        public static bool WithinLimits(this Bounds2D b, float value)
        {
            return (value >= b.minimum && value <= b.maximum);
        }

        public static float Clamp(this Bounds2D b, float value)
        {
            return Mathf.Clamp(value, b.minimum, b.maximum);
        }
    }
}