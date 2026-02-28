using UnityEngine;

namespace InfiniStacker.Audio
{
    public sealed class AudioSettingsService : MonoBehaviour
    {
        [SerializeField] private bool sfxEnabled;
        [SerializeField] private bool musicEnabled;

        public bool SfxEnabled => sfxEnabled;
        public bool MusicEnabled => musicEnabled;

        public void SetSfxEnabled(bool enabled)
        {
            sfxEnabled = enabled;
        }

        public void SetMusicEnabled(bool enabled)
        {
            musicEnabled = enabled;
        }

        public void PlayOneShot(AudioSource source, AudioClip clip, float volume = 1f)
        {
            if (!sfxEnabled || source == null || clip == null)
            {
                return;
            }

            source.PlayOneShot(clip, Mathf.Clamp01(volume));
        }
    }
}
