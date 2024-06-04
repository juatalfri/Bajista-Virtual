using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

public class MainMenu : MonoBehaviour
{
    static string midiFile = null;
    static List<int> midiNotesDescription = new List<int>();
    static List<int> midiNotesTimestamp = new List<int>();
    public TextMeshProUGUI errorText;
    public TextMeshProUGUI midiFileText;
    public void StartAnimation()
    {
        if (midiFile == null)
        {
            errorText.text = "Selecciona un archivo MIDI para continuar";
        }
        else
        {
            // Llamar al modulo c++ para que inicialize el audioDeviceManager
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public void SelectMidi()
    {
        int numTracks = selectMidiFileWrapper();
        // Llamar al modulo c++ para que abra el fileExplorer y obtener el midi seleccionado,
        // y despues llamar a initMidi para obtener el numero de tracks.

        // Una vez hecho eso debe mostrar un pop up que permita seleccionar los tracks a tocar,
        // para despues llamar a proccess midi y obtener las dos listas necesarias para la animacion y el vector midiMessagges
    }

    public void Quit()
    {
        Application.Quit();
    }

    [DllImport("MidiProcessorSynth.dll")]
    public static extern int selectMidiFileWrapper();

}
