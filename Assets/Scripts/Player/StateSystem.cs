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
    public Slider recordSlider;
    public Slider recordingTimeSlider;
    public float recordingTime = 5f;//记录存取时间
    public float moveSpeed;
    public float jumpForce;
    private float jumpMaxTime = 0.2f;
    private float jumpTime = 0;
    private bool canJump = true;
    public bool onGround = true;

    public float sketchmanDistance = 5.0f;

    private float timer = 0f;
    private float intervalTime = 0.5f;//大于这个时间判定为长按
    private float recordTime = 1f;//按满1s触发长按事件
    private bool isRecorded = false;
    private bool longPress = false;
    private int direction = 0;
    private bool interactTrigger = false;
    [SerializeField]private GameObject interactObject = null;
    private bool getBox = false;
    public void InputListener(GameObject player,Rigidbody player_rd,GameObject sketchman)
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            //暂停
            Time.timeScale = 0;
            
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
            playerState = Global.PlayerState.ToSummon;
            ToSummon();
        }
        //跳跃
        JumpFunction(player);
        //移动
        MoveFunction(player);

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
    public void Releasing(GameObject sketchman)
    {
        playerState = Global.PlayerState.Releasing;
        //SketchSystem.Instance.CopyActionToSkecthMan(sketchman);
        SketchSystem.Instance.ReleasingAction(sketchman);
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
    public void InteractTriggerEnter(Collider other)
    {
        interactTrigger = true;
        interactObject = other.gameObject;
    }
    public void InteractTriggerExit() 
    {
        interactTrigger = false; 
        interactObject = null;
    }
    private void LongOrShortPressFunction()
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
    private void InteractFunction()
    {
        if (interactObject.tag == Global.ItemTag.BOX && !getBox)
        {
            getBox = true;
            Debug.Log("GET BOX!");
            interactObject.gameObject.GetComponent<Boxes>().dragged = true;
            canJump = false;
            
        }
        else if (interactObject.tag == Global.ItemTag.BOX && getBox)
        {
            getBox = false;
            Debug.Log("THROW BOX!");
            interactObject.gameObject.GetComponent<Boxes>().dragged = false;
            Instance.playerState = Global.PlayerState.Idle;
            canJump = true;
        }
    }
    private void ReleaseFunction(GameObject sketchman,GameObject player)
    {
        Releasing(sketchman);
        sketchman.transform.position = player.transform.position + new Vector3(sketchmanDistance, 0, 0) * direction;
        sketchman.SetActive(true);
    }
    private void ShortPressFunction(GameObject sketchman,GameObject player)
    {
        if (timer < intervalTime)
        {
            timer = 0f;//复位
            recordSlider.value = 0;
            if (interactTrigger) InteractFunction();
            else if (isRecorded) ReleaseFunction(sketchman, player);
            else
            {
                //Debug.Log("无记录！");
            }
            playerState = Global.PlayerState.Idle;
        }
    }
    private void MoveLeftFunction(GameObject player)
    {
        direction = -1;
        playerState = Global.PlayerState.Moving;
        player.transform.position -= new Vector3(moveSpeed * Time.fixedDeltaTime, 0, 0);
        PlayerRotate(player);
    }
    private void MoveRightFunction(GameObject player)
    {
        direction = 1;
        playerState = Global.PlayerState.Moving;
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
            if (!onGround) playerState = Global.PlayerState.Jumping;
            if(getBox) playerState = Global.PlayerState.Dragging;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            MoveRightFunction(player);
            
            if (!onGround) playerState = Global.PlayerState.Jumping;
            if (getBox) playerState = Global.PlayerState.Dragging;
        }
    }
}
