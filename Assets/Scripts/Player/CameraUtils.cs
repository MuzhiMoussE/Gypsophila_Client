using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraUtils : MonoBehaviour
{
    public Transform target;
    private Vector3 offset;
    void Start()
    {
        //�������ƫ��
        offset = target.position - this.transform.position;
    }
    void Update()
    {
        //����λ��
        this.transform.position = target.position - offset;
    }
}
