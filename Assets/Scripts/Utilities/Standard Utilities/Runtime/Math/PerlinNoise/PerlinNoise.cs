using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lortedo.Utilities.PerlinNoise
{
    [System.Serializable]
    public class PerlinNoise
    {
        #region Fields
        [SerializeField, Range(0, 99999)] private float _seed = 1;
        [Space]
        [SerializeField, Range(1, 8)] private int _octaves = 1;
        [SerializeField, Range(1, 5)] private float _frequency = 4;
        [SerializeField] private float _amplitude = 128;
        #endregion

        public float OctavePerlin(float x, float y)
        {
            float total = 0;
            float maxValue = 0;  // Used for normalizing result to 0.0 - 1.0

            var frequency = _frequency;

            for (int i = 0; i < _octaves; i++)
            {
                total += Mathf.PerlinNoise(_seed + x * frequency, _seed + y * frequency) * _amplitude;

                maxValue += _amplitude;

                frequency *= 2;
            }

            return total / maxValue;
        }
    }
}
