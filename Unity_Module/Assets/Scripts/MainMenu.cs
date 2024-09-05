using TMPro;
using UnityEngine;
using System.Runtime.InteropServices;
using SFB;
using System.IO;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    #region Definicion de variables

    [SerializeField] AudioMixerGroup midiAudioMixerGroup;
    [SerializeField] TMP_Dropdown trackDropdown;
    [SerializeField] Toggle trackMode;
    [SerializeField] AudioSource midiAudioSource;

    float selectedTrack = 0;
    string mediaPath = Application.streamingAssetsPath;

    private ExtensionFilter[] extensions = new[]
    {
        new ExtensionFilter("Archivos Midi", "mid")
    };

    private void Awake()
    {
        DontDestroyOnLoad(midiAudioSource);
    }

    #endregion

    #region Funciones del menu

    public void selectMidi()
    {
        string[] newMidi = StandaloneFileBrowser.OpenFilePanel("Abrir Archivo Midi", mediaPath + "\\Midi samples",
            extensions, false);
        midiAudioMixerGroup.audioMixer.SetFloat("PistaBajo", 0f);
        File.WriteAllText(mediaPath + "\\Midipath.txt", newMidi[0]);
        playMidi();
    }

    public void selectTrackDropdown()
    {
        selectedTrack = float.Parse(trackDropdown.options[trackDropdown.value].text);
    }

    public void StartAnimation()
    {
        if (selectedTrack != 0)
        {
            midiAudioMixerGroup.audioMixer.SetFloat("PistaBajo", selectedTrack / 20f);
            MidiManager.midiManagerInstance.configAnimation = true;
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    #endregion

    #region Funciones de control del midi
    public void playMidi()
    {
        midiAudioSource.Play();
        midiAudioMixerGroup.audioMixer.SetFloat("Volumen", 0);

        midiAudioMixerGroup.audioMixer.SetFloat("CambiarMidi", 1);
        Invoke("restartMidi", 0.5f);
    }

    void restartMidi()
    {
        midiAudioMixerGroup.audioMixer.SetFloat("CambiarMidi", 0);
    }

    public void selectAllTracks()
    {
        if (trackMode.isOn)
        {
            midiAudioMixerGroup.audioMixer.SetFloat("CancionCompleta", 1);
        }
        else
        {
            midiAudioMixerGroup.audioMixer.SetFloat("CancionCompleta", 0);
        }
    }

    //public void rewindMidi()
    //{
    //    midiAudioMixerGroup.audioMixer.SetFloat("RetrocederAvanzar", 0.3333f);
    //    Invoke("disableRewindMidi", 0.5f);
    //}
    //public void forwardMidi()
    //{
    //    midiAudioMixerGroup.audioMixer.SetFloat("RetrocederAvanzar", 0.7777f);
    //    Invoke("disableForwardMidi", 0.5f);
    //}
    //void disableForwardMidi()
    //{
    //    midiAudioMixerGroup.audioMixer.SetFloat("RetrocederAvanzar", 0.5f);
    //}
    //void disableRewindMidi()
    //{
    //    midiAudioMixerGroup.audioMixer.SetFloat("RetrocederAvanzar", 0.5f);
    //}

    #endregion

    #region Funciones PInvoke 

    [DllImport("audioplugin_MidiProcessorSynth_Module.dll")]
    private static extern int getNumTracks();
    #endregion
}
