using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;
using SFB;
using UnityEngine.UI;
using System.IO;
using System;
using UnityEngine.Audio;
using System.Threading;
using TMPro.EditorUtilities;

public class MainMenu : MonoBehaviour
{
    #region Definicion de variables

    [SerializeField] TextMeshProUGUI selectedTrackLabel;
    [SerializeField] AudioSource midiAudioSource;
    [SerializeField] AudioMixerGroup midiAudioMixerGroup;
    [SerializeField] TMP_Dropdown trackDropdown;

    int[] noteNumberArray;
    double[] timestampsArray;

    int numTracks;
    float selectedTrack = 0; 
    string mediaPath = Application.dataPath + "\\Media";
    bool isPlaying = false;

    private ExtensionFilter[] extensions = new[]
    {
        new ExtensionFilter("Archivos Midi", "mid")
    };

    #endregion

    #region Funciones del menu

    public void StartAnimation()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        if (selectedTrack > numTracks & numTracks > 0 )
        {
            selectedTrack = numTracks - 1;
        }
        midiAudioMixerGroup.audioMixer.SetFloat("PistaBajo", selectedTrack / 20f);
        isPlaying = true;
    }

    public void PauseResume()
    {
        if (isPlaying == true)
        {
            midiAudioMixerGroup.audioMixer.SetFloat("PausarMidi", 1);
            isPlaying = false;
        }
        else
        {
            midiAudioMixerGroup.audioMixer.SetFloat("PausarMidi", 0);
            isPlaying = true;
        }

    }

    public void Quit()
    {
        Application.Quit();
    }

    public void selectMidi()
    {
        string[] newMidi = StandaloneFileBrowser.OpenFilePanel("Abrir Archivo Midi", mediaPath + "\\Midi samples", extensions, false);
        selectedTrackLabel.text = newMidi[0];
        midiAudioMixerGroup.audioMixer.SetFloat("PistaBajo", 0f);
        File.WriteAllText(mediaPath + "\\Midipath.txt", newMidi[0]);
        playMidi();
        numTracks = getNumTracks();
        //fillDropdown();
    }
    public void fillDropdown()
    {
        if (trackDropdown.options.Count > 0)
        {
            trackDropdown.options.Clear();
        }

        Thread.Sleep(200);
        numTracks = getNumTracks();
        for (int i = 1; i < numTracks; i++)
        {
            trackDropdown.options.Add(new TMP_Dropdown.OptionData() { text = i.ToString() });
        }
        trackDropdown.RefreshShownValue();
    }

    public void selectTrackDropdown()
    {

        selectedTrack = float.Parse(trackDropdown.options[trackDropdown.value].text);
    }

    #endregion

    #region Funciones de control del midi
    public void playMidi()
    {
        midiAudioSource.Play();

        midiAudioMixerGroup.audioMixer.SetFloat("CambiarMidi", 1);
        Invoke("restartMidi", 0.5f);
        //midiAudioMixerGroup.audioMixer.SetFloat("PistaBajo", selectedTrack / 20f);

    }

    void restartMidi()
    {
        midiAudioMixerGroup.audioMixer.SetFloat("CambiarMidi", 0);
    }
    #endregion


    #region Funciones PInvoke 

    void loadMidiData()
    {
        // Obtener los arrays de los datos
        IntPtr noteNumberPtr;
        int noteNumberSize;
        IntPtr timestampsPtr;
        int timestampsSize;
        getNotesAndTimestaps(out noteNumberPtr, out noteNumberSize, out timestampsPtr, out timestampsSize);

        // Convertir los IntPtr a arrays
        noteNumberArray = new int[noteNumberSize];
        timestampsArray = new double[timestampsSize];
        Marshal.Copy(noteNumberPtr, noteNumberArray, 0, noteNumberSize);
        Marshal.Copy(timestampsPtr, timestampsArray, 0, timestampsSize);
    }

    [DllImport("audioplugin_MidiProcessorSynth_Module.dll")]
    private static extern int getNumTracks();

    [DllImport("audioplugin_MidiProcessorSynth_Module.dll")]
    private static extern void getNotesAndTimestaps(out IntPtr noteNumberArray, out int noteNumberSize, out IntPtr timestampsArray, out int timestampsSize);

    #endregion
}
