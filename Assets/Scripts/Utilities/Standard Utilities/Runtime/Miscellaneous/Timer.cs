using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lortedo.Utilities
{
    public class Timer
    {
        private Coroutine _coroutine;

        public Coroutine Coroutine { get => _coroutine; }

        /// <summary>
        /// Constructor of Timer
        /// </summary>
        /// <param name="ownerOfCoroutine">MonoBehaviour which will start the Coroutine</param>
        /// <param name="duration">Duration of the timer</param>
        /// <param name="task">float parameter is percent of progress of timer. Clamped between 0 and 1.</param>
        public Timer(MonoBehaviour ownerOfCoroutine, float duration, Action<float> task, Action taskOnEnd = null)
        {
            _coroutine = ownerOfCoroutine.StartCoroutine(TimerCoroutine(task, taskOnEnd, duration));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="task">Float paramater is completion between 0 - 1.</param>
        /// <param name="taskOnEnd"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        IEnumerator TimerCoroutine(Action<float> task, Action taskOnEnd, float duration)
        {
            float startingTime = Time.unscaledTime;
            float completion = 0;

            do
            {
                float deltaTime = Time.unscaledTime - startingTime;

                completion = Mathf.Lerp(0, 1, deltaTime / duration);

                if (float.IsNaN(completion) == false)
                {
                    task?.Invoke(completion);
                }

                yield return new WaitForEndOfFrame();
            } while (completion < 1);

            taskOnEnd?.Invoke();
        }
    }
}
