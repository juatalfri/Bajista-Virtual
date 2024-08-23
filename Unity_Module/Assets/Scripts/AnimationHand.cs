using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHand :  MonoBehaviour
{

    #region Definicion de variables

    [SerializeField] Transform hand;

    [SerializeField] Vector3 handPosition1;
    [SerializeField] Vector3 handPosition2;
    [SerializeField] Vector3 handPosition3;

    [SerializeField] Quaternion handRotation1;
    [SerializeField] Quaternion handRotation2;
    [SerializeField] Quaternion handRotation3;

    #endregion

    #region Funciones de animacion

    public void moveHand(int note, int handPositionNote, AnimationFinger animationFinger, Quaternion rotationFinger2,
        Quaternion rotationFinger3, Quaternion rotationFinger4, double currentTimestamp, double nextTimestamp)
    {
        bool inHand1 = MidiManager.midiManagerInstance.notesHand1.Contains(note);
        bool inHand2 = MidiManager.midiManagerInstance.notesHand2.Contains(note);
        bool inHand3 = MidiManager.midiManagerInstance.notesHand3.Contains(note);

        switch (handPositionNote)
        {
            case 1:
                if (!animationFinger.active)
                {
                    if (handPositionNote == MidiManager.midiManagerInstance.currentHandPosition)
                    {
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
                                StartCoroutine(SlerpPosition(0.03, handPosition1, handRotation1));
                            }

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
                            StartCoroutine(SlerpPosition(0.03, handPosition2, handRotation2));
                        }

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
                            StartCoroutine(SlerpPosition(0.03, handPosition2, handRotation2));
                        }

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
                                StartCoroutine(SlerpPosition(0.03, handPosition3, handRotation3));
                            }

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

    IEnumerator SlerpPosition(double slerpDuration, Vector3 handPosition, Quaternion handRotation)
    {
        double timeElapesd = 0;

        while (timeElapesd < slerpDuration)
        {
            hand.localPosition = Vector3.Slerp(hand.localPosition, handPosition,
                (float)(timeElapesd / slerpDuration));
            hand.localRotation = handRotation;
            timeElapesd += Time.deltaTime;
            yield return null;
        }
        hand.localPosition = handPosition;
    }
    #endregion
}
