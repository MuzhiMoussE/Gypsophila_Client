using Frame;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class LevelSwitch : InteractiveItems
{
    public bool canReturn = false;
    private bool finishLevel = false;
    private Color originColor;
    private Material mat;
    private List<GameObject> in_items = new List<GameObject>();
    private float downDistance = 0.4f;
    private AudioSource audioSource;
    public UnityEvent _event;
    public UnityEvent _offevent;
    private void OnTriggerEnter(Collider other)
    {
        if (finishLevel) return;
        if (other != null)
        {
            if(other.tag == Global.ItemTag.PLAYER || other.tag == Global.ItemTag.SKETCH_MAN || other.tag == Global.ItemTag.BOX)
            {
                if(!in_items.Contains(other.gameObject))
                {
                    in_items.Add(other.gameObject);
                    //Debug.Log("ADD" + other.gameObject.name);

                }
                if(in_items.Count>0 && state == ItemState.Off)
                {
                    SwitchOnEvent();
                }

            }
        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        if (!canReturn) return;
        if (other != null)
        {
            if (finishLevel) return;
            if (other.tag == Global.ItemTag.PLAYER || other.tag == Global.ItemTag.SKETCH_MAN || other.tag == Global.ItemTag.BOX)
            {
                if(in_items.Contains(other.gameObject))
                {
                    in_items.Remove(other.gameObject);
                    //Debug.Log("REMOVE" + other.gameObject.name);
                }
                if(in_items.Count == 0 && state == ItemState.On)
                {
                    SwitchOffEvent();
                }

            }
        }
    }

    public void SwitchOnEvent()
    {
        mat.SetColor("_Color",Color.white);
        gameObject.transform.position -= new Vector3(0, downDistance, 0);
        state = ItemState.On;
        audioSource.Play();
        EventCenter.Broadcast(GameEvent.ItemStateChangeEvent);
        if(_event!=null)
        {
            _event.Invoke();
        }
    }
    public void SwitchOffEvent()
    {
        audioSource.Play();
        mat.SetColor("_Color", originColor);
        gameObject.transform.position += new Vector3(0, downDistance, 0);
        state = ItemState.Off;
        EventCenter.Broadcast(GameEvent.ItemStateChangeEvent);
        if (_event != null)
        {
            _offevent.Invoke();
        }
    }
    public void LevelFinishState()
    {
        mat.SetColor("_Color", Color.yellow);
        finishLevel = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        state = ItemState.Off;
        mat = GetComponent<MeshRenderer>().material;
        originColor =mat.GetColor("_Color");
        audioSource = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
