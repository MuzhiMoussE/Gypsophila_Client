using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Hand : MonoBehaviour
{
    public GameObject player;
    [SerializeField] private float moveSpeed;
    private Animator animator;
    private bool canMove = false;
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
        if(canMove) {
            gameObject.transform.position += new Vector3(moveSpeed * Time.deltaTime, 0, 0);
        }
        
    }
    public void HandDiew()
    {
        animator.SetBool("die", true);
    }
}
