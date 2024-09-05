using System.Collections;
using UnityEngine;

public class AnimationFinger :  MonoBehaviour
{

    #region Definicion de variables

    [SerializeField] Transform finger2;
    [SerializeField] Transform finger3;
    [SerializeField] Transform finger4;

    [SerializeField] Quaternion defaultFinger2Rotation;
    [SerializeField] Quaternion defaultFinger3Rotation;
    [SerializeField] Quaternion defaultFinger4Rotation;

    public static AnimationFinger fingerInstance;
    public bool active = false;

    #endregion

    #region Funciones de animacion

    public void moveFingerUp(Quaternion rotationFinger2, Quaternion rotationFinger3, Quaternion rotationFinger4, 
        double currentTimestamp, double nextTimestamp)
    {        
            finger2.localRotation = rotationFinger2;
            finger3.localRotation = rotationFinger3;
            finger4.localRotation = rotationFinger4; 

            active = true;
    }

    public void moveFingerDown(double currentTimestamp, double nextTimestamp)
    {
        if ((nextTimestamp - currentTimestamp) < 0.1)
        {
            finger2.localRotation = defaultFinger2Rotation;
            finger3.localRotation = defaultFinger3Rotation;
            finger4.localRotation = defaultFinger4Rotation;
        }
        else
        {
            StartCoroutine(SlerpRotation(0.08));
        }

        active = false;
    }

    IEnumerator SlerpRotation(double slerpDuration)
    {
        double timeElapesd = 0;

        while (timeElapesd < slerpDuration)
        {
            finger2.localRotation = Quaternion.Slerp(finger2.localRotation, defaultFinger2Rotation,
                (float)(timeElapesd / (slerpDuration - 0.04)));
            finger3.localRotation = Quaternion.Slerp(finger3.localRotation, defaultFinger3Rotation,
                (float)(timeElapesd / (slerpDuration - 0.02)));
            finger4.localRotation = Quaternion.Slerp(finger4.localRotation, defaultFinger4Rotation,
                (float)(timeElapesd / slerpDuration));

            timeElapesd += Time.deltaTime;
            yield return null;
        }
        finger2.localRotation = defaultFinger2Rotation;
        finger3.localRotation = defaultFinger3Rotation;
        finger4.localRotation = defaultFinger4Rotation;
    }

    #endregion

    void Start()
    {
        defaultFinger2Rotation = finger2.localRotation;
        defaultFinger3Rotation = finger3.localRotation;
        defaultFinger4Rotation = finger4.localRotation;
    }
}   
