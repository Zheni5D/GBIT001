using UnityEngine;
public class IdleState : BaseState
{
    private simpleFSM manager;
    private AIParamater paramater;
    private float idletimer;

    public IdleState(simpleFSM _manager)
    {
        manager = _manager;
        paramater = _manager.paramater;
    }
    public void OnEnter()
    {
        paramater.animator.Play("Idle");
        paramater.rigidbody.velocity = Vector2.zero;
    }
    
    public void OnUpdate()
    {
        idletimer += manager.time.deltaTime;
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
        //发现玩家切换状�?
        //----------
        else if(manager.IsPlayerSighted() && !paramater.isFreeze)
        {
            manager.StateTransition(StateType.React);
        }
        //听到声音切换状�?
        else if(paramater.isHeared)
        {
            manager.StateTransition(StateType.Reach);
        }
        //到达巡逻点停留一会再切换回巡�?
        else if(idletimer >= manager.paramater.idleTime) {
            manager.StateTransition(StateType.Partol);
        }
    }

    public void OnExit()
    {
        idletimer = 0f;
    }
}

public class PartolState : BaseState
{
    private simpleFSM manager;
    private AIParamater paramater;
    private int pathIndex=0;

    public PartolState(simpleFSM _manager)
    {
        manager = _manager;
        paramater = _manager.paramater;
    }
    public void OnEnter()
    {
        paramater.animator.Play("Move");
        paramater.reachedEndOfPath = false;
        pathIndex = 0;
    }
    
    public void OnUpdate()
    {
        if(paramater.isFreeze)
        {
            paramater.rigidbody.velocity = Vector2.zero;
        }
        else
        {
            //设置物体沿着path移动
            //------------------
            MoveAlongPath();
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
        //发现玩家切换状�?
        //----------
        else if(manager.IsPlayerSighted() && !paramater.isFreeze)
        {
            manager.StateTransition(StateType.React);
        }
        //听到声音切换状�?
        else if(paramater.isHeared)
        {
            manager.StateTransition(StateType.Reach);
        }
        //物体到达巡逻点后进入朝下一个巡逻点移动,这部分逻辑在OnExit()里面实现x
        //------------------
        else if(paramater.reachedEndOfPath)
        {
            paramater.portalIndex++;
        
            if(paramater.portalIndex >= paramater.portalTarget.Length)
            {
                paramater.portalIndex = 0;
            }
            manager.StateTransition(StateType.Idle);
        }
    }

    public void OnExit()
    {
        
    }

    void MoveAlongPath()
    {
        //路径是否有效
        if (paramater.partolPath == null)
        {
            Debug.LogError("ERROR");
            return;
        }
        //是否到达路径终点
        //---------------
        if(paramater.reachedEndOfPath)
        {
            return;
        }
        //移动与旋�?
        //------------------
        Vector2 obj2target = paramater.partolPath[paramater.portalIndex].vectorPath[pathIndex] 
        - manager.transform.position;//物体指向目标的向�?
        Vector2 dir = obj2target.normalized;
        //如果速度与前方相�?80°,这是个未知的bug
        float dangle = Vector3.Angle(manager.transform.up,dir);
        if(180f - dangle < 1f)
        {
            manager.turn(manager.transform.right);
            return;
        }
         //设置物体移动速度
        manager.time.rigidbody2D.velocity = paramater.speedCoff * paramater.partolSpeed * dir;
        //设置物体永远尝试朝向目标方向
        manager.turn();
        //设置下一路径�?
        //---------------
        if(obj2target.magnitude < paramater.arriveDistance)
        {
            pathIndex++;
            //如果越界
            if(pathIndex >= paramater.partolPath[paramater.portalIndex].vectorPath.Count)
            {
                paramater.reachedEndOfPath = true;
                manager.time.rigidbody2D.velocity = Vector2.zero;
            }
        }
    }

}

public class ReactState : BaseState
{
    private simpleFSM manager;
    private AIParamater paramater;
    private float waitTimer;
    private bool flag;//记录时停开始还是结�?用于检�?开始结束沿"

    public ReactState(simpleFSM _manager)
    {
        manager = _manager;
        paramater = _manager.paramater;
    }
    public void OnEnter()
    {
        waitTimer=0f;
        paramater.animator.Play("Idle");
        manager.Exclamation();
        manager.time.rigidbody2D.velocity = Vector2.zero;
        paramater.maxChaseDistance += 10f;
    }
    
    public void OnUpdate()
    {
        if(paramater.isFreeze){
            if(flag != true){
                //OnTimeStop
            }
            flag = true;
        }else{
            if(flag != false){
                //OnTimeStopOff
                int nState = manager.TimeStopOffJudge(paramater.target.position);
                if(nState == -1){
                    //Do Nothing
                }else{
                    //Debug.Log("To "+((StateType)nState).ToString());
                    manager.StateTransition((StateType)nState);
                    flag = false;
                    return;
                }
            }
            flag = false;
            Vector3 dir = paramater.target.position - manager.transform.position;
            manager.turn(dir);
            waitTimer += manager.time.deltaTime;
            if(waitTimer >= paramater.reactTime) {
                manager.StateTransition(StateType.Chase);
                waitTimer = 0f;
            }
        }
        //AI死亡
        //-----------------------
        if(paramater.isDead)
        {
            manager.StateTransition(StateType.Dead);
            return;
        }
        //AI倒地
        else if(paramater.isDown)
        {
            manager.StateTransition(StateType.Down);
        }
        
    }

    public void OnExit()
    {
        waitTimer = 0;
        paramater.maxChaseDistance -= 10f;
    }
}

public class ChaseState : BaseState
{
    private simpleFSM manager;
    private AIParamater paramater;
    private float updateTimer=0f;
    private float updateRate = .25f;
    private bool flag;
    private bool isUpdatePathSecond;
    private Vector3 lastTargetPos;

    public ChaseState(simpleFSM _manager)
    {
        manager = _manager;
        paramater = _manager.paramater;
    }
    public void OnEnter()
    {
        //paramater.animator.Play("idle");
        paramater.animator.Play("Move");
        manager.UpdatePath();
        paramater.chaseIndex = 0;
        paramater.reachedEndOfPath = false;
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
                    isUpdatePathSecond = true;
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
            //..
            //路径更新定时�?
            //----------------
            updateTimer += manager.time.deltaTime;
            if(updateTimer >= updateRate) {
                if(isUpdatePathSecond)
                    manager.UpdatePath(lastTargetPos);
                else
                    manager.UpdatePath();
                updateTimer = 0;
            }
            //追击目标
            //--------------
            Chase();

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
        else if(manager.IsPlayerSighted() == false && !isUpdatePathSecond)
        {
            isUpdatePathSecond = true;
            lastTargetPos = paramater.target.position;
            manager.UpdatePath(lastTargetPos);
        }
        else if(!paramater.isFreeze)
        {
            //目标进入攻击范围
            //--------------
            if(paramater.attackArea.isInAttackArea())
            {
                manager.StateTransition(StateType.Attack);
            }
            else if(isUpdatePathSecond)
            {
                //追逐到lastPos途中发现了玩�?
                if(manager.IsPlayerSighted())
                {
                    isUpdatePathSecond = false;
                    //manager.StateTransition(StateType.React);
                }
                //追逐到lastPos但发现玩家不在那�?
                if(paramater.reachedEndOfPath)
                {
                isUpdatePathSecond = false;
                manager.StateTransition(StateType.Confused);
                }
            }
        }
        
    }

    public void OnExit()
    {
        paramater.maxChaseDistance -= 5f;
        isUpdatePathSecond = false;
        flag = false;
    }

    void Chase()
    {
        //是否到达路径终点
        //---------------
        if(paramater.chasePath == null)
            return;
        if(paramater.reachedEndOfPath || paramater.chaseIndex >= paramater.chasePath.vectorPath.Count)
        {
            if(Vector2.Distance(manager.transform.position,paramater.target.position) > 1.0f)
                paramater.reachedEndOfPath = false;
            return;
        }
        //移动与旋�?
        //------------------
        Vector2 obj2target = paramater.chasePath.vectorPath[paramater.chaseIndex] 
        - manager.transform.position;//物体指向目标的向�?
        Vector2 dir = obj2target.normalized;       
         //设置物体移动速度
        manager.time.rigidbody2D.velocity =paramater.speedCoff * paramater.chaseSpeed * dir;
        //设置物体永远尝试朝向目标方向
        manager.turn();
         //设置下一路径�?
        //---------------
        if(obj2target.magnitude < paramater.arriveDistance)
        {
            paramater.chaseIndex++;
            //如果越界
            if(paramater.chaseIndex >= paramater.chasePath.vectorPath.Count)
            {
                paramater.reachedEndOfPath = true;
                manager.time.rigidbody2D.velocity = Vector2.zero;
            }
        }
    }
}

public class AttackState : BaseState
{
    private simpleFSM manager;
    private AIParamater paramater;
    private bool flag;
    public AttackState(simpleFSM _manager)
    {
        manager = _manager;
        paramater = _manager.paramater;
    }
    public void OnEnter()
    {
        manager.time.rigidbody2D.velocity = Vector2.zero;

        if (paramater.attackArea is ExplosionAttack)
            paramater.chaseSpeed /= 100;

        if (paramater.attackArea is LaserAttack)
            paramater.turnSpeed /= 4.0f;
    }

    public void OnUpdate()
    {
        if(paramater.isFreeze){
            if(flag != true){
                //OnTimeStop
                paramater.attackArea.StopAttacking(true);
            }
            flag = true;
        }else{
            if(flag != false){
                //OnTimeStopOff
                paramater.attackArea.StopAttacking(false);
                int nState = manager.TimeStopOffJudge(paramater.target.position);
                if(nState == -1){
                    //Do Nothing
                }else{
                    flag = false;
                    manager.StateTransition((StateType)nState);
                    return;
                }
            }
            flag = false;
            //..
            Vector3 dir = paramater.target.position - manager.transform.position;
            manager.turn(dir);

            if (paramater.attackArea is NormalAttack rangedAttack)
            {
                if (rangedAttack.bulletNum > 0)
                {
                    if (rangedAttack.TryAttack())
                    {
                        paramater.animator.Play("Attack");
                    }
                }
                else
                {
                    paramater.animator.Play("Idle");
                }
            }
            else if (paramater.attackArea is LaserAttack laserAttack)
            {
                if (laserAttack.bulletNum > 0)
                {
                    if (laserAttack.TryAttack())
                    {
                        paramater.animator.Play("Attack");
                    }
                }
                else
                {
                    paramater.animator.Play("Idle");
                    laserAttack.CloseLaser();
                }
            }
            else if(paramater.attackArea is MeleeAttack)
            {
                paramater.attackArea.Attack();
                paramater.animator.Play("Attack");
            }
            else if (paramater.attackArea is ExplosionAttack)
            {
                paramater.attackArea.Attack();
                paramater.animator.Play("Idle");
            }
            else if (paramater.attackArea is MakeDogAttack)
            {
                paramater.attackArea.Attack();
                paramater.animator.Play("Attack");
            }
        }
        //AI死亡
        //-----------------------
        if(paramater.isDead)
        {
            manager.StateTransition(StateType.Dead);
            if (paramater.attackArea is LaserAttack laserAttack)
            {
                laserAttack.CloseLaser();
            }
            if (paramater.attackArea is ExplosionAttack explosionAttack)
            {
                explosionAttack.StopExplosion();
            }
        }
        //AI倒地
        else if(paramater.isDown)
        {
            manager.StateTransition(StateType.Down);
        }
        //玩家脱离攻击范围
        else if(!paramater.attackArea.isInAttackArea() && !paramater.isFreeze && paramater.attackArea is not ExplosionAttack)
        {
            if (paramater.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                manager.StateTransition(StateType.Chase);
            if (paramater.attackArea is LaserAttack laserAttack)
            {
                laserAttack.CloseLaser();
            }
        }
    }

    public void OnExit()
    {
        if (paramater.attackArea is LaserAttack laserAttack)
        {
            laserAttack.CloseLaser();
        }

        if (paramater.attackArea is LaserAttack)
            paramater.turnSpeed *= 4.0f;
    }
}

public class DeadState : BaseState
{
    private simpleFSM manager;
    private AIParamater paramater;
    float efxTimer=0f;
    float efxDelayTime = 0.1f;//为了做到时停时玩家杀死敌人不会立即生成尸体而特别添加了延迟

    public DeadState(simpleFSM _manager)
    {
        manager = _manager;
        paramater = _manager.paramater;
    }
    public void OnEnter()
    {
        manager.time.rigidbody2D.velocity = Vector2.zero;
        efxTimer = 0f;
        //manager.CreateBodyAndDestoryMyself();
    }

    public void OnUpdate()
    {
        efxTimer += manager.time.deltaTime;
        if(efxTimer >= efxDelayTime || !paramater.isFreeze) {
            manager.CreateBodyAndDestoryMyself();
        }
    }

    public void OnExit()
    {
        paramater.isDead = false;
    }
}

public class DownState : BaseState
{
    private simpleFSM manager;
    private AIParamater paramater;
    private float downTimer;

    public DownState(simpleFSM _manager)
    {
        manager = _manager;
        paramater = _manager.paramater;
    }
    public void OnEnter()
    {
        paramater.animator.Play("down");
        manager.time.rigidbody2D.velocity = Vector2.zero;
        paramater.bodyCollinder.isTrigger = true;
        manager.gameObject.layer = LayerMask.NameToLayer("DownEnemy");
    }
    
    public void OnUpdate()
    {
        //AI被处决死�?
        //-----------------------
        if(paramater.isDead)
        {
            manager.StateTransition(StateType.ExcuteDead);
            return;
        }
        downTimer += manager.time.deltaTime;
        if(downTimer >= paramater.downTime) {
            manager.StateTransition(StateType.Idle);
            downTimer -= paramater.downTime;
        }
    }

    public void OnExit()
    {
        downTimer = 0f;
        paramater.isDown = false;
        paramater.bodyCollinder.isTrigger = false;
        manager.gameObject.layer = LayerMask.NameToLayer("Enemy");
    }
} 
//废弃
public class ExcuteDeadState : BaseState
{
    private simpleFSM manager;
    private AIParamater paramater;
    private float deadTimer;

    public ExcuteDeadState(simpleFSM _manager)
    {
        manager = _manager;
        paramater = _manager.paramater;
    }
    public void OnEnter()
    {
        paramater.bodyCollinder.isTrigger = true;
        paramater.animator.Play("excute");
    }
    
    public void OnUpdate()
    {
        deadTimer += manager.time.deltaTime;
        if(deadTimer >= 1f) {
            manager.CreateBodyJustHereAndDestoryMyself();
            deadTimer -= 1f;
        }
        
    }

    public void OnExit()
    {
        deadTimer = 0F;
        paramater.isDead = false;
        paramater.bodyCollinder.isTrigger = false;
    }
}

public class ConfusedState : BaseState
{
    private simpleFSM manager;
    private AIParamater paramater;
    private float waitTimer;

    public ConfusedState(simpleFSM _manager)
    {
        manager = _manager;
        paramater = _manager.paramater;
    }
    public void OnEnter()
    {
        paramater.animator.Play("Idle");
        manager.Question();
        manager.time.rigidbody2D.velocity = Vector2.zero;
    }
    
    public void OnUpdate()
    {
        waitTimer += manager.time.deltaTime;
        if(waitTimer >= 1f) {
            manager.StateTransition(StateType.Reach);
            waitTimer -= 1f;
        }
        //AI死亡
        //-----------------------
        if(paramater.isDead)
        {
            manager.StateTransition(StateType.Dead);
            return;
        }
        //AI倒地
        else if(paramater.isDown)
        {
            manager.StateTransition(StateType.Down);
        }
        //发现玩家切换状�?
        //----------
        else if(manager.IsPlayerSighted() && !paramater.isFreeze)
        {
            manager.StateTransition(StateType.React);
        }
        
    }

    public void OnExit()
    {
        waitTimer = 0;
    }
}

public class ReachState : BaseState
{
    private simpleFSM manager;
    private AIParamater paramater;
    private float updateTimer=0f;
    private float updateRate = .5f;
    private Vector3 targetPos;

    public ReachState(simpleFSM _manager)
    {
        manager = _manager;
        paramater = _manager.paramater;
    }
    public void OnEnter()
    {
        //Debug.Log("REACH");
        paramater.animator.Play("Idle");
        
        paramater.chaseIndex = 0;
        paramater.reachedEndOfPath = false;

        //lastTarget = paramater.target;
        if(paramater.isHeared)//如果是通过听见声音进入reach状态的，去
            targetPos = paramater.heardPos;
        else//要么是找不着玩家进入reach状态，�?
            if(paramater.portalTarget.Length == 0)
                targetPos = manager.getOriPos();
            else
                targetPos = paramater.portalTarget[paramater.portalIndex].position;
        
        manager.UpdatePath(targetPos);

    }
    
    public void OnUpdate()
    {
        if(paramater.isFreeze)
        {
            paramater.rigidbody.velocity = Vector2.zero;
        }
        else
        {
            //路径更新定时�?
            //----------------
            //如果target是空的话,直接转到Idle
            // if(paramater.target == null)
            // {
            //     manager.StateTransition(StateType.Idle);
            //     return;
            // }
            updateTimer += manager.time.deltaTime;
            if(updateTimer >= updateRate) {
                manager.UpdatePath(targetPos);
                updateTimer = 0;
            }
            //追击目标
            //--------------
            Chase();
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
        //发现玩家切换状�?
        //----------
        else if(manager.IsPlayerSighted() && !paramater.isFreeze)
        {
            manager.StateTransition(StateType.React);
        }
        //到达原巡逻点
        //--------------
        else if(paramater.reachedEndOfPath)
        {
            if(paramater.isHeared)
            {
                manager.StateTransition(StateType.Confused);
            }
            else
                manager.StateTransition(StateType.Idle);
        }
    }

    public void OnExit()
    {
        // paramater.target = lastTarget;
        paramater.isHeared = false;
    }

    void Chase()
    {
        //是否到达路径终点
        //---------------
        if(paramater.chasePath == null)
            return;
        if(paramater.reachedEndOfPath || paramater.chaseIndex >= paramater.chasePath.vectorPath.Count)
        {
            if(Vector2.Distance(manager.transform.position,targetPos) > 1.0f)
                paramater.reachedEndOfPath = false;
            return;
        }
        //移动与旋�?
        //------------------
        Vector2 obj2target = paramater.chasePath.vectorPath[paramater.chaseIndex] 
        - manager.transform.position;//物体指向目标的向�?
        Vector2 dir = obj2target.normalized;       
         //设置物体移动速度
        manager.time.rigidbody2D.velocity =paramater.speedCoff * paramater.partolSpeed *  dir ;
        //设置物体永远尝试朝向目标方向
        manager.turn();
         //设置下一路径�?
        //---------------
        if(obj2target.magnitude < paramater.arriveDistance)
        {
            paramater.chaseIndex++;
            //如果越界
            if(paramater.chaseIndex >= paramater.chasePath.vectorPath.Count)
            {
                paramater.reachedEndOfPath = true;
                manager.time.rigidbody2D.velocity = Vector2.zero;
            }
        }
    }
}