using System;
using System.Net.Sockets;
using UnityEngine;
using Utility;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine.SceneManagement;
using System.Runtime.CompilerServices;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Cinemachine;

public class SketchReceiver : SingletonMonoBase<SketchReceiver>
{
    public GameObject paintUI;
    public Image loading;
    public bool startLoading = false;
    private int frameCnt = 0;
    public bool startListening = false;
    private TcpListener _listener;
    private const int Port = 9998;
    private  string SavePath;
    private bool canClose = false;
    Thread clientThread;
    TcpClient client;
    Thread thread;
    private void Update()
    {
        if(startListening)
        {
            StartListening();
            startListening = false;
        }
        if(canClose)
        {
            CloseListener();
            canClose = false;
        }
    }
    private void StartListening()
    {
        _listener = new TcpListener(IPAddress.Any, Port);
        _listener.Start();
        Debug.Log("Server started on port " + Port);

        thread = new Thread(new ThreadStart(ListenForClients));
        thread.Start();
    }
    private void ListenForClients()
    {
        while (true)
        {
            client = _listener.AcceptTcpClient();
            clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
            clientThread.Start(client);
        }
    }

    private void HandleClientComm(object client)
    {
        TcpClient tcpClient = (TcpClient)client;
        NetworkStream clientStream = tcpClient.GetStream();

        byte[] message = new byte[4096];
        int bytesRead;

        while (true)
        {
            bytesRead = 0;

            try
            {
                bytesRead = clientStream.Read(message, 0, 4096);
            }
            catch
            {
                break;
            }

            if (bytesRead == 0)
            {
                break;
            }
            // Convert the bytes to a string
            string messageString = Encoding.ASCII.GetString(message, 0, bytesRead);

            // Check if the message contains "END"
            if (messageString.Contains("END"))
            {
                break;
            }
            MemoryStream ms = new MemoryStream();
            while (bytesRead == 4096)
            {
                ms.Write(message, 0, bytesRead);
                bytesRead = clientStream.Read(message, 0, 4096);
            }
            ms.Write(message, 0, bytesRead);
            SavePath = Application.streamingAssetsPath + "/AnimSequence/sketch"+ArchiveSystem.sketchCnt;
            if (!Directory.Exists(SavePath))
            {
                Directory.CreateDirectory(SavePath);
            }
            string fileName = Path.Combine(SavePath, frameCnt + ".png");
            File.WriteAllBytes(fileName, ms.ToArray());
            //Debug.Log("File saved to " + fileName);
            frameCnt++;
        }

        //tcpClient.Close();
        Debug.Log("ClientClose!");

        canClose = true;
    }
    public void CloseListener()
    {
        thread.Abort();
        clientThread.Abort();
        client.Close();
        _listener.Stop();
        startListening = false;
        ArchiveSystem.sketchCnt++;
        //ArchiveSystem archivesystem = FindObjectOfType<ArchiveSystem>();
        //ArchiveSystem.restart = false;
        //ArchiveSystem.ReturnScene();
        startLoading = false;
        loading.gameObject.SetActive(false);
        //gameObject.SetActive(false);
        gameObject.GetComponentInChildren<CinemachineVirtualCamera>().Priority = 10;
        paintUI.SetActive(false);
    }
 }
