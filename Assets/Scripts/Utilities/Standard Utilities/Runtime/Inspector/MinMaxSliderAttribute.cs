using UnityEngine;



// from 
// https://github.com/GucioDevs/SimpleMinMaxSlider/blob/master/Assets/SimpleMinMaxSlider/Scripts/MinMaxSliderAttribute.cs
// author GucioDevs (thank you ^^)

namespace Lortedo.Utilities.Inspector
{
    public class MinMaxSliderAttribute : PropertyAttribute
    {

        public float min;
        public float max;

        public MinMaxSliderAttribute(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }
}