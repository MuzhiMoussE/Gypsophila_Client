using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class SketchUtility : MonoBehaviour
{
    //图片序列->sprite动画
    public enum State
    {
        idle,
        playing,
        pause
    }
    public enum State1
    {
        once,
        loop
    }

    [Header("播放方式(循环、单次)")]//默认单次
    public State1 condition = State1.once;
    [Header("自动播放")]//默认不自动播放
    public bool Play_Awake = false;
    //播放状态(默认、播放中、暂停)
    private State play_state;
    private Image manimg;
    [Header("每秒播放的帧数(整数)")]
    public float frame_number = 30;
    [Header("sprite数组")]
    public Sprite[] sprit_arr;
    //回调事件
    public UnityEvent onCompleteEvent;
    private int index;
    private float tim;
    private float waittim;
    private bool isplay;
    void Awake()
    {
        manimg = GetComponent<Image>();
        tim = 0;
        index = 0;
        waittim = 1 / frame_number;
        play_state = State.idle;
        isplay = false;
        if (manimg == null)
        {
            Debug.Log("Image为空，请添加Image组件！！！");
            return;
        }
        if (sprit_arr.Length < 1)
        {
            Debug.Log("sprite数组为0，请给sprite数组添加元素！！！");
        }
        manimg.sprite = sprit_arr[0];
        if (Play_Awake)
        {
            Play();
        }
    }
    void Update()
    {
        //测试
        if (Input.GetKeyDown(KeyCode.A))
        {
            Play();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Replay();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Stop();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Pause();
        }
        UpMove();

    }

    private void UpMove()
    {
        //单播
        if (condition == State1.once)
        {
            if (play_state == State.idle && isplay)
            {
                play_state = State.playing;
                index = 0;
                tim = 0;
            }
            if (play_state == State.pause && isplay)
            {
                play_state = State.playing;
                tim = 0;
            }
            if (play_state == State.playing && isplay)
            {
                tim += Time.deltaTime;
                if (tim >= waittim)
                {
                    tim = 0;
                    index++;
                    if (index >= sprit_arr.Length)
                    {
                        index = 0;
                        manimg.sprite = sprit_arr[index];
                        isplay = false;
                        play_state = State.idle;
                        //此处可添加结束回调函数
                        if (onCompleteEvent != null)
                        {
                            onCompleteEvent.Invoke();
                            return;
                        }
                    }
                    manimg.sprite = sprit_arr[index];
                }
            }
        }
        //循环播放
        if (condition == State1.loop)
        {
            if (play_state == State.idle && isplay)
            {
                play_state = State.playing;
                index = 0;
                tim = 0;
            }
            if (play_state == State.pause && isplay)
            {
                play_state = State.playing;
                tim = 0;
            }
            if (play_state == State.playing && isplay)
            {
                tim += Time.deltaTime;
                if (tim >= waittim)
                {
                    tim = 0;
                    index++;
                    if (index >= sprit_arr.Length)
                    {
                        index = 0;
                        //此处可添加结束回调函数
                    }
                    manimg.sprite = sprit_arr[index];
                }
            }
        }
    }
    /// <summary>
    /// 播放
    /// </summary>
    public void Play()
    {
        isplay = true;
    }
    /// <summary>
    /// 暂停
    /// </summary>
    public void Pause()
    {
        isplay = false;
        play_state = State.pause;
    }
    /// <summary>
    /// 停止
    /// </summary>
    public void Stop()
    {
        isplay = false;
        play_state = State.idle;
        index = 0;
        tim = 0;
        if (manimg == null)
        {
            Debug.Log("Image为空，请赋值");
            return;
        }
        manimg.sprite = sprit_arr[index];
    }
    /// <summary>
    /// 重播
    /// </summary>
    public void Replay()
    {
        isplay = true;
        play_state = State.playing;
        index = 0;
        tim = 0;
    }

}
