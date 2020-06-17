using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundGroup : MonoBehaviour
    {
        [SerializeField] private AudioClip[] _audioClips;

        private AudioSource _audioSource;

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

        private AudioClip GetRandomAudioClip()
        {
            int index = Random.Range(0, _audioClips.Length);
            return _audioClips[index];
        }
    }
}
