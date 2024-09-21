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

    [SerializeField] GameObject freeNote;

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
            AnimationPick animationPick = FindObjectOfType<AnimationPick>();
            AnimationFinger[] animationFingers = FindObjectsOfType<AnimationFinger>();
            if (freeNote != null)
            {
                animationHand.moveHand(notePrefab, noteNumber, handPosition, rotationFinger2,
                    rotationFinger3, rotationFinger4, timeStamps[index], timeStamps[index] + 1, freeNote);
            }
            else
            {
                foreach (AnimationFinger animationFinger in animationFingers)
                {
                    if (animationFinger.gameObject.name == finger.name)
                    {
                        animationHand.moveHand(notePrefab, noteNumber, handPosition, rotationFinger2,
                            rotationFinger3, rotationFinger4, timeStamps[index], timeStamps[index] + 1, freeNote, animationFinger, animationPick);
                    }
                }
            }
            index++;
        }
    }
}
