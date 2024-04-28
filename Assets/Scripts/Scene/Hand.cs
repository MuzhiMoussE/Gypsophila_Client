using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Rendering;

public class Hand : MonoBehaviour
{
    public bool isStatic = false;
    public GameObject player;
    [SerializeField] private float moveSpeed = 8f;
    private Animator animator;
    public bool canMove = false;
    [SerializeField] private GameObject handmove;
    // Start is called before the first frame update
    void OnEnable()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }
    private enum State
    {
        Idle,
        Move,
        Attack1,
        Attack2,
        Die
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == Global.ItemTag.PLAYER)
        {
            PlayerEnter();
        }
        if(other.tag == Global.ItemTag.SKETCH_MAN)
        {
            SketchEnter();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == Global.ItemTag.PLAYER)
        {
            PlayerExit();
        }
    }
    private void PlayerExit()
    {
        animator.SetBool("attack1", false);
        animator.SetBool("attack2", false);
    }
    private void SketchEnter()
    {
        HandDie();
    }
    private void PlayerEnter()
    {
        int num = Random.Range(0, 2);
        if (num == 0) animator.SetBool("attack1", true);
        else animator.SetBool("attack2", true);
        canMove = true;
        animator.SetBool("move", true);
    }
    private void Move()
    {
        if(canMove && !isStatic) {
            gameObject.transform.position += new Vector3(moveSpeed * Time.deltaTime, 0, 0);
            //Debug.Log(moveSpeed);
        }
        
    }
    public void HandDie()
    {
        animator.SetBool("die", true);
        handmove.GetComponent<Animator>().SetBool("Play2", true);
    }
}
