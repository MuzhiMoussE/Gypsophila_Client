using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mom : MonoBehaviour
{
    public GameObject player;
    public  bool canWalk = true;
    public bool isWalking = false;
    [SerializeField] private float walkSpeed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(canWalk)
        {
            if(isWalking)
            {
                gameObject.transform.position += new Vector3(walkSpeed * Time.deltaTime, 0, 0);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == Global.ItemTag.PLAYER)
        {
            this.GetComponent<Animator>().SetBool("mom_walking", true);
            isWalking = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == Global.ItemTag.PLAYER)
        {
            this.GetComponent<Animator>().SetBool("mom_walking", false);
            isWalking = false;
        }
    }
}
