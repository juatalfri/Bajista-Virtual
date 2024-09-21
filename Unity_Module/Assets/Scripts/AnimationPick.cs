using System;
using System.Collections;
using System.Collections.Generic;
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
    
    [SerializeField] Quaternion pickRotation1;
    [SerializeField] Quaternion pickRotation2;

    [SerializeField] Vector3 handPickPosition1;
    [SerializeField] Vector3 handPickPosition2;
    [SerializeField] Vector3 handPickPosition3;
    [SerializeField] Vector3 handPickPosition4;

    [SerializeField] Quaternion handPickRotation1;
    [SerializeField] Quaternion handPickRotation2;

    [SerializeField] bool pickMode;

    #endregion

    #region Funciones de animacion

    public void movePick(GameObject notePrefab)
    {        
        if (pickMode)
        {
            if (notePrefab.name.Contains("String 1"))
            {
                StartCoroutine(SlerpPosition(0.02, pickPosition1));
            }
            else if (notePrefab.name.Contains("String 2"))
            {
                StartCoroutine(SlerpPosition(0.02, pickPosition2));
            }
            else if (notePrefab.name.Contains("String 3"))
            {
                StartCoroutine(SlerpPosition(0.02, pickPosition3));
            }
            else if (notePrefab.name.Contains("String 4"))
            {
                StartCoroutine(SlerpPosition(0.02, pickPosition4));
            }
        }
        else
        {
            if (notePrefab.name.Contains("String 1"))
            {
                StartCoroutine(SlerpPosition(0.02, handPickPosition1));
            }
            else if (notePrefab.name.Contains("String 2"))
            {
                StartCoroutine(SlerpPosition(0.02, handPickPosition2));
            }
            else if (notePrefab.name.Contains("String 3"))
            {
                StartCoroutine(SlerpPosition(0.02, handPickPosition3));
            }
            else if (notePrefab.name.Contains("String 4"))
            {
                StartCoroutine(SlerpPosition(0.02, handPickPosition4));
            }
        }
    }

    IEnumerator SlerpPosition(double slerpDuration, Vector3 newPosition)
    {
        Transform obj;
        if (pickMode)
        {
            obj = pick;
        }
        else
        {
            obj = handPick;
        }

        double timeElapsed = 0;

        obj.localPosition = newPosition;
        newPosition.y = newPosition.y - 0.4f;
        
        while (timeElapsed < slerpDuration)
        {
            obj.localPosition = Vector3.Slerp(obj.localPosition, newPosition,
                    (float)(timeElapsed / slerpDuration));
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        obj.localPosition = newPosition;
    }

    #endregion
}
