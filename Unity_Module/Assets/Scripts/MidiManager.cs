using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using TMPro;

public class MidiManager : MonoBehaviour
{
    #region Definicion de variables


    public static MidiManager midiManagerInstance;
    public double currentTime = 0;
    public bool configAnimation = false;

    [SerializeField] AudioMixerGroup midiAudioMixerGroup;
    [SerializeField] FretboardNotes[] fretboardNotes;
    
    public int[] notes;
    double[] timestamps;
    bool isPlaying = false;

    public int currentHandPosition = 2;
    public int predominantlyHand;
    public List<int> notesHand4 = new List<int> { 41, 42, 43, 44, 46, 47, 48, 49, 51, 52, 53, 54, 56, 57, 58, 59 };
    public List<int> notesHand1 = new List<int> { 37, 38, 39, 40, 42, 43, 44, 45, 47, 48, 49, 50, 52, 53, 54, 55 };
    public List<int> notesHand2 = new List<int> { 33, 34, 35, 36, 38, 39, 40, 41, 43, 44, 45, 46, 48, 49, 50, 51 };
    public List<int> notesHand3 = new List<int> { 29, 30, 31, 32, 34, 35, 36, 37, 39, 40, 41 ,42, 44, 45, 46, 47 };

    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject animationMenu;
    [SerializeField] TextMeshProUGUI pauseResumeLabel;

    #endregion

    #region Funciones MidiManager

    public void PrepareNotes()
    {
        countNotesPerHand();

        foreach (var fretboardNote in fretboardNotes)
        {
            fretboardNote.SetTimeStamps(notes, timestamps);

        }
        StartSong();
    }

    public void countNotesPerHand()
    {
        int countHand1 = 0;
        int countHand2 = 0;
        int countHand3 = 0;

        foreach (int note in notes)
        {
            if (notesHand1.Contains(note))
            {
                countHand1++;
            }
            else if (notesHand2.Contains(note))
            {
                countHand2++;
            }
            else
            {
                countHand3++;
            }
        }
        predominantlyHand = Math.Max(Math.Max(countHand1, countHand2), countHand3);
        if (predominantlyHand == countHand1)
        {
            predominantlyHand = 1;
        }
        else if (predominantlyHand == countHand2)
        {
            predominantlyHand = 2;
        }
        else
        {
            predominantlyHand = 3;
        }
    }

    public void StartSong()
    {
        mainMenu.SetActive(false);
        animationMenu.SetActive(true);
        CameraManager.cameraManagerInstance.EnableCamera2();

        midiAudioMixerGroup.audioMixer.SetFloat("IniciarMidi", 1);
        isPlaying = true;
    }

    public void PauseResume()
    {
        if (isPlaying)
        {
            pauseResumeLabel.text = "Reanudar";
            midiAudioMixerGroup.audioMixer.SetFloat("Volumen", -80);
            midiAudioMixerGroup.audioMixer.SetFloat("PausarMidi", 1);
            isPlaying = false;
        }
        else
        {
            pauseResumeLabel.text = "Pausar";
            midiAudioMixerGroup.audioMixer.SetFloat("Volumen", 0);
            midiAudioMixerGroup.audioMixer.SetFloat("PausarMidi", 0);
            isPlaying = true;
        }
    }

    public void SalirAnimacion()
    {
        midiAudioMixerGroup.audioMixer.SetFloat("Volumen", -80);
        midiAudioMixerGroup.audioMixer.SetFloat("IniciarMidi", 0);

        SceneManager.LoadScene("Scene");
    }

    #endregion

    #region Funciones PInvoke 

    public void loadMidiData()
    {
        IntPtr noteNumberPtr;
        int noteNumberSize;
        IntPtr timestampsPtr;
        int timestampsSize;
        // Obtener los arrays de los datos
        getNotesAndTimestaps(out noteNumberPtr, out noteNumberSize, out timestampsPtr, out timestampsSize);

        // Convertir los IntPtr a arrays
        notes = new int[noteNumberSize];
        timestamps = new double[timestampsSize];
        Marshal.Copy(noteNumberPtr, notes, 0, noteNumberSize);
        Marshal.Copy(timestampsPtr, timestamps, 0, timestampsSize);
    }

    [DllImport("audioplugin_MidiProcessorSynth_Module.dll")]
    private static extern void getNotesAndTimestaps(out IntPtr noteNumberArray,
        out int noteNumberSize, out IntPtr timestampsArray, out int timestampsSize);

    #endregion

    void Start()
    {
        midiManagerInstance = this;
    }

    private void Update()
    {
        if (configAnimation && notes.Length == 0)
        {
            loadMidiData();
        }
        else if (configAnimation && notes.Length > 0)
        {
            configAnimation = false;
            PrepareNotes();
        }
        
        if (isPlaying)
        {
            currentTime += Time.deltaTime;
        }
    }
}