using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEvent : MonoBehaviour
{
    public GameObject camLast;
    public GameObject camNext;
    public Camera cam;
    public Color _color;
    // Start is called before the first frame update
    public void SetNextCamera()
    {
        camNext.GetComponent<CinemachineVirtualCamera>().Priority = 15;
        camLast.GetComponent<CinemachineVirtualCamera>().Priority = 10;
    }

    public void PreViewNextCamera(float _time)
    {
        camNext.GetComponent<CinemachineVirtualCamera>().Priority = 15;
        camLast.GetComponent<CinemachineVirtualCamera>().Priority = 10;
        IEnumerator enumerator = CameraRetrun(_time);
        StartCoroutine(enumerator);
    }
    IEnumerator CameraRetrun(float _time)
    {
        yield return new WaitForSeconds(_time);
        camLast.GetComponent<CinemachineVirtualCamera>().Priority = 15;
        camNext.GetComponent<CinemachineVirtualCamera>().Priority = 10;
    }
    public void SetCamColor()
    {
        cam.backgroundColor = _color;
    }
}
