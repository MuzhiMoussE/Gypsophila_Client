using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraUtils : MonoBehaviour
{
    public Transform target;
    private Vector3 offset;
    void Start()
    {
        //设置相对偏移
        offset = target.position - this.transform.position;
    }
    void Update()
    {
        //更新位置
        this.transform.position = target.position - offset;
    }
}
