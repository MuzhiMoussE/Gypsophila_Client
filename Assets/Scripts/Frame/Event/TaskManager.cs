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
    //����
    public string eventName;
    //��Ŀ���ڿɽ���״̬�Żύ��
    public bool isTriggered;

    //item״̬��
    public List<InteractiveItems> interactiveItems;

    public UnityEvent onEventTriggered;

    //�����ж��Ƿ񽻻�����������ɹ�����true
    public bool CheckEvent()
    {
        if (isTriggered)
        {
            Debug.LogError("Event " + eventName + " is already triggered!");
            return false;
        }
        //�ж������Ƿ��� ���ɸ���������Ƿ�����ȷ��״̬
        if (interactiveItems.Any(itemStatePair => itemStatePair.state != ItemState.On))
        {
            return false;
        }
        //����¼�
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
public class InteractiveItems
{
    public Item item;
    public ItemState state;
}
public class TaskManager : SingletonMonoBase<TaskManager> 
{
    public string levelName;
    //�洢�ɽ�����Ʒ
    public List<InteractiveItems> interactiveItems;
    //��ؿ����ж��ٸ�С�ؿ�����Ӧ�Ĺؿ��¼�
    public List<LevelEvent> levelEvents;
    //��һ���¼�����
    private int _nextEventIndex;
    private void Start()
    {
        _nextEventIndex = 0;
    }

    //���ܹ㲥
    private void OnEnable()
    {
        EventCenter.AddListener(GameEvent.ItemStateChangeEvent, OnItemStateChanged);
    }

    private void OnDisable()
    {
        EventCenter.RemoveListener(GameEvent.ItemStateChangeEvent, OnItemStateChanged);
    }
    //��Ŀ״̬����
    private void OnItemStateChanged()
    {
        //�ж��¼��Ƿ�����
        if (levelEvents[_nextEventIndex].CheckEvent())
        {
            //ִ���¼����
            EventCenter.Broadcast(GameEvent.TriggerLevelEvent, levelEvents[_nextEventIndex]);
            levelEvents[_nextEventIndex].onEventTriggered?.Invoke();
            _nextEventIndex++;
            EventCenter.Broadcast(GameEvent.ItemStateChangeEvent);
        }
    }
}
