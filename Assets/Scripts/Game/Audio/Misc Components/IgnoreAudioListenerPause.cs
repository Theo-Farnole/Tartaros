namespace Game.Audio
{
    using UnityEngine;

    [RequireComponent(typeof(AudioSource))]
    public class IgnoreAudioListenerPause : MonoBehaviour
    {
        [SerializeField] private bool _ignoreListenerPause = true;
        [SerializeField] private bool _destroyScripAfterIgnoring = true;

        void Start()
        {
            GetComponent<AudioSource>().ignoreListenerPause = _ignoreListenerPause;

            if (_destroyScripAfterIgnoring)
            {
                Destroy(this);
            }
        }
    }
}
