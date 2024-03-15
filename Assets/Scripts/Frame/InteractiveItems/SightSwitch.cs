using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
public class SightSwitch : InteractiveItems
{
    public int stateCount;
    private float rotateAngle;
    public GameObject receiver;
    public List<GameObject> switches = new List<GameObject>();
    private void Start()
    {
        rotateAngle = 360 / stateCount;
    }
    public void Interact()
    {
        gameObject.transform.Rotate(-rotateAngle, 0,0);
    }
    public void LevelFinishState()
    {
        foreach (GameObject _switch in switches)
        {
            _switch.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.yellow);
        }
    }

}


