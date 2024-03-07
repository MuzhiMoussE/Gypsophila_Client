using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Global;
using UnityEngine.UI;
using Utility;
using UnityEngine.SceneManagement;
using static UnityEditorInternal.VersionControl.ListControl;

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
    private int index_l = 0;
    private int index_r = 0;
    private int index_j = 0;
    private bool startRecording = false;
    private float time0 = 0;
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
    public void ReleasingAction()
    {
        foreach (var pair in actionPairs)
        {
            Debug.Log(pair.ToString());
        }
        actionPairs.Clear();
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
                actionPairs.Add(new ActionPair(InputAction.MOVING_LEFT, pair.downTime, pair.upTime));
            }
        }
        if(movingRight.Count>0)
        {
            foreach (var pair in movingRight)
            {
                actionPairs.Add(new ActionPair(InputAction.MOVING_RIGHT, pair.downTime, pair.upTime));
            }
        }
        
        if(jumping.Count>0)
        {
            foreach (var pair in jumping)
            {
                actionPairs.Add(new ActionPair(InputAction.JUMPING, pair.downTime, pair.upTime));
            }
        }
        
    }
    public void CopyActionToSkecthMan(GameObject sketchman)
    {
        if (sketchman == null) return;
        float _timer = 0f;
        
        //应该协程来做，达到条件返回的思路


    }
    
    

}
