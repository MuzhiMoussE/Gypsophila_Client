using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System;
using System.Text;
using System.Threading;

public class Sketch: MonoBehaviour
{
    public Button button;
    public Slider slider;
    public Texture2D icon_normal;
    public Texture2D icon_paint;
    private Texture2D screenShot;
    private float speed = 100;
    private LineRenderer line;
    private Material mat;
    
    private int num = 0;//总共画画点数
    private Color c;
    private Socket socket = null;
    private IPEndPoint endPoint = null;
    private byte[] buffer;
    private bool startPainting = false;
    // Use this for initialization
    private bool inPlane = false;
    private bool canChange = true;
    public void Init()
    {
        slider.value = 0.1f;
        //实例化一张带透明通道大小为256*256的贴图
        screenShot = new Texture2D(256, 256, TextureFormat.RGB24, false);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999);
        socket.Connect(endPoint);
        startPainting = true;
        canChange = true;
    }
    // Update is called once per frame
    void Update()
    {
        if(startPainting)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && canChange)
            {
                if(!inPlane && canChange)
                {
                    Cursor.SetCursor(icon_paint, Vector2.zero, CursorMode.Auto);
                    inPlane = true;
                }
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
                    line.SetPosition(num - 1, hit.point + new Vector3(0, 0, -0.002f));

                }
            }
            else
            {
                if(inPlane && canChange)
                {
                    inPlane = false;
                    Cursor.SetCursor(icon_normal, Vector2.zero, CursorMode.Auto);
                }
            }
        }
        if(SketchReceiver.Instance.startLoading)
        {
            SketchReceiver.Instance.loading.transform.Rotate(new Vector3(0,0,Time.deltaTime*speed));
        }
    }
    public void FinishDrawing()
    {
        Cursor.SetCursor(icon_normal, Vector2.zero, CursorMode.Auto);
        canChange = false;
        StartCoroutine(FinishScreenShot());
    }
    IEnumerator FinishScreenShot()
    {
        SketchReceiver.Instance.startLoading = true;
        SketchReceiver.Instance.loading.gameObject.SetActive(true);
        string image = Application.streamingAssetsPath + "/ScreenShot/screenshot" + ArchiveSystem.sketchCnt.ToString() + ".png";
        ScreenCapture.CaptureScreenshot(Application.streamingAssetsPath + "/ScreenShot/screenshot" + ArchiveSystem.sketchCnt.ToString() + ".png");
        yield return new WaitForSeconds(1);
        SendImage(image);
    }
    private void SendImage(string fileName)
    {
        startPainting = false;
        if (!File.Exists(fileName))
        {
            Debug.Log("File does not exist: " + fileName);
            return;
        }
        byte[] bt;
        using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
        {
            BinaryReader strread = new BinaryReader(fs);
            bt = new byte[fs.Length];
            strread.Read(bt, 0, bt.Length);
        }
        SocketAsyncEventArgs sendEventArgs = new SocketAsyncEventArgs();
        sendEventArgs.RemoteEndPoint = endPoint;
        sendEventArgs.SetBuffer(bt, 0, bt.Length);
        socket.SendAsync(sendEventArgs);
        Debug.Log("Send Over!");
        socket.Close();
        SketchReceiver.Instance.startListening = true;
    }

}

