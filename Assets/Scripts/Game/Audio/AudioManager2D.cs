using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Audio
{
    /// <summary>
    /// This script hold reference to SoundGroup for each Sound2D value. 
    /// Look at the url below to understand difference betwene PlayOneShot and Play method.
    /// https://gamedevbeginner.com/how-to-play-audio-in-unity-with-examples/
    /// </summary>
    public class AudioManager2D : SerializedMonoBehaviour
    {
        // PERFORMANCE NOTE:
        // Dictionary<int, TValue> is more performant then Dictionary<enum, TValue> 
        // However, we select the 'enum' as key method for readability reasons.
        [SerializeField] private Dictionary<Sound2D, SoundGroup> _sounds = new Dictionary<Sound2D, SoundGroup>();

        public void PlayOneShotRandomClip(Sound2D sound2D)
            => _sounds[sound2D].PlayOneShotRandomClip();

        public void PlayRandomClip(Sound2D sound2D)
            => _sounds[sound2D].PlayRandomClip();
    }

    public enum Sound2D
    {
        WaveStart = 1,
        WaveEnd = 2,
        NotEnoughResources = 3,
        UnitCreated = 4, // we don't play sound in function of unit created
        OrderGiven = 5, // we don't play sound in function of order given
        SuccessfulBuilding = 6,
        OnSelection = 7,
        OnButtonClick = 8,
        OnSetAnchorPosition = 9
    }
}