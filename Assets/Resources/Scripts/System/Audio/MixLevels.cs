using UnityEngine;
using UnityEngine.Audio;

public class MixLevels : MonoBehaviour
{
    AudioMixer audioMixer;
    private void Start() { audioMixer = Resources.Load<AudioMixer>("Audio/PlayerAudioMixer"); }

    public void SetMasterLvl(float lvl) { audioMixer.SetFloat("MasterVol", lvl); }
    public void SetReaderLvl(float lvl) { audioMixer.SetFloat("ReaderVol", lvl); }
    public void SetMusicLvl(float lvl) { audioMixer.SetFloat("MusicVol", lvl); }
    public void SetSFXLvl(float lvl) { audioMixer.SetFloat("SFXVol", lvl); }

    public void ClearVol(string param) { audioMixer.ClearFloat(param); }

}
