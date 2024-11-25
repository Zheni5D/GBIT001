using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class actorFSM : simpleFSM
{
    protected override void StateRegister()
    {
        if(statesDic.Count != 0)
        {
            //Debug.LogError("名字为 [" + gameObject.name + "] 的物体已经注册过一次所有状态");
            return;
        }
        else
        {
            statesDic.Add(StateType.Idle,new actorState(this));
            statesDic.Add(StateType.Partol,new PartolState(this));
            statesDic.Add(StateType.Chase,new ChaseState(this));
            statesDic.Add(StateType.React,new ReactState(this));
            statesDic.Add(StateType.Attack,new AttackState(this));
            statesDic.Add(StateType.Dead,new DeadState(this));
            statesDic.Add(StateType.Down,new DownState(this));
            statesDic.Add(StateType.ExcuteDead,new ExcuteDeadState(this));
            statesDic.Add(StateType.Confused,new ConfusedState(this));
            statesDic.Add(StateType.Reach,new ReachState(this));
        }
    }
}
