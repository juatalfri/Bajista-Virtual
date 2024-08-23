using TMPro;
using UnityEngine;
using System.Runtime.InteropServices;
using SFB;
using System.IO;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    #region Definicion de variables

    [SerializeField] TextMeshProUGUI selectedTrackLabel;
    [SerializeField] AudioMixerGroup midiAudioMixerGroup;
    [SerializeField] TMP_Dropdown trackDropdown;
    [SerializeField] AudioSource midiAudioSource;

    float selectedTrack = 0;
    string mediaPath = Application.streamingAssetsPath;
    //int numTracks;

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
        selectedTrackLabel.text = mediaPath;
        midiAudioMixerGroup.audioMixer.SetFloat("PistaBajo", 0f);
        File.WriteAllText(mediaPath + "\\Midipath.txt", newMidi[0]);
        playMidi();
        //fillDropdown();
    }

    public void selectTrackDropdown()
    {
        if (trackDropdown.options[trackDropdown.value].text == "0")
        {
            selectedTrack = 0;
        }
        else
        {
            selectedTrack = float.Parse(trackDropdown.options[trackDropdown.value].text);
        }
    }

    public void StartAnimation()
    {
        if (selectedTrack != 0)
        {
            if (selectedTrack > 20/*getNumTracks()*/)
            {
                selectedTrack = 1;
            }
            midiAudioMixerGroup.audioMixer.SetFloat("PistaBajo", selectedTrack / 20f);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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

        midiAudioMixerGroup.audioMixer.SetFloat("CambiarMidi", 1);
        Invoke("restartMidi", 0.5f);
    }

    void restartMidi()
    {
        midiAudioMixerGroup.audioMixer.SetFloat("CambiarMidi", 0);
    }

    //public void fillDropdown()
    //{
    //    if (trackDropdown.options.Count > 0)
    //    {
    //        trackDropdown.options.Clear();
    //    }

    //    Thread.Sleep(200);
    //    numTracks = getNumTracks();
    //    for (int i = 1; i < numTracks; i++)
    //    {
    //        trackDropdown.options.Add(new TMP_Dropdown.OptionData() { text = i.ToString() });
    //    }
    //    trackDropdown.RefreshShownValue();
    //}
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
