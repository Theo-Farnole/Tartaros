namespace Game.Audio
{    
    using UnityEngine;
    using DG.Tweening;

    [RequireComponent(typeof(AudioSource))]
    public class SoundGroup : MonoBehaviour
    {
        [SerializeField] private AudioClip[] _audioClips = new AudioClip[0];

        private AudioSource _audioSource = null;
        private float _originalVolume = 1;

        void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void PlayOneShotRandomClip()
        {
            _audioSource.PlayOneShot(GetRandomAudioClip());
        }

        public void PlayRandomClip()
        {
            if (_audioSource.isPlaying)
                Debug.LogWarning("Call PlayRandomClip while audio source is not stopped can make weird noise.");

            _audioSource.Stop();
            _audioSource.clip = GetRandomAudioClip();
            _audioSource.Play();
        }

        public void SmoothMute(float smooothDuration)
        {
            _originalVolume = _audioSource.volume;

            _audioSource.DOFade(0, smooothDuration);
        }

        public void SmoothAmplify(float smoothDuration)
        {
            _audioSource.DOFade(_originalVolume, smoothDuration);
        }

        private AudioClip GetRandomAudioClip()
        {
            int index = Random.Range(0, _audioClips.Length);
            return _audioClips[index];
        }
    }
}
