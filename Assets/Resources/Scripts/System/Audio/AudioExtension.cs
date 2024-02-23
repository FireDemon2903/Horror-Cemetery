using UnityEngine;
using UnityEngine.Audio;

public class AudioExtension : MonoBehaviour
{
    public AudioMixer audioMixer { get; private set; }

    private void Start()
    {
        audioMixer = Resources.Load<AudioMixer>("Audio/PlayerAudioMixer");
    }

    public AudioMixerGroup[] AllMixerGroups
    {
        get
        {
            return audioMixer.FindMatchingGroups(string.Empty);
            ;
        }
    }
}