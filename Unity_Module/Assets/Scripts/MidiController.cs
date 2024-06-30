using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEditor.Audio;
using UnityEngine;
using UnityEngine.Audio;

public class MidiController : MonoBehaviour
{
    [SerializeField] AudioSource midiAudioSource;
    [SerializeField] AudioMixerGroup midiAudioMixerGroup;
    
    int[] noteNumberArray;
    double[] timestampsArray;
    int numTracks;
    float selectedTrack = 3;

    public void playMidi()
    {
        midiAudioSource.Play();
        midiAudioMixerGroup.audioMixer.SetFloat("CambiarMidi", 1);
        Invoke("restartMidi", 0.5f);

        numTracks = getNumTracks();
        midiAudioMixerGroup.audioMixer.SetFloat("PistaBajo", selectedTrack / 20f);
    }

    void restartMidi()
    {
        midiAudioMixerGroup.audioMixer.SetFloat("CambiarMidi", 0);
    }

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
