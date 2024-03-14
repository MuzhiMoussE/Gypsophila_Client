using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Utility;
using Frame;
[Serializable]
public class LevelEvent
{
    //名称
    public string eventName;
    //项目处于可交互状态才会交互
    public bool isTriggered;

    //item状态对
    public List<InteractiveItems> interactiveItems;

    public UnityEvent onEventTriggered;

    //用来判断是否交互，如果交互成功返回true
    public bool CheckEvent()
    {
        if (isTriggered)
        {
            Debug.LogError("Event " + eventName + " is already triggered!");
            return false;
        }
        //判断条件是否达成 若干个相关物体是否在正确的状态
        if (interactiveItems.Any(itemStatePair => itemStatePair.state != ItemState.On))
        {
            return false;
        }
        //这个事件
        isTriggered = true;
        return true;
    }
}
public enum Item
{
    LevelSwitch,
    SightSwitch
}
public enum ItemState
{
    On,
    Off
}
[Serializable]
public class InteractiveItems :MonoBehaviour
{
    public Item item;
    public ItemState state;
}
public class TaskManager : SingletonMonoBase<TaskManager> 
{
    //大关卡中有多少个小关卡，对应的关卡事件
    public List<LevelEvent> levelEvents;
    //下一个事件索引
    private int _nextEventIndex;
    private void Start()
    {
        _nextEventIndex = 0;
    }

    //接受广播
    private void OnEnable()
    {
        EventCenter.AddListener(GameEvent.ItemStateChangeEvent, OnItemStateChanged);
    }

    private void OnDisable()
    {
        EventCenter.RemoveListener(GameEvent.ItemStateChangeEvent, OnItemStateChanged);
    }
    //项目状态更改
    private void OnItemStateChanged()
    {
        //判断事件是否启动
        if (_nextEventIndex <levelEvents.Count && levelEvents[_nextEventIndex].CheckEvent())
        {
            //执行事件后果
            EventCenter.Broadcast(GameEvent.TriggerLevelEvent, levelEvents[_nextEventIndex]);
            levelEvents[_nextEventIndex].onEventTriggered?.Invoke();
            _nextEventIndex++;
            EventCenter.Broadcast(GameEvent.ItemStateChangeEvent);
        }
    }
}
