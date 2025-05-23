using UnityEngine;

namespace DefaultNamespace
{
    
    [CreateAssetMenu(menuName = "Create SoundCollection", fileName = "SoundCollection", order = 0)]
    public class SoundCollection : ScriptableObject
    {
        [SerializeField] private AudioClip[] _audioClips;
        [SerializeField] private float _minPitch = 1.0f;
        [SerializeField] private float _maxPitch = 1.0f;
        [SerializeField] private float _minVolume = 1.0f;
        [SerializeField] private float _maxVolume = 1.0f;

        public AudioClip GetRandomClip()
        {
            if (_audioClips == null || _audioClips.Length == 0) return null;
            return _audioClips[Random.Range(0, _audioClips.Length)];
        }

        public (AudioClip clip, float pitch, float volume) GetRandomClipWithVariation()
        {
            if (_audioClips == null || _audioClips.Length == 0) return (null, 0f, 0f);
            AudioClip clip = _audioClips[Random.Range(0, _audioClips.Length)];
            float pitch = Random.Range(_minPitch, _maxPitch);
            float volume = Random.Range(_minVolume, _maxVolume);
            return (clip, pitch, volume);
        }
    }
}