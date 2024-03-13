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
            StateSystem.Instance.playerState = Global.PlayerState.Idle;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != Global.ItemTag.PLAYER)
        {
            StateSystem.Instance.onGround = false;
            StateSystem.Instance.playerState = Global.PlayerState.Jumping;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag != Global.ItemTag.PLAYER)
        {
            StateSystem.Instance.onGround = true ;
            StateSystem.Instance.playerState = Global.PlayerState.Idle;
            return;
        }
    }
}
