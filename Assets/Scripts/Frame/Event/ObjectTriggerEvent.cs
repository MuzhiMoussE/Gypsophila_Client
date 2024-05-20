using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectTriggerEvent : MonoBehaviour
{
    public bool untrigger = true;
    public UnityEvent _event;
    public UnityEvent _exitevent;
    public bool canDisappear = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!untrigger) return;
        if (other.tag == Global.ItemTag.PLAYER)
        {
            _event.Invoke();
            untrigger = false;
            if(canDisappear) StartCoroutine(triggerDisappear());

        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == Global.ItemTag.PLAYER)
        {
            if(_exitevent != null)
            {
                _exitevent.Invoke();
                untrigger = true;
            }
            
            if (canDisappear) StartCoroutine(triggerDisappear());

        }
    }
    IEnumerator triggerDisappear()
    {
        yield return new WaitForSeconds(1);
        gameObject.GetComponent<TriggerEvent>().enabled = false;
    }
}
