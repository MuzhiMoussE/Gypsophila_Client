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
    public static int LevelIndex = -1;
    public static Transform playerPos;
    private static Transform[] restartPos;
    public static bool restart = true;
    public static int sketchCnt = 0;
    public static void ClearLevel()
    {
        isRecorded = false;
        painted = false;
        currentTasks = null;
        LevelIndex = -1;
        restartPos.Initialize();
        restart = true;
        sketchCnt = 0;
    }
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
    public static void PrintLevelState(int levelindex)
    {
        Debug.Log("Current level:" + levelindex + " Restart:" + restart + " Recorded? " + isRecorded + " Painted? " + painted);
    }
    public static void LoadArchive(int levelindex,Transform player)
    {
        StateSystem.Instance.playerState = PlayerState.Idle;
        AnimSystem.Instance.ChangeAnimState(StateSystem.Instance.playerState);
        PrintLevelState(levelindex);
        if(levelindex == -1)
        {
            player.position = restartPos[0].position;
        }
        else
        {
            //playerPos = player.transform;
            for (int i = 0; i < levelindex; i++)
            {
                currentTasks.levelEvents[i].isTriggered = true;
            }
            if (restart) player.position = restartPos[levelindex].position;
            else
            {
                player.position = restartPos[levelindex + 1].position;
                restart = true;
            }
        }
        SketchUtility.Instance.SketchInit();

    }
    public static void LoadScene(int index)
    {
        SceneIndex = index;
        //获取加载对象
        SceneManager.LoadScene(index);
    }
}
