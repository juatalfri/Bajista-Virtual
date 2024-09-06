using TMPro;
using UnityEngine;
using System.Runtime.InteropServices;
using SFB;
using System.IO;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    #region Definicion de variables

    [SerializeField] AudioMixerGroup midiAudioMixerGroup;
    [SerializeField] Slider velocitySlider;
    [SerializeField] TMP_InputField velocityField;

    #endregion

    #region Funciones del menu

    public void selectNoteVelocitySlider()
    {
        velocityField.text = velocitySlider.value.ToString().Substring(0,5);
        midiAudioMixerGroup.audioMixer.SetFloat("Velocidad", velocitySlider.value);
    }

    public void selectNoteVelocityField()
    {
        velocitySlider.value = float.Parse(velocityField.text);
        midiAudioMixerGroup.audioMixer.SetFloat("Velocidad", float.Parse(velocityField.text));
    }

    #endregion
}
