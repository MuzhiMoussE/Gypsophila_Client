using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class AnimSystem : SingletonMonoBase<AnimSystem>
{
    private Animator animator;
    private string animStateString = null;
    private Global.PlayerState lastState = Global.PlayerState.Idle;
    public void setInit(Animator _animator)
    {
        animator = _animator;
    }
    public void ChangeAnimState(Global.PlayerState state)
    {
        if (lastState == state)
            return;
        animator.SetBool(lastState.ToString(), false);
        animStateString = state.ToString();
        animator.SetBool(animStateString, true);
        lastState = state;
        
    }
}
