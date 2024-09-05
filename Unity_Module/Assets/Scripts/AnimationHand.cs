using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHand :  MonoBehaviour
{

    #region Definicion de variables

    [SerializeField] Transform hand;
    [SerializeField] Transform pick;

    [SerializeField] Vector3 pickPosition1;
    [SerializeField] Vector3 pickPosition2;
    [SerializeField] Vector3 pickPosition3;
    [SerializeField] Vector3 pickPosition4;
    
    [SerializeField] Quaternion pickRotation1;
    [SerializeField] Quaternion pickRotation2;

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
        GameObject freeNote = null, AnimationFinger animationFinger = null)
    {
        bool inHand4 = MidiManager.midiManagerInstance.notesHand4.Contains(note);
        bool inHand1 = MidiManager.midiManagerInstance.notesHand1.Contains(note);
        bool inHand2 = MidiManager.midiManagerInstance.notesHand2.Contains(note);
        bool inHand3 = MidiManager.midiManagerInstance.notesHand3.Contains(note);

        switch (handPositionNote)
        {
            case 0:
                if (!inHand4 && !inHand1 && !inHand2 && !inHand3)
                {
                    if (freeNote.GetComponent<MeshRenderer>().material.color == standardNote.color)
                    {
                        movePick(notePrefab);
                        freeNote.GetComponent<MeshRenderer>().material = colouredNote;
                    }
                    else
                    {
                        freeNote.GetComponent<MeshRenderer>().material = standardNote;
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
                        movePick(notePrefab);
                        animationFinger.moveFingerUp(rotationFinger2, rotationFinger3, rotationFinger4,
                            currentTimestamp, nextTimestamp);
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
                        movePick(notePrefab);
                        animationFinger.moveFingerUp(rotationFinger2, rotationFinger3, rotationFinger4,
                            currentTimestamp, nextTimestamp);
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

                            movePick(notePrefab);
                            animationFinger.moveFingerUp(rotationFinger2, rotationFinger3, rotationFinger4,
                                currentTimestamp, nextTimestamp);
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
                        movePick(notePrefab);
                        animationFinger.moveFingerUp(rotationFinger2, rotationFinger3, rotationFinger4,
                            currentTimestamp, nextTimestamp);
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

                        movePick(notePrefab);
                        animationFinger.moveFingerUp(rotationFinger2, rotationFinger3, rotationFinger4,
                            currentTimestamp, nextTimestamp);
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

                        movePick(notePrefab);
                        animationFinger.moveFingerUp(rotationFinger2, rotationFinger3, rotationFinger4,
                            currentTimestamp, nextTimestamp);
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
                        movePick(notePrefab);
                        animationFinger.moveFingerUp(rotationFinger2, rotationFinger3, rotationFinger4,
                            currentTimestamp, nextTimestamp);
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

                            movePick(notePrefab);
                            animationFinger.moveFingerUp(rotationFinger2, rotationFinger3, rotationFinger4,
                                currentTimestamp, nextTimestamp);
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

    void movePick(GameObject notePrefab)
    {
        if (notePrefab.name.Contains("String 1"))
        {
            StartCoroutine(SlerpPosition(0.02, pickPosition1, "pick"));
        }
        else if (notePrefab.name.Contains("String 2"))
        {
            StartCoroutine(SlerpPosition(0.02, pickPosition2, "pick"));
        }
        else if (notePrefab.name.Contains("String 3"))
        {
            StartCoroutine(SlerpPosition(0.02, pickPosition3, "pick"));
        }
        else if (notePrefab.name.Contains("String 4"))
        {
            StartCoroutine(SlerpPosition(0.02, pickPosition4, "pick"));
        }
    }

    IEnumerator SlerpPosition(double slerpDuration, Vector3 newPosition, String objectType = "hand")
    {
        double timeElapesd = 0;

        if (objectType == "pick")
        {
            pick.localPosition = newPosition;
            newPosition.y = newPosition.y - 0.4f;
        }
        while (timeElapesd < slerpDuration)
        {
            if (objectType == "pick")
            {
                pick.localPosition = Vector3.Slerp(pick.localPosition, newPosition,
                    (float)(timeElapesd / slerpDuration));
            }
            else
            {
                hand.localPosition = Vector3.Slerp(hand.localPosition, newPosition,
                    (float)(timeElapesd / slerpDuration));
            }

            timeElapesd += Time.deltaTime;
            yield return null;
        }
        if (objectType == "pick")
        {
            pick.localPosition = newPosition;
        }
        else
        {
            hand.localPosition = newPosition;
        }
    }

    //IEnumerator SlerpRotation(double slerpDuration)
    //{
    //    double timeElapesd = 0;

    //    while (timeElapesd < slerpDuration)
    //    {
    //        pick.localRotation = Quaternion.Slerp(pick.localRotation, pickRotation2,
    //            (float)(timeElapesd / (slerpDuration)));

    //        timeElapesd += Time.deltaTime;
    //        yield return null;
    //    }
    //    pick.localRotation = pickRotation2;
    //}
    #endregion
}
