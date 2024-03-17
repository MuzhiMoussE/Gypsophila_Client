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
    public float recordingTime = 5f;//��¼��ȡʱ��
    public float moveSpeed = 4f;
    public float jumpForce;
    private float jumpMaxTime = 0.2f;
    private float jumpTime = 0;
    private bool canJump = true;
    public bool onGround = true;
    private bool isDead = false;
    public float sketchmanDistance = 5.0f;

    private float timer = 0f;
    private float intervalTime = 0.5f;//�������ʱ���ж�Ϊ����
    private float recordTime = 1f;//����1s���������¼�
    private bool isRecorded = false;
    private bool longPress = false;
    private int direction = 0;
    [SerializeField]private bool interactTrigger = false;
    private Stack<GameObject> interactStack = new Stack<GameObject>();
    [SerializeField]private GameObject interactObject = null;
    private bool getBox = false;
    private GameObject box = null;
    public void InputListener(GameObject player,Rigidbody player_rd,GameObject sketchman)
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            //��ͣ
            Time.timeScale = 0;
            
        }
        else if(Input.GetKeyDown(KeyCode.Q))
        {
            RotateReleaser(false);
        }
        //�̰��ͷţ�������¼����
        else if (Input.GetKey(KeyCode.E))
        {
            LongOrShortPressFunction();
        }
        else if(Input.GetKeyUp(KeyCode.E))//���ð���̧���ж��ǲ��Ƕ̰�
        {
            if(!longPress)
            {
                ShortPressFunction(sketchman, player);
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
            ToSummon();
        }
        //��Ծ
        JumpFunction(player);
        //�ƶ�
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
        playerState = Global.PlayerState.ToSummon;
        AnimSystem.Instance.ChangeAnimState(playerState);
        Debug.Log("�ٻ���ת��");
        IEnumerator e = ToIdle(5);
        StartCoroutine(e);

    }
    public void Recording()
    {
        playerState = Global.PlayerState.Recording;
        AnimSystem.Instance.ChangeAnimState(playerState);
        SketchSystem.Instance.StartRecordingAction();
        recordingTimeSlider.GameObject().SetActive(true);
        Debug.Log("��ʼ��¼��");
        timer = 0;//��λ
        recordSlider.value = 0;
        IEnumerator coroutine = Recorder();
        StartCoroutine(coroutine);
    }
    public void Releasing(GameObject sketchman)
    {
        playerState = Global.PlayerState.Releasing;
        AnimSystem.Instance.ChangeAnimState(playerState);
        Debug.Log("RELEASING");
        //�ͷ�ֽ��ʵ��
        IEnumerator e = ToIdle(1);
        StartCoroutine(e);
        //SketchSystem.Instance.CopyActionToSkecthMan(sketchman);
        SketchSystem.Instance.ReleasingAction(sketchman);
        isRecorded = false;
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
        AnimSystem.Instance.ChangeAnimState(playerState);
        recordingTimeSlider.value = 0;
        recordingTimeSlider.GameObject().SetActive(false);
    }
    //�����Ƚ������Ӧ����ջ
    public void InteractTriggerEnter(Collider other)
    {
        interactTrigger = true;
        if (other.tag == Global.ItemTag.BOX)
        {
            box = other.gameObject;
        }
        else
        {
            interactStack.Push(other.gameObject);
            interactObject = interactStack.Peek();
        }

    }
    public void InteractTriggerExit(Collider other) 
    {
        if(other.gameObject.tag == Global.ItemTag.BOX)
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
                interactObject = interactStack.Peek();
            }
        }
    }
    private void LongOrShortPressFunction()
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
    private void RotateReleaser(bool isClockwise)
    {
        playerState = Global.PlayerState.Interacting;
        AnimSystem.Instance.ChangeAnimState(playerState);
        IEnumerator e = ToIdle(1);
        StartCoroutine(e);
        SightSwitch sightSwitch = interactObject.GetComponent<SightSwitch>();
        sightSwitch.Interact(isClockwise) ;
    }
    private void InteractFunction()
    {
        if (box!=null && !getBox)
        {
            getBox = true;
            Debug.Log("GET BOX!");
            box.gameObject.GetComponent<Boxes>().dragged = true;
            box.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            box.gameObject.transform.position += new Vector3(0, 0.5f, 0);
            canJump = false;
            
        }
        else if (box!=null && getBox)
        {
            getBox = false;
            Debug.Log("THROW BOX!");
            box.gameObject.GetComponent<Boxes>().dragged = false;
            box.gameObject.GetComponent<Rigidbody>().isKinematic=false;
            Instance.playerState = Global.PlayerState.Idle;
            canJump = true;
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
        Releasing(sketchman);
        sketchman.transform.position = player.transform.position + new Vector3(sketchmanDistance, 0, 0) * direction;
        sketchman.transform.position += new Vector3(0, 2, 0);
        sketchman.SetActive(true);
    }
    private void ShortPressFunction(GameObject sketchman,GameObject player)
    {
        if (timer < intervalTime)
        {
            timer = 0f;//��λ
            recordSlider.value = 0;
            if (interactTrigger) InteractFunction();
            else if (isRecorded) ReleaseFunction(sketchman, player);
            else
            {
                //Debug.Log("�޼�¼��");
            }
            //playerState = Global.PlayerState.Idle;
            //AnimSystem.Instance.ChangeAnimState(playerState);
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
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
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
         if(player.transform.position.y < -30)
         {
            isDead = true;
         }



         if(isDead)
         {
            DeadEvent();
         }
    }
    public void DeadEvent()
    {
        playerState = Global.PlayerState.Die;
        AnimSystem.Instance.ChangeAnimState(playerState);
        IEnumerator e = GameOver();
        StartCoroutine(e);
        
    }
    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(5);
        Time.timeScale = 0;
        Debug.Log("Dead!");
        Application.Quit();
    }
    IEnumerator ToIdle(float _second)
    {
        yield return new WaitForSeconds(_second);
        playerState = Global.PlayerState.Idle;
        AnimSystem.Instance.ChangeAnimState(playerState);
    }
}
