using System;
using System.Collections.Generic;

namespace Frame
{
    //事件列表
    public enum GameEvent
    {
        ItemStateChangeEvent,   //物品状态变化
        TriggerLevelEvent,   //触发关卡事件
        //SceneUnloadEvent,   //场景卸载事件
        //ScenePreloadEvent,     //场景加载事件（卸载当前场景（如有）后，加载新场景）
        //SceneLoadCompletedEvent,    //场景加载完成事件
        //PreloadSceneDisplayEvent,  //预加载场景显示事件
        //TitleFadeEvent,     //标题淡入淡出事件
        //AfterTitleEvent,    //标题动画播放完成事件
        // DayPeriodChanged,    //一日的时期（昼夜黄昏）变化（DayPeriod）
        // BehaviorEnable,      //可以进行某种行为 (PlayerBehavior)
        // PlayerEnterStatus,   //玩家角色进入某种状态(PlayerStatus)
    }

    public delegate void Callback();
    public delegate void Callback<in T1>(T1 arg1);
    public delegate void Callback<in T1, in T2>(T1 arg1, T2 arg2);
    //事件广播中心
    public class EventCenter
    {
        private static Dictionary<GameEvent, Delegate> EventListeners = new Dictionary<GameEvent, Delegate>();

        public static void AddListener(GameEvent eventType, Callback callback)
        {
            if (CheckPreviousListeners(eventType, callback, out var previous))
                EventListeners[eventType] = (Callback)previous + callback;
        }

        public static void AddListener<T1>(GameEvent eventType, Callback<T1> callback)
        {
            if (CheckPreviousListeners(eventType, callback, out var previous))
                EventListeners[eventType] = (Callback<T1>)previous + callback;
        }

        public static void AddListener<T1, T2>(GameEvent eventType, Callback<T1, T2> callback)
        {
            if (CheckPreviousListeners(eventType, callback, out var previous))
                EventListeners[eventType] = (Callback<T1, T2>)previous + callback;
        }
        
        public static void RemoveListener(GameEvent eventType, Callback callback)
        {
            if (CheckPreviousListeners(eventType, callback, out var previous) && previous != null)
                EventListeners[eventType] = (Callback)EventListeners[eventType] - callback;
            AfterListenerRemoved(eventType);
        }
        public static void RemoveListener<T1>(GameEvent eventType, Callback<T1> callback)
        {
            if (CheckPreviousListeners(eventType, callback, out var previous) && previous != null)
                EventListeners[eventType] = (Callback<T1>)EventListeners[eventType] - callback;
            AfterListenerRemoved(eventType);
        }
        public static void RemoveListener<T1,T2>(GameEvent eventType, Callback<T1,T2> callback)
        {
            if (CheckPreviousListeners(eventType, callback, out var previous) && previous != null)
                EventListeners[eventType] = (Callback<T1,T2>)EventListeners[eventType] - callback;
            AfterListenerRemoved(eventType);
        }
        private static bool CheckPreviousListeners(GameEvent eventType, Delegate callback, out Delegate previous)
        {
            if (!EventListeners.TryGetValue(eventType, out previous) || previous.GetType() == callback.GetType())
                return true;
            throw new Exception($"EventCenter listener type mismatched: {eventType}");
        }

        private static void AfterListenerRemoved(GameEvent eventType)
        {
            if (EventListeners[eventType] == null)
                EventListeners.Remove(eventType);
        }

        public static void Broadcast(GameEvent eventType)
        {
            if(EventListeners.TryGetValue(eventType, out var listener))
            {
                Callback callback = listener as Callback;
                callback?.Invoke();
            }
        }
 
        public static void Broadcast<T1>(GameEvent eventType, T1 arg1)
        {
            if(EventListeners.TryGetValue(eventType, out var listener))
            {
                Callback<T1> callback = listener as Callback<T1>;
                callback?.Invoke(arg1);
            }
        }
        
        public static void Broadcast<T1,T2>(GameEvent eventType, T1 arg1, T2 arg2)
        {
            if(EventListeners.TryGetValue(eventType, out var listener))
            {
                Callback<T1,T2> callback = listener as Callback<T1,T2>;
                callback?.Invoke(arg1,arg2);
            }
        }
    }
}