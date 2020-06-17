namespace Game.Audio
{
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Button))]
    public class PlaySoundOnButtonClick : MonoBehaviour
    {
        [SerializeField] private Sound2D _soundToPlayOnClick = Sound2D.OnButtonClick;

        private Button _button;
        private static AudioManager2D _audioManager2D;

        private Button Button
        {
            get
            {
                if (_button == null)
                    _button = GetComponent<Button>();

                return _button;
            }
        }

        private static AudioManager2D AudioManager
        {
            get
            {
                if (_audioManager2D == null)
                    _audioManager2D = FindObjectOfType<AudioManager2D>();

                return _audioManager2D;
            }
        }

        void OnEnable()
        {
            Button.onClick.AddListener(() => AudioManager.PlayOneShotRandomClip(Sound2D.OnButtonClick));
        }
    }
}
