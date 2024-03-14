using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boxes : MonoBehaviour
{
    public bool playerNear = false;
    public bool dragged = false;
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        PlayerGetListener();
    }

    private void PlayerGetListener()
    {
        if (!playerNear) return;
        if (dragged && StateSystem.Instance.playerState != Global.PlayerState.Dragging)
        {
            this.gameObject.transform.SetParent(player.transform);
            StateSystem.Instance.playerState = Global.PlayerState.Dragging;
            //Debug.Log("GET!!!");
            return;
        }
        else if (!dragged && gameObject.transform.parent != null)
        {
            this.gameObject.transform.parent = null;
            return;
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == Global.ItemTag.UNTAGGED) return;

        if (collision.gameObject.tag == Global.ItemTag.PLAYER)
        {
            playerNear = true;
            player = collision.gameObject;
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.tag == Global.ItemTag.UNTAGGED) return;

        if (collision.gameObject.tag == Global.ItemTag.PLAYER)
        {
            playerNear = false;
            player = collision.gameObject;
        }
    }
}
