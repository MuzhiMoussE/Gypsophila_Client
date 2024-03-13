using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Global;
using UnityEngine.UI;
using Utility;
using UnityEngine.SceneManagement;
using static UnityEditorInternal.VersionControl.ListControl;
using System;
using Unity.VisualScripting;

public class SketchSystem:SingletonMonoBase<SketchSystem>
{
    private class KeyPair
    {
        public float downTime = 0;
        public float upTime = 0;
        public KeyPair(float downTime, float upTime)
        {
            this.downTime = downTime;
            this.upTime = upTime;
        }
        public KeyPair() { 
            this.downTime = 0;
            this.upTime = 0;
        }
        public void SetStart(float startTime)
        {
            downTime = startTime;
        }
        public void SetEnd(float endTime)
        {
            upTime = endTime;
        }

    }

    private List<KeyPair> movingLeft = new List<KeyPair>();
    private List<KeyPair> movingRight = new List<KeyPair>();
    private List<KeyPair> jumping = new List<KeyPair>() ;
    private List<ActionPair> actionPairs = new List<ActionPair>();
    public Queue<ActionPair> actionQueue = new Queue<ActionPair>();
    private int index_l = 0;
    private int index_r = 0;
    private int index_j = 0;
    private int direction = 0;
    private bool startRecording = false;
    private float time0 = 0;
    private float _time0 = 0;
    private bool startReleased = false;
    private bool canRelease = false;

    private void Update()
    {
        RecordingAction();
    }
    public void StartRecordingAction()
    {
        startRecording = true;
        time0 = Time.time;
    }
    public void StopRecordingAction()
    {
        startRecording = false;
        StoreAction();
    }
    public void ReleasingAction(GameObject sketchman)
    {
        canRelease = true;
        if(actionQueue.Count == 0)//什么都没有记录
        {
            IEnumerator e = noneAction(sketchman);
            StartCoroutine(e);
        }
        //foreach(var pair in actionQueue)
        //{
        //    Debug.Log("RECORDED "+pair.ToString());
        //}
        //Debug.Log(actionQueue.Count+" actions recorded!");
    }
    private void RecordingAction()
    {
        if(!startRecording)
        {
            return;
        }
        else
        {  
            //记录输入
            //移动
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                movingLeft.Add(new KeyPair(Time.time-time0,0f));

            }
            if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow))
            {
                movingLeft[index_l].SetEnd(Time.time-time0);
                index_l++;

            }
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                //Debug.Log(movingRight.Count);
                movingRight.Add(new KeyPair(Time.time - time0, 0f));
            }
            if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
            {
                movingRight[index_r].SetEnd(Time.time - time0);
                index_r++;
            }
            if(Input.GetKeyDown(KeyCode.Space))
            {
                jumping.Add(new KeyPair());
                jumping[index_j].SetStart(Time.time - time0);
            }
            if(Input.GetKeyUp(KeyCode.Space))
            {
                jumping[index_j].SetEnd(Time.time - time0);
                index_j++;
            }
        }
    }
    private void StoreAction()
    {
        if(movingLeft.Count>0)
        {
            foreach (var pair in movingLeft)
            {
                if (pair.upTime < 0.001f) pair.upTime = StateSystem.Instance.recordingTime;
                actionPairs.Add(new ActionPair(InputAction.MOVING_LEFT, pair.downTime, pair.upTime));
            }
        }
        if(movingRight.Count>0)
        {
            foreach (var pair in movingRight)
            {
                if (pair.upTime < 0.001f) pair.upTime = StateSystem.Instance.recordingTime;
                actionPairs.Add(new ActionPair(InputAction.MOVING_RIGHT, pair.downTime, pair.upTime));
            }
        }
        
        if(jumping.Count>0)
        {
            foreach (var pair in jumping)
            {
                if (pair.upTime < 0.001f)
                    pair.upTime = StateSystem.Instance.recordingTime;
                actionPairs.Add(new ActionPair(InputAction.JUMPING, pair.downTime, pair.upTime));
            }
        }
        QuickSortByStartTime(actionPairs,0,actionPairs.Count-1);
        for(int i=0;i<actionPairs.Count-1;i++)
        {
            //上一次的抬起晚于下一次的按下，让上一次的抬起时间略小于下一次的按下时间
            if (actionPairs[i].endTime > actionPairs[i + 1].startTime)
                actionPairs[i].endTime -= (actionPairs[i].endTime - actionPairs[i + 1].startTime)*2;
            
        }
        foreach(var pair in actionPairs)
        {
            actionQueue.Enqueue(pair);
        }
        actionPairs.Clear();
        movingLeft.Clear();
        movingRight.Clear();
        jumping.Clear();
        index_l = 0;
        index_r = 0;
        index_j = 0;
}
    public void CopyActionToSkecthMan(GameObject sketchman)//update中使用
    {
        if (!canRelease) return;
        else
        {
            sketchman.transform.position += new Vector3(direction * Time.fixedDeltaTime * StateSystem.Instance.moveSpeed, 0, 0);
            //Debug.Log("释放！");
            if (!startReleased)
            {
                startReleased = true;
                _time0 = Time.time;
                
            }
            else
            {
                if (actionQueue.Count>0 && Math.Abs(Time.time - _time0) > actionQueue.Peek().startTime)
                {
                    ActionPair temp = actionQueue.Dequeue();
                    StartAction(sketchman, temp);
                }
                    
            }   
        }   
    }
    //用Queue先进先出！！！！！！！！！！！！！！！
    IEnumerator noneAction(GameObject sketchman)
    {
        yield return new WaitForSeconds(StateSystem.Instance.recordingTime);
        sketchman.SetActive(false);
    }
    IEnumerator stopAction(ActionPair action,GameObject sketchman)
    {
        yield return  new WaitForSeconds(action.endTime-action.startTime);
        direction = 0;
        //Debug.Log("OVER " + action.ToString()+" TIME "+Time.time);
        if(actionQueue.Count == 0)
        {
            canRelease = false;
            startReleased = false;
            _time0 = 0;
            sketchman.SetActive(false);
        }

    }
    private void StartAction(GameObject sketchman,ActionPair action)
    {
        //Debug.Log("START " + action.ToString() + " TIME " + Time.time);
        //这里使用协程
        if (action.action == InputAction.JUMPING)
        {
            //跳跃
            sketchman.GetComponent<Rigidbody>().AddForce(new Vector3(0, StateSystem.Instance.jumpForce, 0));
        }
        else
        {
            if (action.action == InputAction.MOVING_LEFT) direction = -1;
            else if (action.action == InputAction.MOVING_RIGHT) direction = 1;
            //何时停止，将direction置0，使用协程
            IEnumerator e = stopAction(action,sketchman);
            StartCoroutine(e);
        }

    }
    private void QuickSortByStartTime(List<ActionPair> list,int low,int high)
    {
        int i, j;
        ActionPair temp = new ActionPair(), t = new ActionPair();
        if (low > high)
        {
            return;
        }
        i = low;
        j = high;
        temp = list[low];//temp就是基准位
        while (i < j)
        {
            while (temp.startTime <= list[j].startTime && i < j)//先看右边，依次往左递减
            {
                j--;
            }
            while (temp.startTime >= list[i].startTime && i < j) //再看左边，依次往右递增
            {
                i++;
            }
            //如果满足条件则交换
            if (i < j)
            {
                t = list[j];
                list[j] = list[i];
                list[i] = t;
            }
        }
        //最后将基准为与i和j相等位置的数字交换
        list[low] = list[i];
        list[i] = temp;
        QuickSortByStartTime(list, low, j - 1);//递归调用左半数组
        QuickSortByStartTime(list, j + 1, high);//递归调用右半数组
    }



}
