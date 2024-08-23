using System.Collections.Generic;
using UnityEngine;

public class FretboardNotes : MonoBehaviour
{
    #region Definicion de variables

    [SerializeField] GameObject notePrefab;
    [SerializeField] int noteNumber;
    [SerializeField] Transform finger;
    [SerializeField] List<double> timeStamps;
    [SerializeField] int handPosition;

    [SerializeField] Quaternion rotationFinger4;
    [SerializeField] Quaternion rotationFinger3;
    [SerializeField] Quaternion rotationFinger2;

    int index = 0;

    #endregion

    #region Preparar notas

    public void SetTimeStamps(int[] notes, double[] timestamps)
    {
        for (int i = 0; i < notes.Length; i++)
        {
            if (noteNumber == notes[i])
            {
                timeStamps.Add(timestamps[i]);
            }
        }
    }

    #endregion

    void Update()
    {
        if (index < timeStamps.Count && MidiManager.midiManagerInstance.currentTime >= timeStamps[index])
        {
            AnimationHand animationHand = FindObjectOfType<AnimationHand>();
            AnimationFinger[] animationFingers = FindObjectsOfType<AnimationFinger>();
            foreach (AnimationFinger animationFinger in animationFingers)
            {
                if (animationFinger.gameObject.name == finger.name)
                {
                    animationHand.moveHand(noteNumber, handPosition, animationFinger, rotationFinger2, 
                        rotationFinger3, rotationFinger4, timeStamps[index], timeStamps[index] + 1);
                }
            }
            index++;
        }
    }
}
