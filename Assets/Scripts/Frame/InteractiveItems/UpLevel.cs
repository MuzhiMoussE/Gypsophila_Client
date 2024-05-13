using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpLevel : MonoBehaviour
{
    public GameObject player;
    public bool onlyOnce = false;

    public void PlayerFollow()
    {
        player.transform.parent = transform;
    }
    public void PlayerMoveExitLevel()
    {
        player.transform.parent = null;
        player.transform.position += new Vector3(0, 1, 0);
    }
    public void PlayerMove()
    {
        if(onlyOnce)
        {
            player.transform.parent = null;
            player.transform.position += new Vector3(0, 1, 0);
            onlyOnce = false;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
