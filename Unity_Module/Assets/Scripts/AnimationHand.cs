using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHand :  MonoBehaviour
{

    #region Definicion de variables

    [SerializeField] Transform hand;

    [SerializeField] Vector3 handPosition4;
    [SerializeField] Vector3 handPosition1;
    [SerializeField] Vector3 handPosition2;
    [SerializeField] Vector3 handPosition3;

    [SerializeField] Quaternion handRotation4;
    [SerializeField] Quaternion handRotation1;
    [SerializeField] Quaternion handRotation2;
    [SerializeField] Quaternion handRotation3;

    [SerializeField] Material colouredNote;
    [SerializeField] Material standardNote;


    #endregion

    #region Funciones de animacion

    public void moveHand(GameObject notePrefab, int note, int handPositionNote, Quaternion rotationFinger2,
        Quaternion rotationFinger3, Quaternion rotationFinger4, double currentTimestamp, double nextTimestamp,
        GameObject freeNote = null, AnimationFinger animationFinger = null, AnimationPick animationPick = null) 
    {
        bool inHand1 = MidiManager.midiManagerInstance.notesHand1.Contains(note);
        bool inHand2 = MidiManager.midiManagerInstance.notesHand2.Contains(note);
        bool inHand3 = MidiManager.midiManagerInstance.notesHand3.Contains(note);
        bool inHand4 = MidiManager.midiManagerInstance.notesHand4.Contains(note);

        switch (handPositionNote)
        {
            case 0:
                if (!inHand1 && !inHand2 && !inHand3 && !inHand4)
                {
                    if (freeNote.GetComponent<MeshRenderer>().material.color == standardNote.color)
                    {
                        animationPick.movePick(notePrefab);
                        freeNote.GetComponent<MeshRenderer>().material.color = colouredNote.color;
                    }
                    else
                    {
                        freeNote.GetComponent<MeshRenderer>().material.color = standardNote.color;
                    }
                }
                break;

            case 4:
                if (!animationFinger.active)
                {
                    if (!inHand1 && !inHand2 && !inHand3)
                    {
                        if ((nextTimestamp - currentTimestamp) < 0.03)
                        {
                            hand.localPosition = handPosition1;
                            hand.localRotation = handRotation1;
                        }
                        else
                        {
                            hand.localRotation = handRotation1;
                            StartCoroutine(SlerpPosition(0.03, handPosition4));
                        }
                        animationPick.movePick(notePrefab);
                        animationFinger.moveFingerUp(rotationFinger2, rotationFinger3, rotationFinger4);
                        MidiManager.midiManagerInstance.currentHandPosition = 4;
                    }
                }
                else
                {
                    animationFinger.moveFingerDown(currentTimestamp, nextTimestamp);
                }
                break;

            case 1:
                if (!animationFinger.active)
                {
                    if (handPositionNote == MidiManager.midiManagerInstance.currentHandPosition)
                    {
                        animationPick.movePick(notePrefab);
                        animationFinger.moveFingerUp(rotationFinger2, rotationFinger3, rotationFinger4);
                    }
                    else if (!inHand2)
                    {
                        if (!inHand3 || MidiManager.midiManagerInstance.predominantlyHand == 1)
                        {
                            if ((nextTimestamp - currentTimestamp) < 0.03)
                            {
                                hand.localPosition = handPosition1;
                                hand.localRotation = handRotation1;
                            }
                            else
                            {
                                hand.localRotation = handRotation1;
                                StartCoroutine(SlerpPosition(0.03, handPosition1));
                            }

                            animationPick.movePick(notePrefab);
                            animationFinger.moveFingerUp(rotationFinger2, rotationFinger3, rotationFinger4);
                            MidiManager.midiManagerInstance.currentHandPosition = 1;
                        }
                    }
                }
                else 
                {
                    animationFinger.moveFingerDown(currentTimestamp, nextTimestamp);
                }
                break;

            case 2:
                if (!animationFinger.active)
                {
                    if (handPositionNote == MidiManager.midiManagerInstance.currentHandPosition)
                    {
                        animationPick.movePick(notePrefab);
                        animationFinger.moveFingerUp(rotationFinger2, rotationFinger3, rotationFinger4);
                    }
                    else if ((!inHand1 || MidiManager.midiManagerInstance.predominantlyHand == 2) && MidiManager.midiManagerInstance.currentHandPosition == 1)
                    {
                        if ((nextTimestamp - currentTimestamp) < 0.03)
                        {
                            hand.localPosition = handPosition2;
                            hand.localRotation = handRotation2;
                        }
                        else
                        {
                            hand.localRotation = handRotation2;
                            StartCoroutine(SlerpPosition(0.03, handPosition2));
                        }

                        animationPick.movePick(notePrefab);
                        animationFinger.moveFingerUp(rotationFinger2, rotationFinger3, rotationFinger4);
                        MidiManager.midiManagerInstance.currentHandPosition = 2;
                    }
                    else if ((!inHand3 || MidiManager.midiManagerInstance.predominantlyHand == 1) && MidiManager.midiManagerInstance.currentHandPosition == 3)
                    {
                        if ((nextTimestamp - currentTimestamp) < 0.03)
                        {
                            hand.localPosition = handPosition2;
                            hand.localRotation = handRotation2;
                        }
                        else
                        {
                            hand.localRotation = handRotation2;
                            StartCoroutine(SlerpPosition(0.03, handPosition2));
                        }

                        animationPick.movePick(notePrefab);
                        animationFinger.moveFingerUp(rotationFinger2, rotationFinger3, rotationFinger4);
                        MidiManager.midiManagerInstance.currentHandPosition = 2;
                    }
                }
                else
                {
                    animationFinger.moveFingerDown(currentTimestamp, nextTimestamp);
                }
                break;

            case 3:
                if (!animationFinger.active)
                {
                    if (handPositionNote == MidiManager.midiManagerInstance.currentHandPosition)
                    {
                        animationPick.movePick(notePrefab);
                        animationFinger.moveFingerUp(rotationFinger2, rotationFinger3, rotationFinger4);
                    }
                    else if (!inHand2)
                    {
                        if (!inHand1 || MidiManager.midiManagerInstance.predominantlyHand == 3)
                        {
                            if ((nextTimestamp - currentTimestamp) < 0.03)
                            {
                                hand.localPosition = handPosition3;
                                hand.localRotation = handRotation3;
                            }
                            else
                            {
                                hand.localRotation = handRotation3;
                                StartCoroutine(SlerpPosition(0.03, handPosition3));
                            }

                            animationPick.movePick(notePrefab);
                            animationFinger.moveFingerUp(rotationFinger2, rotationFinger3, rotationFinger4);
                            MidiManager.midiManagerInstance.currentHandPosition = 3;
                        }
                    }
                }
                else
                {
                    animationFinger.moveFingerDown(currentTimestamp, nextTimestamp);
                }
                break;
        }
    }

    IEnumerator SlerpPosition(double slerpDuration, Vector3 newPosition)
    {
        double timeElapsed = 0;

        while (timeElapsed < slerpDuration)
        {

            hand.localPosition = Vector3.Slerp(hand.localPosition, newPosition,
                    (float)(timeElapsed / slerpDuration));

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        hand.localPosition = newPosition;
    }

    #endregion
}
