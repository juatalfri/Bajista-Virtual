using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class CameraManager : MonoBehaviour
{
    #region Definicion de variables

    public static CameraManager cameraManagerInstance;

    public Camera camera1;
    public Camera camera2;
    public Camera camera3;
    public Camera camera4;

    #endregion

    #region Funciones CameraManager

    public void EnableCamera1()
    {
        camera2.enabled = false;
        camera3.enabled = false;
        camera4.enabled = false; 
        
        camera1.enabled = true;
    }


    public void EnableCamera2()
    {
        camera1.enabled = false;
        camera3.enabled = false;
        camera4.enabled = false;

        camera2.enabled = true;
    }

    public void EnableCamera3()
    {
        camera1.enabled = false;
        camera2.enabled = false;
        camera4.enabled = false;

        camera3.enabled = true;
    }

    public void EnableCamera4()
    {
        camera1.enabled = false;
        camera2.enabled = false;
        camera3.enabled = false;

        camera4.enabled = true;
    }

    #endregion

    void Start()
    {
        EnableCamera1();
        cameraManagerInstance = this;
    }
}