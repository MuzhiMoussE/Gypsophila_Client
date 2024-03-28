using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class Global
{

    public enum PlayerState
    {
        Idle,
        Moving,
        Jumping,
        ToSummon,
        Releasing,
        Recording,
        Interacting,
        Dragging,
        Die
    }
    public enum InputAction
    {
        MOVING_LEFT,
        MOVING_RIGHT,
        JUMPING,
        NONE
    }
    public class ActionPair
    {
        public InputAction action;
        public float startTime;
        public float endTime;
        //public bool startReleased;
        public ActionPair()
        {
            action = InputAction.NONE;
            startTime = 0;
            endTime = 0;
            //startReleased = false;
        }
        public ActionPair(InputAction action, float startTime, float endTime)
        {
            this.action = action;
            this.startTime = startTime;
            this.endTime = endTime;
            //startReleased = false;
        }

        override public string ToString()
        {
            return action.ToString()+" Start:"+startTime+"  End:"+endTime;
        }
    }
    public static class ItemTag
    {
        public static string PLAYER = "Player";
        public static string BOX = "Box";
        public static string LEVEL_SWITCH = "LevelSwitch";
        public static string SIGHT_SWITCH = "SightSwitch";
        public static string UNTAGGED = "Untagged";
        public static string SKETCH_MAN = "SketchMan";
        public static string PAINTER = "Painter";
        public static string TRAP = "Trap";
    }

}
