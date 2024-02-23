using UnityEngine;
using UnityEngine.Audio;

public class MixLevels : MonoBehaviour
{
    AudioMixer audioMixer;
    private void Start() { audioMixer = Resources.Load<AudioMixer>("Audio/PlayerAudioMixer"); }

    public void SetMasterLv(float lv) { audioMixer.SetFloat("MasterVol", lv); }
    public void SetReaderLv(float lv) { audioMixer.SetFloat("ReaderVol", lv); }
    public void SetMusicLv(float lv) { audioMixer.SetFloat("MusicVol", lv); }
    public void SetSFXLv(float lv) { audioMixer.SetFloat("SFXVol", lv); }

    public void ClearVol(string param) { audioMixer.ClearFloat(param); }

}
