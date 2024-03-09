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
    public float recordingTime = 5f;//记录存取时间

    public float moveSpeed = 3f;
    public float jumpForce = 10f;
    private float timer = 0f;
    private float intervalTime = 0.5f;//大于这个时间判定为长按
    private float recordTime = 1f;//按满1s触发长按事件
    private bool isRecorded = false;
    private bool longPress = false;

    public void InputListener(GameObject player,Rigidbody player_rd)
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            //暂停
            Time.timeScale = 0;
            
        }
        //短按释放，长按记录动作
        else if (Input.GetKey(KeyCode.E))
        {
            timer += Time.deltaTime;
            //此处只检测长按逻辑
            if (timer >= intervalTime)//超过间隔时间也就是长按
            {
                longPress = true;
                if (timer >= recordTime && !isRecorded)//开启记录
                {
                    Recording();
                }
                else//未开启，进度条++
                {
                    recordSlider.value = timer / recordTime;
                }
            }
        }
        else if(Input.GetKeyUp(KeyCode.E))//利用按键抬手判断是不是短按
        {
            if(!longPress)
            {
                
                if (timer < intervalTime)
                {
                    timer = 0f;//复位
                    recordSlider.value = 0;
                    if(isRecorded)
                    {
                        Releasing();
                    }
                    else
                    {
                        Debug.Log("无记录！");
                    }
                    playerState = Global.PlayerState.Idle;
                }
            }
            else if(timer < recordTime)
            {
                longPress = false;
                timer = 0f;//复位
                recordSlider.value = 0;
            }
        }
        else if(Input.GetKeyDown(KeyCode.S))//切换画面绘制纸人
        {
            playerState = Global.PlayerState.ToSummon;
            ToSummon();
        }
        //跳跃
        if(Input.GetKey(KeyCode.Space))
        {
            playerState = Global.PlayerState.Jumping;
            player_rd.AddForce(new Vector3(0,jumpForce,0));

            playerState = lastState;
        }
        //移动
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
        Debug.Log("召唤跳转！");
        playerState = Global.PlayerState.Idle;
    }
    public void Recording()
    {
        playerState = Global.PlayerState.Recording;
        SketchSystem.Instance.StartRecordingAction();
        recordingTimeSlider.GameObject().SetActive(true);
        Debug.Log("开始记录！");
        timer = 0;//复位
        recordSlider.value = 0;
        IEnumerator coroutine = Recorder();
        StartCoroutine(coroutine);
    }
    public void Releasing()
    {
        playerState = Global.PlayerState.Releasing;
        //SketchSystem.Instance.CopyActionToSkecthMan(sketchman);
        SketchSystem.Instance.ReleasingAction();
        //释放纸人实体
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
        Debug.Log("记录完成！");
        playerState = Global.PlayerState.Idle;
        recordingTimeSlider.value = 0;
        recordingTimeSlider.GameObject().SetActive(false);
    }
}
