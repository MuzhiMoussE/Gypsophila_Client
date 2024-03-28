using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGroundListener : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != Global.ItemTag.PLAYER)
        {
            StateSystem.Instance.onGround = true;
            if(!StateSystem.Instance.isDead) StateSystem.Instance.playerState = Global.PlayerState.Idle;
            AnimSystem.Instance.ChangeAnimState(StateSystem.Instance.playerState);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != Global.ItemTag.PLAYER)
        {
            StateSystem.Instance.onGround = false;
            //StateSystem.Instance.playerState = Global.PlayerState.Jumping;
            //AnimSystem.Instance.ChangeAnimState(StateSystem.Instance.playerState);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag != Global.ItemTag.PLAYER)
        {
            StateSystem.Instance.onGround = true ;
            //StateSystem.Instance.playerState = Global.PlayerState.Idle;
            //AnimSystem.Instance.ChangeAnimState(StateSystem.Instance.playerState);
            return;
        }
    }
}
