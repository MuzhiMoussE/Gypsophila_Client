using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static Global;

public static class ArchiveSystem
{
    //静态类，一直存储
    public static bool isRecorded = false;
    public static bool painted = false;
    public static TaskManager currentTasks;
    public static int SceneIndex = 1;
    public static int LevelIndex = 0;
    public static Transform playerPos;
    private static Transform[] restartPos;
    public static bool restart = true;
    public static int sketchCnt = 0;
    public static void Init(TaskManager tasks)
    {
        ResetSystemForScene(tasks);
        //DontDestroyOnLoad(gameObject);
    }
    public static void RecordPlayerPos(Transform player)
    {
        playerPos = player;
    }
    private static void ResetSystemForScene(TaskManager tasks)
    {
        currentTasks = tasks;
        restartPos = new Transform[currentTasks.levelEvents.Count];
        for (int i = 0; i < currentTasks.levelEvents.Count; i++)
        {
            restartPos[i] = currentTasks.levelEvents[i].restartPos;
        }
    }
    public static  void ReturnScene()
    {
        //StartCoroutine(LoadScene(SceneIndex));
        //asyncLoad.allowSceneActivation = true;
        LoadScene(SceneIndex);
        //需要一个全局函数获得player
        //LoadArchive(LevelIndex,player);
    }
    public static  void LoadArchive(int levelindex,Transform player)
    {
        Debug.Log("Current level:" + levelindex);
        //playerPos = player.transform;
        for (int i = 0; i < levelindex; i++)
        {
            currentTasks.levelEvents[i].isTriggered = true;
        }
        if(restart) player.position = restartPos[levelindex].position;
        else
        {
            player.position = restartPos[levelindex+1].position;
            StateSystem.Instance.playerState = PlayerState.Idle;
            AnimSystem.Instance.ChangeAnimState(StateSystem.Instance.playerState);
            restart = true;
            SketchUtility.Instance.SketchInit();
        }
    }
    public static void LoadScene(int index)
    {
        SceneIndex = index;
        //获取加载对象
        SceneManager.LoadSceneAsync(index);
    }
}
