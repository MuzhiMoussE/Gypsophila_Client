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
    public List<ActionPair> actionPairs = new List<ActionPair>();
    private int index_l = 0;
    private int index_r = 0;
    private int index_j = 0;
    private int direction = 0;
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
            //��¼����
            //�ƶ�
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
        
    }
    public void CopyActionToSkecthMan(GameObject sketchman)
    {
        if (sketchman == null) return;
        float _timer = 0f;
        int index = 0;
        
        //Ӧ��Э���������ﵽ�������ص�˼·
        while(_timer<StateSystem.Instance.recordingTime)
        {
            if(index>=actionPairs.Count) { break; }
            if (_timer - actionPairs[index].startTime < 0.01f)//ĳ�����Ŀ���ʱ�䣬һ����˳���
            {
                StartAction(sketchman, actionPairs[index]);
                index++;
            }
            sketchman.transform.position += new Vector3(direction * Time.deltaTime*StateSystem.Instance.moveSpeed,0,0);
            _timer += Time.deltaTime;
        }
    }
    IEnumerator stopAction(ActionPair action)
    {
        yield return  new WaitForSeconds(action.endTime);
        direction = 0;
        Debug.Log("OVER "+action.ToString());
    }
    private void StartAction(GameObject sketchman,ActionPair action)
    {
        Debug.Log("START "+action.ToString());
        //����ʹ��Э��
        if(action.action == InputAction.JUMPING)
        {
            //��Ծ
            sketchman.GetComponent<Rigidbody>().AddForce(new Vector3(0,StateSystem.Instance.jumpForce,0));
            
        }
        else
        {
            if (action.action == InputAction.MOVING_LEFT) direction = -1;
            else if (action.action == InputAction.MOVING_RIGHT) direction = 1;
            //��ʱֹͣ����direction��0��ʹ��Э��
            IEnumerator e = stopAction(action);
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
        temp = list[low];//temp���ǻ�׼λ
        while (i < j)
        {
            while (temp.startTime <= list[j].startTime && i < j)//�ȿ��ұߣ���������ݼ�
            {
                j--;
            }
            while (temp.startTime >= list[i].startTime && i < j) //�ٿ���ߣ��������ҵ���
            {
                i++;
            }
            //������������򽻻�
            if (i < j)
            {
                t = list[j];
                list[j] = list[i];
                list[i] = t;
            }
        }
        //��󽫻�׼Ϊ��i��j���λ�õ����ֽ���
        list[low] = list[i];
        list[i] = temp;
        QuickSortByStartTime(list, low, j - 1);//�ݹ�����������
        QuickSortByStartTime(list, j + 1, high);//�ݹ�����Ұ�����
    }



}
