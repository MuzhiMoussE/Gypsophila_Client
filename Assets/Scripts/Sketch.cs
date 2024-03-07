using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System;

public class Sketch: MonoBehaviour
{
    private Texture2D screenShot;
    public Button button;
    LineRenderer line;
    Material mat;
    public Slider slider;
    int num = 0;//总共画画点数
    int shotindex = 0;
    Color c;
    private Socket socket = null;
    private IPEndPoint endPoint = null;
    // Use this for initialization
    void Start()
    {
        slider.value = 0.1f;
        //实例化一张带透明通道大小为256*256的贴图
        screenShot = new Texture2D(256, 256, TextureFormat.RGB24, false);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999);
        socket.Connect(endPoint);
    }
    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (c == null)
                {
                    return;
                }
                GameObject obj = new GameObject();
                line = obj.AddComponent<LineRenderer>();
                line.material.color = c;
                line.widthMultiplier = slider.value;//宽度
                line.SetPosition(0, hit.point);
                line.SetPosition(1, hit.point);
                num = 0;
            }
            if (Input.GetMouseButton(0))
            {
                num++;
                line.positionCount = num;
                line.SetPosition(num - 1, hit.point + Vector3.up * 0.2f);

            }
        }
    }
    public void FinishDrawing()
    {
        string image = Application.streamingAssetsPath + "/ScreenShot/screenshot" + shotindex.ToString() + ".png";
        ScreenCapture.CaptureScreenshot(Application.streamingAssetsPath + "/ScreenShot/screenshot" + shotindex.ToString() + ".png");
        SendImage(image);
        shotindex++;

    }
    void SendImage(string fileName)
    {
        FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Read);
        BinaryReader strread = new BinaryReader(fs);
        byte[] bt = new byte[fs.Length];
        strread.Read(bt, 0, bt.Length - 1);
        socket.Send(bt);
        fs.Close();
        socket.Close();
    }
}

