using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class LevelFinishEvent : MonoBehaviour
{
    private Vector3 originPos;
    private Vector3 targetPos;
    public List<GameObject> levelSwitches;
    public GameObject camLast;
    public GameObject camNext;
    public GameObject sightReleaser;
    private float upSpeed;
    public void MoveUpByAnim()
    {
        gameObject.GetComponent<Animator>().SetBool("Play", true);
    }
    public void MoveUp(float _distance)
    {
        originPos = gameObject.transform.position;
        targetPos = gameObject.transform.position + new Vector3(0, _distance, 0);
        transform.position = targetPos;
    }
    public void MoveDown(float _distance)
    {
        
    }
    public void ReturnOriginPosition()
    {
        gameObject.transform.position = originPos;
    }
    public void LevelFinishStateForSwitches()
    {
        foreach (var switches in levelSwitches)
        {
            switches.GetComponent<LevelSwitch>().LevelFinishState();
        }
    }
    public void LevelFinishStateForSights()
    {
        sightReleaser.GetComponent<SightSwitch>().LevelFinishState();
        sightReleaser.SetActive(false);
    }
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
    public void CloserReleaser()
    {
        sightReleaser.GetComponentInChildren<ReflectiveProjection>().enabled = false;
        sightReleaser.GetComponentInChildren<LineRenderer>().enabled = false;
        sightReleaser.SetActive(false);
    }

}
