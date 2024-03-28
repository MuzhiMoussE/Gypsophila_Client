using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PencilTraps : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == Global.ItemTag.PLAYER)
        {
            other.GetComponent<Player>().PlayerDead();
        }
    }
}
