using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class Player : MonoBehaviour
{
    private GameObject _player;
    private Rigidbody _rigidbody;
    [SerializeField]private GameObject _sketchman;
    private void Awake()
    {
        _player = gameObject;
       
        _rigidbody = gameObject.GetComponent<Rigidbody>();
    }

    private void Start()
    {
        AnimSystem.Instance.setInit(this.GetComponentInChildren<Animator>());
    }
    private void Update()
    {
        StateSystem.Instance.InputListener(_player, _rigidbody, _sketchman);
    }

    private void FixedUpdate()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == Global.ItemTag.SIGHT_SWITCH || 
            other.tag == Global.ItemTag.BOX || 
            other.tag == Global.ItemTag.LEVEL_SWITCH || 
            other.tag == Global.ItemTag.PAINTER)
        {
            StateSystem.Instance.InteractTriggerEnter(other);
            
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == Global.ItemTag.SIGHT_SWITCH || 
            other.tag == Global.ItemTag.BOX || 
            other.tag == Global.ItemTag.LEVEL_SWITCH || 
            other.tag == Global.ItemTag.PAINTER)
        {
            StateSystem.Instance.InteractTriggerExit(other);
        }

    }
    public void PlayerDead()
    {
        StateSystem.Instance.DeadEvent(_player);
        _player.layer = 0;
    }

}
