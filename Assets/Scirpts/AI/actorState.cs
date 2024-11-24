using UnityEngine;

public class actorState : BaseState //替代idleState
{
    private simpleFSM manager;
    private AIParamater paramater;

    public actorState(simpleFSM fsm)
    {
        manager = fsm;
        paramater = fsm.paramater;
    }
    public void OnEnter()
    {
        paramater.animator.Play("Idle");
        paramater.rigidbody.velocity = Vector2.zero;
    }

    public void OnUpdate()
    {
        //AI死亡
        //-----------------------
        if(paramater.isDead)
        {
            manager.StateTransition(StateType.Dead);
        }
        //AI倒地
        else if(paramater.isDown)
        {
            manager.StateTransition(StateType.Down);
        }
        //发现玩家切换状态
        //----------
        else if(manager.IsPlayerSighted())
        {
            manager.StateTransition(StateType.React);
        }
        //听到声音切换状态
        else if(paramater.isHeared)
        {
            manager.StateTransition(StateType.Reach);
        }
    }

    public void OnExit()
    {

    }
}

public class DontChaseState : BaseState//替代ChaseState
{
    private simpleFSM manager;
    private AIParamater paramater;
    private bool flag;
    private Vector3 lastTargetPos;

    public DontChaseState(simpleFSM _manager)
    {
        manager = _manager;
        paramater = _manager.paramater;
    }
    public void OnEnter()
    {
        paramater.animator.Play("idle");
        paramater.chaseViewAngle /= 2.0f;
        paramater.maxChaseDistance += 5f;
    }
    
    public void OnUpdate()
    {
       if(paramater.isFreeze){
            if(flag != true){
                //OnTimeStop
                paramater.rigidbody.velocity = Vector2.zero;
            }
            flag = true;
        }else{
            if(flag != false){
                //OnTimeStopOff
                int nState = manager.TimeStopOffJudge(paramater.target.position);
                //Debug.Log(nState);
                if(nState == -1){
                    //Do Nothing
                }else if(nState == (int)StateType.Confused)
                {
                    //如果在追逐过程中,AI到玩家之间有障碍,玩家时停脱身
                    //此时AI应该认为玩家还在"那个"地方,而不是立即困�?
                    lastTargetPos = manager.getLastTargetPos();
                    manager.UpdatePath(lastTargetPos);
                }
                else{
                    manager.StateTransition((StateType)nState);
                    flag = false;
                    return;
                }
            }
            flag = false;
            //不追击目标
            //--------------
            Vector3 dir = paramater.target.position - manager.transform.position;
            manager.turn(dir);

        }
        //AI死亡
        //-----------------------
        if(paramater.isDead)
        {
            manager.StateTransition(StateType.Dead);
        }
        //AI倒地
        else if(paramater.isDown)
        {
            manager.StateTransition(StateType.Down);
        }
        //丢失目标
        //--------------
        else if(manager.IsPlayerSighted() == false )
        {
            manager.StateTransition(StateType.Confused);
        }
        else if(!paramater.isFreeze)
        {
            //目标进入攻击范围
            //--------------
            if(paramater.attackArea.isInAttackArea())
            {
                manager.StateTransition(StateType.Attack);
            }
        }
        
    }

    public void OnExit()
    {
        paramater.chaseViewAngle *= 2.0f;
        paramater.maxChaseDistance -= 5f;
        flag = false;
    }

}
