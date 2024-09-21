using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AnimationPick :  MonoBehaviour
{

    #region Definicion de variables

    [SerializeField] Transform handPick;
    [SerializeField] Transform pick;

    [SerializeField] Vector3 pickPosition1;
    [SerializeField] Vector3 pickPosition2;
    [SerializeField] Vector3 pickPosition3;
    [SerializeField] Vector3 pickPosition4;

    [SerializeField] Vector3 handPickPosition1;
    [SerializeField] Vector3 handPickPosition2;
    [SerializeField] Vector3 handPickPosition3;
    [SerializeField] Vector3 handPickPosition4;

    [SerializeField] Quaternion handPickRotation1;
    [SerializeField] Quaternion handPickRotation2;


    #endregion

    #region Funciones de animacion

    public void movePick(GameObject notePrefab)
    {        
        if (MidiManager.midiManagerInstance.pickMode)
        {

            if (notePrefab.name.Contains("String 1"))
            {
                StartCoroutine(Slerp(0.02, pickPosition1));
            }
            else if (notePrefab.name.Contains("String 2"))
            {
                StartCoroutine(Slerp(0.02, pickPosition2));
            }
            else if (notePrefab.name.Contains("String 3"))
            {
                StartCoroutine(Slerp(0.02, pickPosition3));
            }
            else if (notePrefab.name.Contains("String 4"))
            {
                StartCoroutine(Slerp(0.02, pickPosition4));
            }
        }
        else
        {
            if (notePrefab.name.Contains("String 1"))
            {
                StartCoroutine(Slerp(0.02, handPickPosition1));
            }
            else if (notePrefab.name.Contains("String 2"))
            {
                StartCoroutine(Slerp(0.02, handPickPosition2));
            }
            else if (notePrefab.name.Contains("String 3"))
            {
                StartCoroutine(Slerp(0.02, handPickPosition3));
            }
            else if (notePrefab.name.Contains("String 4"))
            {
                StartCoroutine(Slerp(0.02, handPickPosition4));
            }
        }
    }

    IEnumerator Slerp(double slerpDuration, Vector3 newPosition)
    {
        double timeElapsed = 0;
        
        if (MidiManager.midiManagerInstance.pickMode)
        {
            pick.localPosition = newPosition;
            newPosition.y = newPosition.y - 0.5f;
            while (timeElapsed < slerpDuration)
            {
                pick.localPosition = Vector3.Slerp(pick.localPosition, newPosition,
                        (float)(timeElapsed / slerpDuration));
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            pick.localPosition = newPosition;
        }
        else
        {
            handPick.localPosition = newPosition;
            handPick.localRotation = handPickRotation1;

            while (timeElapsed < slerpDuration)
            {
                handPick.localRotation = Quaternion.Slerp(handPick.localRotation, handPickRotation2,
                        (float)(timeElapsed / slerpDuration));
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            handPick.localRotation = handPickRotation2;
        }
    }
    #endregion
}
