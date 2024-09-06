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

    bool midiSelected = false;
    float selectedTrack = 0;
    string mediaPath = Application.streamingAssetsPath;

    private ExtensionFilter[] extensions = new[]
    {
        new ExtensionFilter("Archivos Midi", "mid")
    };

    #endregion

    #region Funciones del menu

    public void selectMidi()
    {
        string[] newMidi = StandaloneFileBrowser.OpenFilePanel("Abrir Archivo Midi", mediaPath + "\\Midi samples",
            extensions, false);
        File.WriteAllText(mediaPath + "\\Midipath.txt", newMidi[0]);
        midiSelected = true;
    }

    public void selectTrackDropdown()
    {
        selectedTrack = float.Parse(trackDropdown.options[trackDropdown.value].text);
        midiAudioMixerGroup.audioMixer.SetFloat("PistaBajo", selectedTrack / 20f);
    }

    public void StartAnimation()
    {
        if (selectedTrack != 0 && midiSelected)
        {
            midiAudioSource.Play();

            midiAudioMixerGroup.audioMixer.SetFloat("FinalizarMidi", 0);
            midiAudioMixerGroup.audioMixer.SetFloat("IniciarMidi", 1);
 
            MidiManager.midiManagerInstance.configAnimation = true;
            //Invoke("restartMidi", 0.5f);
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    #endregion

    #region Funciones de control del midi

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

    #endregion
}
