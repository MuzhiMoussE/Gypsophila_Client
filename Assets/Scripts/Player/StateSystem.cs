using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utility;

public class StateSystem : SingletonMonoBase<StateSystem>
{
    public Global.PlayerState playerState = Global.PlayerState.Idle;
    public Global.PlayerState lastState = Global.PlayerState.Idle;
    public Slider recordSlider;
    public Slider recordingTimeSlider;
    public float recordingTime = 5f;//��¼��ȡʱ��

    public float moveSpeed = 3f;
    public float jumpForce = 10f;
    private float timer = 0f;
    private float intervalTime = 0.5f;//�������ʱ���ж�Ϊ����
    private float recordTime = 1f;//����1s���������¼�
    private bool isRecorded = false;
    private bool longPress = false;

    public void InputListener(GameObject player,Rigidbody player_rd)
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            //��ͣ
            Time.timeScale = 0;
            
        }
        //�̰��ͷţ�������¼����
        else if (Input.GetKey(KeyCode.E))
        {
            timer += Time.deltaTime;
            //�˴�ֻ��ⳤ���߼�
            if (timer >= intervalTime)//�������ʱ��Ҳ���ǳ���
            {
                longPress = true;
                if (timer >= recordTime && !isRecorded)//������¼
                {
                    Recording();
                }
                else//δ������������++
                {
                    recordSlider.value = timer / recordTime;
                }
            }
        }
        else if(Input.GetKeyUp(KeyCode.E))//���ð���̧���ж��ǲ��Ƕ̰�
        {
            if(!longPress)
            {
                
                if (timer < intervalTime)
                {
                    timer = 0f;//��λ
                    recordSlider.value = 0;
                    if(isRecorded)
                    {
                        Releasing();
                    }
                    else
                    {
                        Debug.Log("�޼�¼��");
                    }
                    playerState = Global.PlayerState.Idle;
                }
            }
            else if(timer < recordTime)
            {
                longPress = false;
                timer = 0f;//��λ
                recordSlider.value = 0;
            }
        }
        else if(Input.GetKeyDown(KeyCode.S))//�л��������ֽ��
        {
            playerState = Global.PlayerState.ToSummon;
            ToSummon();
        }
        //��Ծ
        if(Input.GetKey(KeyCode.Space))
        {
            playerState = Global.PlayerState.Jumping;
            player_rd.AddForce(new Vector3(0,jumpForce,0));

            playerState = lastState;
        }
        //�ƶ�
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            playerState = Global.PlayerState.Moving;
            player.transform.position -= new Vector3(moveSpeed * Time.fixedDeltaTime, 0, 0);
            lastState = playerState;
        }
        else if(Input.GetKey(KeyCode.D)  || Input.GetKey(KeyCode.RightArrow))
        {
            playerState = Global.PlayerState.Moving;
            player.transform.position += new Vector3(moveSpeed * Time.fixedDeltaTime, 0, 0);
            lastState = playerState;

        }
        else
        {
            playerState = Global.PlayerState.Idle;
            lastState = playerState;
        }

    }
    public void ToSummon()
    {
        //SceneManager.LoadScene(2);
        Debug.Log("�ٻ���ת��");
        playerState = Global.PlayerState.Idle;
    }
    public void Recording()
    {
        playerState = Global.PlayerState.Recording;
        SketchSystem.Instance.StartRecordingAction();
        recordingTimeSlider.GameObject().SetActive(true);
        Debug.Log("��ʼ��¼��");
        timer = 0;//��λ
        recordSlider.value = 0;
        IEnumerator coroutine = Recorder();
        StartCoroutine(coroutine);
    }
    public void Releasing()
    {
        playerState = Global.PlayerState.Releasing;
        //SketchSystem.Instance.CopyActionToSkecthMan(sketchman);
        SketchSystem.Instance.ReleasingAction();
        //�ͷ�ֽ��ʵ��
        isRecorded = false;
        playerState = Global.PlayerState.Idle;
    }
    IEnumerator Recorder()
    {
        while(recordingTimeSlider.value < 1f)
        {
            recordingTimeSlider.value += recordingTime / 50f; 
            yield return new WaitForSeconds(recordingTime/10f);
        }
        isRecorded = true;
        longPress = false;
        SketchSystem.Instance.StopRecordingAction();
        Debug.Log("��¼��ɣ�");
        playerState = Global.PlayerState.Idle;
        recordingTimeSlider.value = 0;
        recordingTimeSlider.GameObject().SetActive(false);
    }
}
