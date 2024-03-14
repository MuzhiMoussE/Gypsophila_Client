using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelSwitch : InteractiveItems
{
    public Color originColor;
    public Material mat;
    [SerializeField]private List<GameObject> in_items = new List<GameObject>();
    private float downDistance = 0.1f;

    private void OnTriggerEnter(Collider other)
    {
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
                    state = ItemState.On;
                    SwitchOnEvent();
                }

            }
        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        if (other != null)
        {
            if (other.tag == Global.ItemTag.PLAYER || other.tag == Global.ItemTag.SKETCH_MAN || other.tag == Global.ItemTag.BOX)
            {
                if(in_items.Contains(other.gameObject))
                {
                    in_items.Remove(other.gameObject);
                    //Debug.Log("REMOVE" + other.gameObject.name);
                }
                if(in_items.Count == 0 && state == ItemState.On)
                {
                    state = ItemState.Off;
                    SwitchOffEvent();
                }

            }
        }
    }

    public void SwitchOnEvent()
    {
        mat.SetColor("_Color",Color.white);
        gameObject.transform.position -= new Vector3(0, downDistance, 0);
    }
    public void SwitchOffEvent()
    {
        mat.SetColor("_Color", originColor);
        gameObject.transform.position += new Vector3(0, downDistance, 0);
    }


    // Start is called before the first frame update
    void Start()
    {
        state = ItemState.Off;
        mat = GetComponent<MeshRenderer>().material;
        originColor = mat.GetColor("_Color");

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
