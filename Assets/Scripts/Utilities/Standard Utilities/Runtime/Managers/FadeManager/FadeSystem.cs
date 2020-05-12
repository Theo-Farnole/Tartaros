using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lortedo.Utilities.Managers
{
    /// <summary>
    /// Manage fade with Canvas.
    /// </summary>
    public static class FadeSystem
    {
        #region Fields
        private static Image _fadeImage;
        #endregion

        #region Methods
        static void CreateFadeImage()
        {
            if (_fadeImage != null)
                return;

            GameObject init = new GameObject
            {
                name = "Fader"
            };

            Canvas myCanvas = init.AddComponent<Canvas>();
            myCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            init.AddComponent<CanvasGroup>();
            _fadeImage = init.AddComponent<Image>();

            GameObject.DontDestroyOnLoad(init);
        }

        #region Fade
        public static void Fade(FadeType fadeType, float duration)
        {
            Fade(fadeType, duration, Color.black);
        }

        public static void Fade(FadeType fadeType, float duration, Color color)
        {
            CreateFadeImage();

            _fadeImage.StopAllCoroutines();

            _fadeImage.color = color;
            _fadeImage.Fade(fadeType, duration / 2);
        }
        #endregion

        #region Fade Blink
        /// <summary>
        /// Fading in, then fading out.
        /// </summary>
        /// <param name="duration">Total duration of blink. Thus, fading in duration's half of the blink duration.</param>
        public static void FadeBlink(float duration)
        {
            FadeBlink(duration, Color.black);
        }

        /// <summary>
        /// Fading in, then fading out.
        /// </summary>
        /// <param name="duration">Total duration of blink. Thus, fading in duration's half of the blink duration.</param>
        public static void FadeBlink(float duration, Color color)
        {
            Fade(FadeType.FadeIn, duration / 2, color);
            _fadeImage.ExecuteAfterTime(duration / 2, () => Fade(FadeType.FadeOut, duration / 2, color));
        }
        #endregion
        #endregion
    }
}
