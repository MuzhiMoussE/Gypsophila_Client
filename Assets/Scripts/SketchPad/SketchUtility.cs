using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class SketchUtility : MonoBehaviour
{
    //ͼƬ����->sprite����
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

    [Header("���ŷ�ʽ(ѭ��������)")]//Ĭ�ϵ���
    public State1 condition = State1.once;
    [Header("�Զ�����")]//Ĭ�ϲ��Զ�����
    public bool Play_Awake = false;
    //����״̬(Ĭ�ϡ������С���ͣ)
    private State play_state;
    private Image manimg;
    [Header("ÿ�벥�ŵ�֡��(����)")]
    public float frame_number = 30;
    [Header("sprite����")]
    public Sprite[] sprit_arr;
    //�ص��¼�
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
            Debug.Log("ImageΪ�գ������Image���������");
            return;
        }
        if (sprit_arr.Length < 1)
        {
            Debug.Log("sprite����Ϊ0�����sprite�������Ԫ�أ�����");
        }
        manimg.sprite = sprit_arr[0];
        if (Play_Awake)
        {
            Play();
        }
    }
    void Update()
    {
        //����
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
        //����
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
                        //�˴�����ӽ����ص�����
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
        //ѭ������
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
                        //�˴�����ӽ����ص�����
                    }
                    manimg.sprite = sprit_arr[index];
                }
            }
        }
    }
    /// <summary>
    /// ����
    /// </summary>
    public void Play()
    {
        isplay = true;
    }
    /// <summary>
    /// ��ͣ
    /// </summary>
    public void Pause()
    {
        isplay = false;
        play_state = State.pause;
    }
    /// <summary>
    /// ֹͣ
    /// </summary>
    public void Stop()
    {
        isplay = false;
        play_state = State.idle;
        index = 0;
        tim = 0;
        if (manimg == null)
        {
            Debug.Log("ImageΪ�գ��븳ֵ");
            return;
        }
        manimg.sprite = sprit_arr[index];
    }
    /// <summary>
    /// �ز�
    /// </summary>
    public void Replay()
    {
        isplay = true;
        play_state = State.playing;
        index = 0;
        tim = 0;
    }

}
