using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lortedo.Utilities.PerlinNoise.Filters
{
    [System.Serializable]
    public abstract class Filter
    {
        public abstract float ApplyFilter(float currentValue);
    }

    [System.Serializable]
    public class ClampFilter : Filter
    {
        [SerializeField] private bool _enableClamping = false;
        [Space]
        [SerializeField, Range(0, 1)] private float _maxPercent = 0.2f;
        [Space]
        [SerializeField] private float _height = -2;

        public override float ApplyFilter(float value)
        {
            if (!_enableClamping)
                return value;

            bool isWater = value <= _maxPercent;

            if (isWater)
            {
                value = _height;
            }

            return value;
        }
    }

    [System.Serializable]
    public class RandomSurfaceFilter : Filter
    {
        [SerializeField] private bool _enableSurfaceFilter = true;
        [Space]
        [SerializeField] private float _heightMaxGap = 0.3f;

        // we are using System random for MultiThread usage
        private System.Random _random = new System.Random();

        public override float ApplyFilter(float value)
        {
            if (!_enableSurfaceFilter)
                return value;

            float rand = (float)_random.NextDouble(); // 0.0f to 1.0f
            rand *= 2; // 0.f to 2f
            rand -= 1f; // -1f to 1f;

            value += _heightMaxGap * rand;

            return value;
        }
    }

    [System.Serializable]
    public class IslandFilter : Filter
    {
        [SerializeField] private bool _enableIslandEffect = false;
        [Space]
        [SerializeField] private float _islandRadius = 20;
        [SerializeField, Range(0, 3)] private float _islandMultipler = 1;


        public float ApplyFilter(float value, Vector3 noisePosition, Vector3 islandCenter)
        {
            if (!_enableIslandEffect)
                return value;

            float distFromCenter = Vector3.Distance(noisePosition, islandCenter);
            float y = 1 - (distFromCenter / _islandRadius);

            return value + y * _islandMultipler;
        }

        public override float ApplyFilter(float currentValue)
        {
            return ApplyFilter(currentValue, Vector3.zero, Vector3.zero);
        }
    }
}