using Cinemachine;
using Frame;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utility;
using static Global;

public class StateSystem : SingletonMonoBase<StateSystem>
{
    public Global.PlayerState playerState = Global.PlayerState.Idle;
    public Slider recordSlider;
    public Slider recordingTimeSlider;
    public Text tipsText;
    public bool showTips;
    public float recordingTime = 5f;//记录存取时间
    public float moveSpeed = 6f;
    public float jumpForce;
    private float jumpMaxTime = 0.2f;
    private float jumpTime = 0;
    private bool canJump = true;
    public bool onGround = true;
    public float sketchmanDistance = 5.0f;
    public bool isDead = false;
    private float timer = 0f;
    private float intervalTime = 0.5f;//大于这个时间判定为长按
    private float recordTime = 1f;//按满1s触发长按事件
    [SerializeField]private bool longPress = false;
    private int direction = 0;
    private bool interactTrigger = false;
    private Stack<GameObject> interactStack = new Stack<GameObject>();
    public  GameObject interactObject = null;
    private bool getBox = false;
    private GameObject box = null;
    public void InputListener(GameObject player,Rigidbody player_rd,GameObject sketchman)
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            //暂停
            Time.timeScale = 0;
        }
        else if(Input.GetKeyDown(KeyCode.Q))
        {
            RotateReleaser(false);
        }
        //短按释放，长按记录动作
        else if (Input.GetKey(KeyCode.E))
        {
            LongOrShortPressFunction(); 
        }
        else if(Input.GetKeyUp(KeyCode.E))//利用按键抬手判断是不是短按
        {
            
            if(!longPress)
            {
                //Debug.Log("NOT LONG PRESS!");
                ShortPressFunction(sketchman, player);
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
            ToSummon();
        }
        //跳跃
        JumpFunction(player);
        //移动
        MoveFunction(player);
        CheckDead(player);

    }
    private void PlayerRotate(GameObject player)
    {
        if(Input.GetAxis("Horizontal")>0)
        {
            player.transform.rotation = Quaternion.LookRotation(new Vector3(0, 0, 90));
        }
        else
        {
            player.transform.rotation = Quaternion.LookRotation(new Vector3(0, 0, -90));
        }
    }
    public void ToSummon()
    {
        if (interactObject == null) return;
        if (interactObject.tag == Global.ItemTag.PAINTER && !ArchiveSystem.painted)
        {
            
            playerState = Global.PlayerState.ToSummon;
            AnimSystem.Instance.ChangeAnimState(playerState);
           // Debug.Log("召唤跳转！");
            ArchiveSystem.painted = true;
            IEnumerator e = ToIdle(5);
            StartCoroutine(e);
            interactObject.GetComponent<Painter>().paintPlane.GetComponentInChildren<CinemachineVirtualCamera>().Priority = 20;
            //paintPlane.SetActive(true);
            interactObject.GetComponent<Painter>().paintPlane.GetComponent<Sketch>().Init();
            interactObject.GetComponent<Painter>().paintPlane.GetComponent<SketchReceiver>().paintUI.SetActive(true);
            //SceneManager.LoadScene(4);
            //StartCoroutine(LoadScene(4));


        }
    }
    IEnumerator LoadScene(int index)
    {
        //获取加载对象
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(index);
        //设置加载完成后不跳转
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            //输出加载进度
            Debug.Log(asyncLoad.progress);
            //进度.百分之九十后进行操作,当进度为百分之90其实已经完成了大部分的工作,就可以进行下面的逻辑处理了
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }
    public void Recording()
    {
        playerState = Global.PlayerState.Recording;
        AnimSystem.Instance.ChangeAnimState(playerState);
        SketchSystem.Instance.StartRecordingAction();
        recordingTimeSlider.GameObject().SetActive(true);
        //Debug.Log("开始记录！");
        timer = 0;//复位
        recordSlider.value = 0;
        IEnumerator coroutine = Recorder();
        StartCoroutine(coroutine);
    }
    public void Releasing(GameObject sketchman)
    {
        playerState = Global.PlayerState.Releasing;
        AnimSystem.Instance.ChangeAnimState(playerState);
        //Debug.Log("RELEASING");
        //SketchSystem.Instance.CopyActionToSkecthMan(sketchman);
        SketchSystem.Instance.ReleasingAction(sketchman);
        //释放纸人实体
        StartCoroutine(ToIdle(1));

        ArchiveSystem.isRecorded = false;
        ArchiveSystem.painted = false;
        ArchiveSystem.sketchCnt++;

    }
    IEnumerator Recorder()
    {
        while(recordingTimeSlider.value < 1f)
        {
            recordingTimeSlider.value += recordingTime / 50f; 
            yield return new WaitForSeconds(recordingTime/10f);
        }
        ArchiveSystem.isRecorded = true;
        longPress = false;
        SketchSystem.Instance.StopRecordingAction();
        //Debug.Log("记录完成！");
        playerState = Global.PlayerState.Idle;
        AnimSystem.Instance.ChangeAnimState(playerState);
        recordingTimeSlider.value = 0;
        recordingTimeSlider.GameObject().SetActive(false);
    }
    //这里先进后出，应该用栈
    private void ShowTips(GameObject _object)
    {
        tipsText.gameObject.SetActive(true);
        if(_object.tag == Global.ItemTag.BOX)
        {
            tipsText.text = "按下E键拿起/放下";
        }
        else if(_object.tag == Global.ItemTag.SIGHT_SWITCH)
        {
            tipsText.text = "按下E键/Q键转动";
        }
        else if (_object.tag == Global.ItemTag.PAINTER)
        {
            tipsText.text = "按下S键使用";
        }
        else
        {
            tipsText.text = "";
        }
    }
    private void CloseTips()
    {
        tipsText.gameObject.SetActive(false);
    }
    public void InteractTriggerEnter(Collider other)
    {
        interactTrigger = true;
        if (other.tag == Global.ItemTag.BOX)
        {
            box = other.gameObject;
            if (showTips) ShowTips(box);
        }
        else
        {
            interactStack.Push(other.gameObject);
            interactObject = interactStack.Peek();
            if (showTips) ShowTips(interactObject);
        }

    }
    public void InteractTriggerExit(Collider other) 
    {
        CloseTips();
        if (other.gameObject.tag == Global.ItemTag.BOX && !getBox)
        {
            box = null;
            if(interactStack.Count == 0) interactTrigger = false;
        }
        else
        {
            if (interactStack.Count <= 1)
            {
                interactTrigger = false;
                interactObject = null;
                interactStack.Clear();
                if(box!= null) interactTrigger =true;
            }
            else if (interactStack.Contains(other.gameObject))
            {
                interactStack.Pop();
                interactObject = interactStack.Peek();
                //Debug.Log("interact now:" + interactObject.name);
            }
        }
    }
    private void LongOrShortPressFunction()
    {
        timer += Time.deltaTime;
        //此处只检测长按逻辑
        if (timer >= intervalTime)//超过间隔时间也就是长按
        {
            longPress = true;
            //Debug.Log("LONG PRESS!");
            if (timer >= recordTime)//开启记录
            {
                Recording();
            }
            else//未开启，进度条++
            {
                recordSlider.value = timer / recordTime;
            }
        }
    }
    private void RotateReleaser(bool isClockwise)
    {
        playerState = Global.PlayerState.Interacting;
        AnimSystem.Instance.ChangeAnimState(playerState);
        IEnumerator e = ToIdle(1);
        StartCoroutine(e);
        if(interactObject.tag == Global.ItemTag.SIGHT_SWITCH)
        {
            SightSwitch sightSwitch = interactObject.GetComponent<SightSwitch>();
            sightSwitch.Interact(isClockwise);
        }

    }
    private void InteractFunction()
    {
        if (box!=null && !getBox)
        {
            getBox = true;
            //Debug.Log("GET BOX!");
            box.gameObject.GetComponent<Boxes>().dragged = true;
            box.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            //box.gameObject.transform.position += new Vector3(0, 0.3f, 0);
            canJump = false;
            showTips = false;
        }
        else if (box!=null && getBox)
        {
            getBox = false;
            //Debug.Log("THROW BOX!");
            box.gameObject.GetComponent<Boxes>().dragged = false;
            box.gameObject.GetComponent<Rigidbody>().isKinematic=false;
            playerState = Global.PlayerState.Idle;
            canJump = true;
            showTips = true;
        }
        if(interactObject != null)
        {
            if (interactObject.tag == Global.ItemTag.SIGHT_SWITCH)
            {
                RotateReleaser(true);

            }
        }

    }
    private void ReleaseFunction(GameObject sketchman,GameObject player)
    {
        //Debug.Log("Start Releasing!");
        Releasing(sketchman);
        sketchman.transform.position = player.transform.position + new Vector3(sketchmanDistance, 0, 0) * direction;
        sketchman.transform.position += new Vector3(0, 2, 0);
        sketchman.SetActive(true);
    }
    private void ShortPressFunction(GameObject sketchman,GameObject player)
    {
        timer = 0f;//复位
        recordSlider.value = 0;
        if (interactTrigger) InteractFunction();
        else
        {
            //Debug.Log("ISRECORDED:" + ArchiveSystem.isRecorded + " ISPAINTED:" + ArchiveSystem.painted);
            if (ArchiveSystem.isRecorded && ArchiveSystem.painted)
            {
                Debug.Log("Start Releasing!");
                ReleaseFunction(sketchman, player);
            }
            else
            {
                Debug.Log("无记录或未绘画！");
            }
        }
    }
    private void MoveLeftFunction(GameObject player)
    {
        direction = -1;
        if (!getBox)
        {
            playerState = Global.PlayerState.Moving;
            moveSpeed = 4f;
        }
        else
        {
            playerState = Global.PlayerState.Dragging;
            moveSpeed = 2f;
        }
        AnimSystem.Instance.ChangeAnimState(playerState);
        player.transform.position -= new Vector3(moveSpeed * Time.fixedDeltaTime, 0, 0);
        PlayerRotate(player);
    }
    private void MoveRightFunction(GameObject player)
    {
        direction = 1;
        if(!getBox)
        {
            playerState = Global.PlayerState.Moving;
            moveSpeed = 4f;
        }
        else
        {
            playerState = Global.PlayerState.Dragging;
            moveSpeed = 2f; 
        }
        AnimSystem.Instance.ChangeAnimState(playerState);
        player.transform.position += new Vector3(moveSpeed * Time.fixedDeltaTime, 0, 0);
        PlayerRotate(player);
    }
    private void JumpFunction(GameObject player)
    {
        if (Input.GetKey(KeyCode.Space))
        {
            
            if (jumpTime < jumpMaxTime && canJump)
            {
                playerState = Global.PlayerState.Jumping;
                AnimSystem.Instance.ChangeAnimState(playerState);
                player.transform.position += new Vector3(0, jumpForce * Time.deltaTime, 0);
                jumpTime += Time.deltaTime;
            }
            else
            {
                jumpTime = 0;
                canJump = false;

            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpTime = 0;
            
            if (onGround && !getBox)
            {
                canJump = true;
                playerState = Global.PlayerState.Jumping;
                AnimSystem.Instance.ChangeAnimState(playerState);
            }
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (!onGround)
            {
                canJump = false;
                //playerState = Global.PlayerState.Jumping;
            }
        }
    }
    private void MoveFunction(GameObject player)
    {

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            MoveLeftFunction(player);
            OnMoingFunction();
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            MoveRightFunction(player);
            OnMoingFunction();
        }

        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            playerState = Global.PlayerState.Idle;
            AnimSystem.Instance.ChangeAnimState(playerState);
        }
    }
    private void OnMoingFunction()
    {
        if (!onGround) playerState = Global.PlayerState.Jumping;
        if (getBox) playerState = Global.PlayerState.Dragging;
        AnimSystem.Instance.ChangeAnimState(playerState);
    }
    public void CheckDead(GameObject player)
    {
         if(player.transform.position.y < -30 && !isDead)
         {
            DeadEvent(player);
         }
    }
    public void DeadEvent(GameObject player)
    {
        isDead = true;  
        StartCoroutine(GameOver(player));
        
    }
    IEnumerator GameOver(GameObject player)
    {
        playerState = Global.PlayerState.Die;
        //Debug.Log(playerState);
        AnimSystem.Instance.ChangeAnimState(playerState);
        ArchiveSystem.restart = false;
        yield return new WaitForSeconds(1);
        //Time.timeScale = 0;
        //sssDebug.Log("Dead!");
        //Application.Quit();
        //Time.timeScale = 1;
        ArchiveSystem.LoadArchive(ArchiveSystem.LevelIndex, player.transform);
        isDead = false;
        player.layer = 6;
        yield return null;
    }
    IEnumerator ToIdle(float _second)
    {
        yield return new WaitForSeconds(_second);
        playerState = Global.PlayerState.Idle;
        AnimSystem.Instance.ChangeAnimState(playerState);
    }
}
