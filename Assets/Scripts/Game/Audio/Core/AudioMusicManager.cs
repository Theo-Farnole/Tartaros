namespace Game.Audio
{
    using UnityEngine;

    public enum MusicPhase
    {
        Construction = 1,
        Defend = 2
    }

    public class AudioMusicManager : MonoBehaviour
    {
        [SerializeField] private SoundGroup _constructionMusics = null;
        [SerializeField] private SoundGroup _defendMusics = null;
        [Space]
        [SerializeField] private float _fadeDuration = 1f;

        public void PlayMusic(MusicPhase musicPhase)
        {
            GetSoundsGroup(musicPhase).PlayRandomClip();

            // fade musics
            GetSoundsGroup(musicPhase).SmoothAmplify(_fadeDuration);
            GetOppositeSoundsGroup(musicPhase).SmoothMute(_fadeDuration);
        }

        /// We could remove this method by using a dictionary. 
        /// But I find it very overkill to use a dictionary for 2 values.
        /// 
        /// Moreover, using a dictionary means a no official way to serialize it, maybe leading to problems.        
        SoundGroup GetSoundsGroup(MusicPhase musicPhase)
        {
            switch (musicPhase)
            {
                case MusicPhase.Construction:
                    return _constructionMusics;

                case MusicPhase.Defend:
                    return _defendMusics;

                default:
                    throw new System.NotImplementedException("Music phase was " + musicPhase);
            }
        }

        SoundGroup GetOppositeSoundsGroup(MusicPhase musicPhase) => GetSoundsGroup(musicPhase.GetOppositeMusicPhase());
    }

    public static class MusicPhaseExtension
    {
        public static MusicPhase GetOppositeMusicPhase(this MusicPhase musicPhase)
        {
            switch (musicPhase)
            {
                case MusicPhase.Construction:
                    return MusicPhase.Defend;

                case MusicPhase.Defend:
                    return MusicPhase.Construction;

                default:
                    throw new System.NotImplementedException("Music phase was " + musicPhase);
            }
        }
    }
}