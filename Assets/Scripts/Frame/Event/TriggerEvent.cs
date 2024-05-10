using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class TriggerEvent : MonoBehaviour
{
    public GameObject storyObject;
    public GameObject triggerObj;
    public GameObject camLast;
    public GameObject camNext;
    public Camera cam;
    public Color _color;
    public AudioSource audioSource;
    public AudioClip clip;
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
    public void SetTriggerObjDown()
    {
        triggerObj.GetComponent<Rigidbody>().isKinematic = false;
    }
    public void SetAnimPlay2()
    {
        triggerObj.GetComponent<Animator>().SetBool("Play2", true);
    }
    public void SetAnimPlay()
    {
        triggerObj.GetComponent<Animator>().SetBool("Play", true);
    }
    public void ObjectAppear()
    {
        triggerObj.SetActive(true);
    }
    public void ToNextScene()
    {
        ArchiveSystem.ClearLevel();
        ArchiveSystem.SceneIndex++;
        ArchiveSystem.LoadScene(ArchiveSystem.SceneIndex);
    }
    public void StartStory()
    {
        storyObject.SetActive(true);
        storyObject.GetComponent<PlayableDirector>().Play();
    }
    public void TipsDisappear()
    {
        StateSystem.Instance.showTips = false;
    }
    public void TipsAppear()
    {
        StateSystem.Instance.showTips = true;
    }
    public void SetTip(string text)
    {
        StateSystem.Instance.tipsText.text = text;
    }
    public void PlayAudio()
    {
        audioSource.PlayOneShot(clip);
    }
    public void stopMom()
    {
        triggerObj.GetComponent<Mom>().canWalk = false;
        triggerObj.GetComponent<Mom>().isWalking = false;
        triggerObj.GetComponent<Animator>().SetBool("mom_thank", true);
    }
    public void DeleteObj()
    {
        Destroy(triggerObj);
    }
    public void ChangeGravity()
    {
        triggerObj.GetComponent<Rigidbody>().drag = 5.0f;
    }
    public void ResetGravity()
    {
        triggerObj.GetComponent<Rigidbody>().drag = 0;
    }
}
